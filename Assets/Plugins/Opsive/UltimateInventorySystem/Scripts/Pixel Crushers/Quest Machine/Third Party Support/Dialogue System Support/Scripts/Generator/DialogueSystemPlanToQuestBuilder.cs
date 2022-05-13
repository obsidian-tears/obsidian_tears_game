// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;
using PixelCrushers.QuestMachine;

namespace PixelCrushers.QuestMachine.DialogueSystemSupport
{

    /// <summary>
    /// Class to build a quest from a plan created by a QuestGenerator, using
    /// Dialogue System conversation content.
    /// </summary>
    public class DialogueSystemPlanToQuestBuilder : PlanToQuestBuilder
    {

        protected DialogueSystemActionConversationMap actionConversationMap { get; set; }

        public DialogueSystemPlanToQuestBuilder(DialogueSystemActionConversationMap actionConversationMap) : base()
        {
            this.actionConversationMap = actionConversationMap;
        }

        /// <summary>
        /// Adds the quest's offer text, using a motive conversation if possible.
        /// </summary>
        /// <param name="questBuilder">QuestBuilder.</param>
        /// <param name="mainTargetEntity">Target of the quest.</param>
        /// <param name="mainTargetDescriptor">Descriptor of the target (with count).</param>
        /// <param name="domainName">Domain where the target is located.</param>
        /// <param name="goal">Final step to complete quest.</param>
        /// <param name="goal">Goal step, which may contain completion text.</param>
        protected override void AddOfferText(QuestBuilder questBuilder, string mainTargetEntity, string mainTargetDescriptor, string domainName, PlanStep goal, Motive motive)
        {
            var record = (actionConversationMap != null) ? actionConversationMap.GetRecordForAction(goal.action) : null;
            if (actionConversationMap == null)
            {
                base.AddOfferText(questBuilder, mainTargetEntity, mainTargetDescriptor, domainName, goal, motive);
            }
            else
            {
                var motiveIndex = GetMotiveIndex(goal.action.motives, motive);
                if (!(0 <= motiveIndex && motiveIndex < goal.action.motives.Length))
                {
                    if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: Action '" + goal.action.name + "' has no Motives. Can't set up conversation.", goal.action);
                    return;
                }
                questBuilder.AddContents(questBuilder.quest.offerContentList, CreateConversationContent(record.motiveConversations[motiveIndex].conversation));
            }
        }

        protected virtual int GetMotiveIndex(Motive[] motives, Motive motive)
        {
            if (motives == null) return 0;
            for (int i = 0; i < motives.Length; i++)
            {
                if (motives[i] == motive) return i;
            }
            return 0;
        }

        protected virtual DialogueSystemConversationQuestContent CreateConversationContent(string conversation)
        {
            var content = DialogueSystemConversationQuestContent.CreateInstance<DialogueSystemConversationQuestContent>();
            content.conversation = conversation;
            return content;
        }

        /// <summary>
        /// Adds reward text, setting the {REWARD} tag for use in conversations.
        /// </summary>
        /// <param name="questBuilder"></param>
        /// <param name="entity"></param>
        /// <param name="goal"></param>
        /// <param name="rewardsUIContents"></param>
        /// <param name="rewardSystems"></param>
        protected override void AddRewards(QuestBuilder questBuilder, QuestEntity entity, PlanStep goal, List<QuestContent> rewardsUIContents, List<RewardSystem> rewardSystems)
        {
            base.AddRewards(questBuilder, entity, goal, rewardsUIContents, rewardSystems);

            // Copy added content into rewards[] list:
            var rewards = new List<string>();
            for (int i = 0; i < questBuilder.quest.offerContentList.Count; i++)
            {
                var content = questBuilder.quest.offerContentList[i];
                if (content == null || content is DialogueSystemConversationQuestContent) continue;
                var text = content.runtimeText;
                if (!string.IsNullOrEmpty(text)) rewards.Add(text);
            }

            // If there's more than just the rewards UI contents preface text:
            var numActualRewards = rewards.Count - rewardsUIContents.Count;
            if (numActualRewards > 0)
            {
                // Generate {REWARD} string:
                var rewardString = string.Empty;
                for (int i = 0; i < rewards.Count; i++)
                {
                    rewardString += rewards[i];
                    if (i == rewards.Count - 1) // Add period to end of last reward.
                    {
                        rewardString += ".";
                    }
                    else if (i == rewards.Count - 2) // Add "{and}" before last reward.
                    {
                        if (numActualRewards > 1)
                        {
                            rewardString += QuestMachineTags.ReplaceTags(" {and} ", questBuilder.quest);
                        }
                    }
                    else if (i >= rewardsUIContents.Count) // Otherwise add "," unless we're adding the preliminary text.
                    {
                        rewardString += ", ";
                    }
                }
                if (!string.IsNullOrEmpty(rewardString)) questBuilder.quest.tagDictionary.dict.Add("{REWARD}", rewardString);
            }
        }

        /// <summary>
        /// Adds quest heading text to the main quest's Dialogue, Journal, and HUD categories.
        /// </summary>
        /// <param name="questBuilder">QuestBuilder.</param>
        /// <param name="goal">Goal step, which may contain completion text.</param>
        protected override void AddQuestHeadings(QuestBuilder questBuilder, PlanStep goal)
        {
            //--- Not to dialogue: var hasSuccessfulDialogueText = !StringField.IsNullOrEmpty(goal.action.actionText.completedText.dialogueText);
            var hasSuccessfulJournalText = !StringField.IsNullOrEmpty(goal.action.actionText.completedText.journalText);
            //--- AddQuestHeading(questBuilder, QuestContentCategory.Dialogue, hasSuccessfulDialogueText);
            AddQuestHeading(questBuilder, QuestContentCategory.Journal, hasSuccessfulJournalText);
            AddQuestHeading(questBuilder, QuestContentCategory.HUD, false);
        }

        /// <summary>
        /// Adds text to show in UIs after a quest has been successfully completed.
        /// </summary>
        /// <param name="questBuilder">QuestBuilder.</param>
        /// <param name="mainTargetEntity">Main target entity that quest is about.</param>
        /// <param name="mainTargetDescriptor">Target descriptor (with count).</param>
        /// <param name="domainName">Target's domain.</param>
        /// <param name="goal">Goal step in quest.</param>
        protected override void AddSuccessfulText(QuestBuilder questBuilder, string mainTargetEntity, string mainTargetDescriptor, string domainName, PlanStep goal)
        {
            var successful = questBuilder.quest.GetStateInfo(QuestState.Successful);
            //--- Not to dialogue: var hasSuccessfulDialogueText = !StringField.IsNullOrEmpty(goal.action.actionText.completedText.dialogueText);
            var hasSuccessfulJournalText = !StringField.IsNullOrEmpty(goal.action.actionText.completedText.journalText);
            //---if (hasSuccessfulDialogueText)
            //---{
            //---    var dlgText = questBuilder.CreateBodyContent(ReplaceStepTags(goal.action.actionText.completedText.dialogueText.value, mainTargetEntity, mainTargetDescriptor, domainName, string.Empty, 0));
            //---    questBuilder.AddContents(successful.GetContentList(QuestContentCategory.Dialogue), dlgText);
            //---}
            if (hasSuccessfulJournalText)
            {
                var jrlText = questBuilder.CreateBodyContent(ReplaceStepTags(goal.action.actionText.completedText.journalText.value, mainTargetEntity, mainTargetDescriptor, domainName, string.Empty, 0));
                questBuilder.AddContents(successful.GetContentList(QuestContentCategory.Journal), jrlText);
            }
        }

        /// <summary>
        /// Adds the text for a step.
        /// </summary>
        protected override void AddStepNodeText(QuestBuilder questBuilder, QuestNode conditionNode, QuestStateInfo state, string targetEntity, string targetDescriptor, string domainName,
            string counterName, int requiredCounterValue, PlanStep step, ActionStateText actionStateText)
        {
            if (state != conditionNode.GetStateInfo(QuestNodeState.Active)) return;

            // Text for condition node's Active state:
            var activeState = state;

            // Dialogue:
            var dialogueList = activeState.categorizedContentList[(int)QuestContentCategory.Dialogue];
            var record = (actionConversationMap != null) ? actionConversationMap.GetRecordForAction(step.action) : null;
            if (record != null && !string.IsNullOrEmpty(record.activeStateConversation))
            {
                questBuilder.AddContents(dialogueList, CreateConversationContent(record.activeStateConversation));
            }
            else
            {
                var taskText = ReplaceStepTags(step.action.actionText.activeText.dialogueText.value, targetEntity, targetDescriptor, domainName, counterName, requiredCounterValue);
                var bodyText = questBuilder.CreateBodyContent(taskText);
                dialogueList.contentList.Add(bodyText);
            }

            // Set node's DOMAIN, TARGETDESCRIPTOR, TARGET, TARGETS, COUNTERGOAL tags:
            AddTagsToDictionary(conditionNode.tagDictionary, step);

            // Journal: (unchanged)
            var jrlText = ReplaceStepTags(step.action.actionText.activeText.journalText.value, targetEntity, targetDescriptor, domainName, counterName, requiredCounterValue);
            var jrlbodyText = questBuilder.CreateBodyContent(jrlText);
            var journalList = activeState.categorizedContentList[(int)QuestContentCategory.Journal];
            journalList.contentList.Add(jrlbodyText);

            // HUD: (unchanged)
            var hudText = ReplaceStepTags(step.action.actionText.activeText.hudText.value, targetEntity, targetDescriptor, domainName, counterName, requiredCounterValue);
            var hudbodyText = questBuilder.CreateBodyContent(hudText);
            var hudList = activeState.categorizedContentList[(int)QuestContentCategory.HUD];
            hudList.contentList.Add(hudbodyText);
        }

        protected override void AddTagsToDictionary(TagDictionary tagDictionary, PlanStep goal)
        {
            if (tagDictionary == null || goal == null) return;
            tagDictionary.dict[QuestMachineTags.DOMAIN] = StringField.GetStringValue(goal.fact.domainType.displayName);
            tagDictionary.dict[QuestMachineTags.ACTION] = StringField.GetStringValue(goal.action.displayName);
            tagDictionary.dict[QuestMachineTags.TARGET] = StringField.GetStringValue(goal.fact.entityType.displayName);
            tagDictionary.dict[QuestMachineTags.TARGETS] = StringField.GetStringValue(goal.fact.entityType.pluralDisplayName);
            tagDictionary.dict[QuestMachineTags.TARGETDESCRIPTOR] = goal.fact.entityType.GetDescriptor(goal.requiredCounterValue);
            var completion = goal.action.completion;
            if (completion.mode == ActionCompletion.Mode.Counter)
            {
                tagDictionary.dict[QuestMachineTags.COUNTERGOAL] = goal.requiredCounterValue.ToString();
            }
        }

        /// <summary>
        /// Adds the turn-in conversation to the return node.
        /// </summary>
        protected override void AddReturnNodeText(QuestBuilder questBuilder, QuestNode returnNode, QuestGiver questGiver, string mainTargetEntity, string mainTargetDescriptor, string domainName, PlanStep goal, string hudText)
        {
            var stateInfo = returnNode.stateInfoList[(int)QuestNodeState.Active];
            QuestStateInfo.ValidateCategorizedContentListCount(stateInfo.categorizedContentList);

            // Dialogue: (Use conversation if available.)
            var dialogueList = returnNode.stateInfoList[(int)QuestNodeState.Active].categorizedContentList[(int)QuestContentCategory.Dialogue];
            var record = (actionConversationMap != null) ? actionConversationMap.GetRecordForAction(goal.action) : null;
            if (record != null && !string.IsNullOrEmpty(record.turnInConversation))
            {
                questBuilder.AddContents(dialogueList, CreateConversationContent(record.turnInConversation));
            }
            else
            {
                var successText = ReplaceStepTags(StringField.GetStringValue(goal.action.actionText.successText), mainTargetEntity, mainTargetDescriptor, domainName, string.Empty, 0);
                var bodyText = questBuilder.CreateBodyContent(successText);
                dialogueList.contentList.Add(bodyText);
            }

            // Set node's DOMAIN, TARGETDESCRIPTOR, TARGET, TARGETS, COUNTERGOAL tags:
            AddTagsToDictionary(returnNode.tagDictionary, goal);

            // Journal: (unchanged)
            var jrlText = "{Return to} " + questGiver.displayName;
            var jrlBodyText = questBuilder.CreateBodyContent(jrlText);
            var journalList = returnNode.stateInfoList[(int)QuestNodeState.Active].categorizedContentList[(int)QuestContentCategory.Journal];
            journalList.contentList.Add(jrlBodyText);

            // HUD: (unchanged)
            var hudBodyText = questBuilder.CreateBodyContent(hudText);
            var hudList = returnNode.stateInfoList[(int)QuestNodeState.Active].categorizedContentList[(int)QuestContentCategory.HUD];
            hudList.contentList.Add(hudBodyText);
        }

    }

}