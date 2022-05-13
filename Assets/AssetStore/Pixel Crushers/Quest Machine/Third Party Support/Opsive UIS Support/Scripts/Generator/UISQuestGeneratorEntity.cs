// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine.UISSupport
{

    /// <summary>
    /// This subclass of QuestGeneratorEntity assigns a UISPlanToQuestBuilder, which
    /// is a subclass of PlanToQuestBuilder that handles collection of UIS items.
    /// </summary>
    [AddComponentMenu("Tools/Pixel Crushers/Quest Machine/Third Party/UIS/Generator/UIS Quest Generator Entity")]
    public class UISQuestGeneratorEntity : QuestGeneratorEntity
    {
        public override void Start()
        {
            base.Start();
            questGenerator.planToQuestBuilder = new UISPlanToQuestBuilder();
        }
    }

    public class UISPlanToQuestBuilder : PlanToQuestBuilder
    {
        // Keep track of what items we need to take from the player when the player returns:
        public List<string> itemsToTake;
        public List<int> amountToTake;

        public override Quest ConvertPlanToQuest(QuestEntity entity, StringField group, PlanStep goal, Motive motive, Plan plan, bool requireReturnToComplete, List<QuestContent> rewardsUIContents, List<RewardSystem> rewardSystems)
        {
            itemsToTake = new List<string>();
            amountToTake = new List<int>();
            var quest = base.ConvertPlanToQuest(entity, group, goal, motive, plan, requireReturnToComplete, rewardsUIContents, rewardSystems);

            // Add child nodes to each UISItemAmountQuestCondition node that backstep if the item count drops below the requirement:
            var questBuilder = new QuestBuilder(quest);
            var preRevertNodeCount = quest.nodeList.Count;
            for (int i = quest.nodeList.Count - 1; i > 0; i--)
            {
                var parentNode = quest.nodeList[i];
                if (parentNode.id.value.StartsWith("revert.")) continue;
                var isItemConditionNode = parentNode.conditionSet.conditionList.Count > 0 && parentNode.conditionSet.conditionList[0] is UISItemAmountQuestCondition;
                if (isItemConditionNode)
                {
                    var parentCondition = parentNode.conditionSet.conditionList[0] as UISItemAmountQuestCondition;

                    // Create node that checks if we need to backsteps:
                    var id = new StringField("revert." + parentNode.id);
                    var internalName = new StringField("Revert " + parentNode.internalName);
                    var revertNode = questBuilder.AddConditionNode(parentNode, id, internalName, ConditionCountMode.All);
                    revertNode.canvasRect = new Rect(revertNode.canvasRect.x + 20 + QuestNode.DefaultNodeWidth, revertNode.canvasRect.y, revertNode.canvasRect.width, revertNode.canvasRect.height);

                    // Condition: If item count drops below required value:
                    var revertCondition = UISItemAmountQuestCondition.CreateInstance<UISItemAmountQuestCondition>();
                    revertCondition.itemName = parentCondition.itemName;
                    revertCondition.comparisonMode = CounterValueConditionMode.AtMost;
                    revertCondition.amount = new QuestNumber(parentCondition.amount.literalValue - 1);
                    revertCondition.counterIndex = parentCondition.counterIndex;
                    revertNode.conditionSet.conditionList.Add(revertCondition);
                    var revertActionList = revertNode.GetStateInfo(QuestNodeState.True).actionList;

                    // Action: Backstep:
                    // - Set myself inactive:
                    var action = SetQuestNodeStateQuestAction.CreateInstance<SetQuestNodeStateQuestAction>();
                    action.questNodeID = new StringField(revertNode.id);
                    action.state = QuestNodeState.Inactive;
                    revertActionList.Add(action);
                    // - Set later nodes inactive:
                    for (int j = i + 1; j < preRevertNodeCount; j++)
                    {
                        var laterNode = quest.nodeList[j];
                        action = SetQuestNodeStateQuestAction.CreateInstance<SetQuestNodeStateQuestAction>();
                        action.questNodeID = new StringField(laterNode.id);
                        action.state = QuestNodeState.Inactive;
                        revertActionList.Add(action);
                    }
                    // - Set parent node back to active:
                    action = SetQuestNodeStateQuestAction.CreateInstance<SetQuestNodeStateQuestAction>();
                    action.questNodeID = new StringField(parentNode.id);
                    action.state = QuestNodeState.Active;
                    revertActionList.Add(action);
                }
            }

            return quest;
        }

        // We override AddStepCondition to handle UISGetItemAction, which
        // is an action to get a specified amount of an Inventory Pro item.
        protected override void AddStepCondition(QuestBuilder questBuilder, QuestNode conditionNode, string targetEntity, string targetDescriptor, string domainName, HashSet<string> counterNames, out string counterName, out int requiredCounterValue, PlanStep goal, PlanStep step)
        {
            if (step.action is UISGetItemAction)
            {
                // Add node that requires player to get a specified amount of an Inventory Pro item:
                var quest = questBuilder.quest;
                var completion = step.action.completion;
                counterName = goal.fact.entityType.pluralDisplayName.value + completion.baseCounterName.value;
                if (!counterNames.Contains(counterName))
                {
                    var counter = questBuilder.AddCounter(counterName, completion.initialValue, completion.minValue, completion.maxValue, false, completion.updateMode);
                }
                requiredCounterValue = Mathf.Min(step.requiredCounterValue, step.fact.count);

                var condition = UISItemAmountQuestCondition.CreateInstance<UISItemAmountQuestCondition>();
                condition.itemName = new StringField(step.fact.entityType.name);
                condition.comparisonMode = CounterValueConditionMode.AtLeast;
                condition.amount = new QuestNumber(requiredCounterValue);
                condition.counterIndex = quest.GetCounterIndex(counterName);
                conditionNode.conditionSet.conditionList.Add(condition);
                itemsToTake.Add(StringField.GetStringValue(condition.itemName));
                amountToTake.Add(requiredCounterValue);
            }
            else
            {
                base.AddStepCondition(questBuilder, conditionNode, targetEntity, targetDescriptor, domainName, counterNames, out counterName, out requiredCounterValue, goal, step);
            }
        }

        // We override AddReturnNode to also remove items that the player was asked to get:
        protected override QuestNode AddReturnNode(QuestBuilder questBuilder, QuestNode previousNode, QuestEntity entity, string mainTargetEntity, string mainTargetDescriptor, string domainName, PlanStep goal, int rewardsContentIndex = 9999)
        {
            var returnNode = base.AddReturnNode(questBuilder, previousNode, entity, mainTargetEntity, mainTargetDescriptor, domainName, goal, rewardsContentIndex);
            var successInfo = questBuilder.quest.GetStateInfo(QuestState.Successful);
            for (int i = 0; i < itemsToTake.Count; i++)
            {
                var itemAction = UISRemoveItemQuestAction.CreateInstance<UISRemoveItemQuestAction>();
                itemAction.itemName = new StringField(itemsToTake[i]);
                itemAction.amount = new QuestNumber(amountToTake[i]);
                successInfo.actionList.Add(itemAction);
            }
            return returnNode;
        }

    }
}