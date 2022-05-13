// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    public delegate void GeneratedQuestDelegate(Quest quest);

    /// <summary>
    /// Class to procedurally generate quests.
    /// </summary>
    public class QuestGenerator
    {

        #region Fields & Properties

        public static int maxSimultaneousPlanners = 5;
        public static int maxGoalActionChecksPerFrame = 100;
        public static int maxStepsPerFrame = 100;
        public static int defaultMaxSearchDepth = 1000;
        public static bool detailedDebug = false;

        private int m_maxSearchDepth = defaultMaxSearchDepth;
        public int maxSearchDepth { get { return m_maxSearchDepth; } set { m_maxSearchDepth = value; } }

        private QuestEntity entity { get; set; }
        private StringField group { get; set; }
        private DomainType domainType { get; set; }
        private WorldModel worldModel { get; set; }
        private bool requireReturnToComplete { get; set; }
        private List<QuestContent> rewardsUIContents { get; set; }
        private List<RewardSystem> rewardSystems { get; set; }
        private UrgentFactSelectionMode goalSelectionMode { get; set; }
        private bool abandonable { get; set; }
        private string[] ignoreList { get; set; }
        private Coroutine coroutine { get; set; }
        private bool cancel { get; set; }

        private PlanStep goal { get; set; }
        private Motive motive { get; set; }
        private List<PlanStep> masterStepList = new List<PlanStep>();
        private Plan plan { get; set; }

        private PlanToQuestBuilder m_planToQuestBuilder = new PlanToQuestBuilder();

        /// <summary>
        /// Helper that creates a quest from a plan. If you want to change the 
        /// way plans are turned into quests, you can create a subclass of
        /// PlanToQuestBuilder and assign it to this property.
        /// </summary>
        public PlanToQuestBuilder planToQuestBuilder
        {
            get { return m_planToQuestBuilder; }
            set { m_planToQuestBuilder = value; }
        }

        #endregion

        #region Entry Points

        public void GenerateQuest(QuestEntity entity, StringField group, DomainType domainType, WorldModel worldModel, bool requireReturnToComplete,
            List<QuestContent> rewardsUIContents, List<RewardSystem> rewardSystems, List<Quest> existingQuests, GeneratedQuestDelegate generatedQuest,
            UrgentFactSelectionMode goalSelectionMode, bool generateAbandonableQuests)
        {
            if (entity == null || domainType == null || worldModel == null) return;
            coroutine = entity.StartCoroutine(GenerateQuestCoroutine(entity, group, domainType, worldModel, requireReturnToComplete, 
                rewardsUIContents, rewardSystems, existingQuests, generatedQuest, goalSelectionMode, generateAbandonableQuests));
        }

        public void CancelGeneration()
        {
            cancel = true;
        }

        private IEnumerator GenerateQuestCoroutine(QuestEntity entity, StringField group, DomainType domainType, WorldModel worldModel, bool requireReturnToComplete,
            List<QuestContent> rewardsUIContents, List<RewardSystem> rewardSystems, List<Quest> existingQuests, GeneratedQuestDelegate generatedQuest,
            UrgentFactSelectionMode goalSelectionMode, bool generateAbandonableQuests)
        {
            this.cancel = false;
            this.entity = entity;
            this.group = group;
            this.domainType = domainType;
            this.worldModel = worldModel;
            this.requireReturnToComplete = requireReturnToComplete;
            this.rewardsUIContents = rewardsUIContents;
            this.rewardSystems = rewardSystems;
            this.ignoreList = GenerateIgnoreList(existingQuests);
            this.goalSelectionMode = goalSelectionMode;
            this.abandonable = generateAbandonableQuests;
            masterStepList = new List<PlanStep>();
            goal = null;
            plan = null;
            Quest quest = null;
            worldModel.observer = new Fact(domainType, entity.entityType, 1);
            yield return DetermineGoal();
            if (!(cancel || goal == null))
            {
                yield return GeneratePlan();
                if (!(cancel || plan == null))
                {
                    BackfillMinimumCounterValues();
                    if (detailedDebug) LogPlan(plan);
                    quest = planToQuestBuilder.ConvertPlanToQuest(entity, group, goal, motive, plan, requireReturnToComplete, rewardsUIContents, rewardSystems);
                }
            }
            if (abandonable && quest != null) quest.isAbandonable = true;
            generatedQuest(quest);
        }

        private string[] GenerateIgnoreList(List<Quest> existingQuests)
        {
            if (existingQuests == null || existingQuests.Count == 0) return null;
            var list = new List<string>();
            for (int i = 0; i < existingQuests.Count; i++)
            {
                if (existingQuests[i] == null) continue;
                var entityTypeName = existingQuests[i].goalEntityTypeName;
                if (string.IsNullOrEmpty(entityTypeName) || list.Contains(entityTypeName)) continue;
                list.Add(entityTypeName);
            }
            return list.ToArray();
        }

        #endregion

        #region Determine Goal

        private IEnumerator DetermineGoal()
        {
            // Search world model for most urgent fact:
            Fact[] mostUrgentFacts;
            var cumulativeUrgency = worldModel.ComputeUrgency(goalSelectionMode, out mostUrgentFacts, ignoreList, detailedDebug);
            var mostUrgentFact = ChooseWeightedMostUrgentFact(mostUrgentFacts);
            if (cumulativeUrgency <= 0 || mostUrgentFact == null)
            {
                if (QuestMachine.debug || detailedDebug) Debug.Log("Quest Machine: [Generator] " + entity.displayName + ": No facts are currently urgent for " + entity.displayName + ". Not generating a new quest.", entity);
                yield break;
            }
            if (QuestMachine.debug || detailedDebug) Debug.Log("Quest Machine: [Generator] " + entity.displayName + ": Most urgent fact: " + mostUrgentFact.count + " " + 
                mostUrgentFact.entityType.name + " in " + mostUrgentFact.domainType.name, entity);

            // Choose goal action to perform on that fact:
            float bestUrgency = Mathf.Infinity;
            Action bestAction = null;
            Motive bestMotive = null;
            var actions = GetEntityActions(mostUrgentFact.entityType);
            if (actions == null) yield break;
            int numChecks = 0;
            for (int i = 0; i < actions.Count; i++)
            {
                numChecks++;
                if (numChecks > maxGoalActionChecksPerFrame)
                {
                    numChecks = 0;
                    yield return null;
                }
                var action = actions[i];
                if (action == null) continue;
                var wm = new WorldModel(worldModel);
                wm.ApplyAction(mostUrgentFact, action);
                Fact newMostUrgentFact;
                var newUrgency = wm.ComputeUrgency(out newMostUrgentFact);
                var bestMotiveForAction = ChooseBestMotive(action.motives);
                var weightedUrgency = (bestMotiveForAction != null) ? (newUrgency - (GetDriveAlignment(bestMotiveForAction.driveValues) * newUrgency)) : newUrgency;
                if (weightedUrgency < bestUrgency) // Select goal action based on resulting urgency weighted by how well the motive aligns with the giver's drives.
                {
                    bestUrgency = weightedUrgency;
                    bestAction = action;
                    bestMotive = bestMotiveForAction;
                }
            }
            if (bestAction == null) yield break;
            motive = bestMotive;
            goal = new PlanStep(mostUrgentFact, bestAction, Mathf.CeilToInt(mostUrgentFact.entityType.maxCountInAction.Evaluate(mostUrgentFact.count)));
            if (QuestMachine.debug || detailedDebug) Debug.Log("Quest Machine: [Generator] " + entity.displayName + ": Goal: " + bestAction.name + " " + 
                mostUrgentFact.count + " " + mostUrgentFact.entityType.name, entity);
        }

        private Fact ChooseWeightedMostUrgentFact(Fact[] mostUrgentFacts)
        {
            if (mostUrgentFacts == null || mostUrgentFacts.Length == 0) return null;
            float totalWeight = 0;
            for (int i = 0; i < mostUrgentFacts.Length; i++)
            {
                var fact = mostUrgentFacts[i];
                if (fact == null) continue;
                totalWeight += fact.urgency;
            }
            var randomValue = Random.value * totalWeight;
            float min = 0;
            for (int i = 0; i < mostUrgentFacts.Length; i++)
            {
                var fact = mostUrgentFacts[i];
                if (fact == null) continue;
                var max = min + fact.urgency;
                if (min <= randomValue && randomValue <= max)
                {
                    return fact;
                }
            }
            return null;
        }

        #endregion

        #region Motives

        private Motive ChooseBestMotive(Motive[] motives)
        {
            if (motives == null) return null;
            Motive bestMotive = null;
            float bestAlignment = -Mathf.Infinity;
            for (int i = 0; i < motives.Length; i++)
            {
                var motive = motives[i];
                if (motive == null) continue;
                var alignment = GetDriveAlignment(motive.driveValues);

                if (detailedDebug) Debug.Log("Quest Machine: [Generator] Motive Alignment: entity=" + ((entity != null) ? entity.name : "null") + 
                    " motive=" + motive.text + " alignment=" + alignment, entity);

                if (alignment > bestAlignment)
                {
                    bestAlignment = alignment;
                    bestMotive = motive;
                }
            }
            return bestMotive;
        }

        private float GetDriveAlignment(DriveValue[] driveValues)
        {
            if (driveValues == null) return 0;
            float totalAlignment = 0;
            int count = 0;
            for (int i = 0; i < driveValues.Length; i++)
            {
                var driveValue = driveValues[i];
                if (driveValue == null || driveValue.drive == null) continue;
                var entityDriveValue = LookupEntityDriveValue(driveValue.drive);
                if (entityDriveValue == null) continue;
                float difference = Mathf.Abs(driveValue.value - entityDriveValue.value);
                var alignment = (200f - difference) / 200f;
                totalAlignment += alignment;
                count++;
            }
            return (count == 0) ? 0 : (totalAlignment / (float)count);
        }

        private DriveValue LookupEntityDriveValue(Drive drive)
        {
            if (drive == null || entity == null || entity.entityType == null) return null;
            return entity.entityType.LookupDriveValue(drive);
        }

        #endregion

        #region Generate Plan

        private IEnumerator GeneratePlan()
        {
            yield return BFS(worldModel, goal);
        }

        // Currently use a BFS rather than A* because it produces better results since there's no good heuristic yet.
        private IEnumerator BFS(WorldModel initialWorldModel, PlanStep goal)
        {
            yield return null;

            var Q = new Queue<Plan>();

            // Queue the initial state:
            Q.Enqueue(new Plan(null, null, initialWorldModel));

            int numStepsChecked = 0;
            int safeguard = 0;
            while (Q.Count > 0 && safeguard < maxSearchDepth)
            {
                safeguard++;

                numStepsChecked++;
                if (numStepsChecked > maxStepsPerFrame)
                {
                    numStepsChecked = 0;
                    yield return null;
                }

                var current = Q.Dequeue();

                //--- For debugging:
                //var indent = string.Empty;
                //for (int i = 0; i < current.steps.Count; i++) { indent += "        "; }
                //var lastStep = (current.steps.Count > 0) ? current.steps[current.steps.Count - 1] : null;
                //Debug.Log(indent + "Goal met (lastStep=" + lastStep + ")? " + current.worldModel.AreRequirementsMet(goal.action.requirements));

                // If the current state meets the goal requirements, return a finished plan:
                if (current.worldModel.AreRequirementsMet(goal.action.requirements, goal.fact))
                {
                    plan = new Plan(current, goal, null);
                    yield break;
                }

                // Otherwise queue up the actions that are valid in the current state:
                var lastStep = (current.steps.Count > 0) ? current.steps[current.steps.Count - 1] : null;
                foreach (var fact in current.worldModel.facts)
                {
                    var actions = GetEntityActions(fact.entityType);
                    if (actions == null) continue;
                    foreach (var action in actions)
                    {
                        if (fact == null || fact.entityType == null || action == null) continue;
                        if (lastStep != null && fact == lastStep.fact && action == lastStep.action) continue; // Don't repeat last action.
                        if (!current.worldModel.AreRequirementsMet(action.requirements)) continue; // If not valid, don't queue it.

                        //---Debug.Log(indent + action.name + " " + fact.entityType.name);

                        var newWorldModel = new WorldModel(current.worldModel);
                        newWorldModel.ApplyAction(fact, action);
                        var newPlan = new Plan(current, GetStep(fact, action), newWorldModel);
                        Q.Enqueue(newPlan);
                    }
                }
            }
            if (QuestMachine.debug || detailedDebug) Debug.Log("Quest Machine: [Generator] Could not create quest. Exceeded safeguard while generating plan to " + 
                goal.action.name + " " + goal.fact.entityType.name + ".", entity);
        }

        private List<Action> GetEntityActions(EntityType entityType)
        {
            // Nonrecursively gather a list of all actions on the entity and its parents:
            if (entityType == null) return null;
            if (entityType.parents == null || entityType.parents.Count == 0) return entityType.actions;
            var actions = new List<Action>();
            var processed = new List<EntityType>();
            var Q = new Queue<EntityType>();
            Q.Enqueue(entityType);
            int safeguard = 0;
            while (Q.Count > 0 && safeguard < 1000)
            {
                safeguard++;
                var et = Q.Dequeue();
                if (et == null) continue;
                processed.Add(et);
                if (et.parents != null)
                {
                    // Add parents to queue to check for actions:
                    for (int i = 0; i < et.parents.Count; i++)
                    {
                        var parent = et.parents[i];
                        if (parent != null && !processed.Contains(parent)) Q.Enqueue(parent);
                    }
                }
                if (et.actions != null)
                {
                    // Add actions to list:
                    for (int i = 0; i < et.actions.Count; i++)
                    {
                        var action = et.actions[i];
                        if (action != null && !actions.Contains(action)) actions.Add(action);
                    }
                }
            }
            return actions;
        }

        private PlanStep GetStep(Fact fact, Action action)
        {
            foreach (var step in masterStepList)
            {
                if (step.fact == fact && step.action == action) return step;
            }
            var newStep = new PlanStep(fact, action);
            masterStepList.Add(newStep);
            return newStep;
        }

        /// <summary>
        /// For each counter type (e.g., numApplesPicked, numOrcsKilled), find the 
        /// highest required value. Then set all required values for that counter
        /// type to that value.
        /// </summary>
        private void BackfillMinimumCounterValues()
        {
            var requiredCounterValue = new Dictionary<string, int>();
            for (int i = 0; i < plan.steps.Count; i++)
            {
                var step = plan.steps[i];
                if (step.action.completion.mode != ActionCompletion.Mode.Counter) continue;
                var stepCounterName = StringField.GetStringValue(step.action.completion.baseCounterName);
                var stepRequiredValue = step.action.completion.requiredValue;
                if (!requiredCounterValue.ContainsKey(stepCounterName))
                {
                    requiredCounterValue.Add(stepCounterName, stepRequiredValue);
                }
                else
                {
                    requiredCounterValue[stepCounterName] = Mathf.Max(requiredCounterValue[stepCounterName], stepRequiredValue);
                }
            }
            for (int i = 0; i < plan.steps.Count; i++)
            {
                var step = plan.steps[i];
                if (step.action.completion.mode != ActionCompletion.Mode.Counter) continue;
                var stepCounterName = StringField.GetStringValue(step.action.completion.baseCounterName);
                step.requiredCounterValue = Mathf.Max(step.requiredCounterValue, requiredCounterValue[stepCounterName]);
            }
        }

        private void LogPlan(Plan plan)
        {
            var s = "Quest Machine: [Generator] Plan (" + plan.steps.Count + " steps):\n";
            foreach (var planStep in plan.steps)
            {
                if (planStep.fact != null)
                {
                    s += "   " + planStep.ToString() + "\n";
                }
                else
                {
                    s += "   (null)\n";
                }
            }
            Debug.Log(s);
        }

        #endregion

    }

}