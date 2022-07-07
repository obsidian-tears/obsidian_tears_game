// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Utility class for procedurally building quests.
    /// </summary>
    public class QuestBuilder
    {

        public Quest quest { get; set; }

        #region Constructors

        /// <summary>
        /// Creates a new quest.
        /// </summary>
        /// <param name="name">Name to use for quest name, ID, and title.</param>
        public QuestBuilder(StringField name)
        {
            CreateQuest(StringField.GetStringValue(name), name, name);
        }

        /// <summary>
        /// Creates a new quest.
        /// </summary>
        /// <param name="name">Name to use for quest name, ID, and title.</param>
        public QuestBuilder(string name)
        {
            CreateQuest(name, name, name);
        }

        /// <summary>
        /// Creates a new quest.
        /// </summary>
        /// <param name="name">Quest name.</param>
        /// <param name="id">Quest ID.</param>
        /// <param name="title">Quest title (visible in UIs).</param>
        public QuestBuilder(string name, StringField id, StringField title)
        {
            CreateQuest(name, id, title);
        }

        /// <summary>
        /// Creates a new quest.
        /// </summary>
        /// <param name="name">Quest name.</param>
        /// <param name="id">Quest ID.</param>
        /// <param name="title">Quest title (visible in UIs).</param>
        public QuestBuilder(string name, string id, string title)
        {
            CreateQuest(name, new StringField(id), new StringField(title));
        }

        /// <summary>
        /// Creates a QuestBuilder for an existing quest. Use this form to add
        /// new content to an existing quest.
        /// </summary>
        /// <param name="quest">The quest to edit.</param>
        public QuestBuilder(Quest quest)
        {
            this.quest = quest;
        }

        private void CreateQuest(string name, StringField id, StringField title)
        {
            quest = Quest.CreateInstance<Quest>();
            quest.Initialize();
            quest.isAsset = false;
            quest.isInstance = true;
            quest.name = name;
            quest.id = id;
            quest.title = title;
            GetStartNode().id = new StringField(id + ".start");
        }

        private void CreateQuest(string name, string id, string title)
        {
            CreateQuest(name, new StringField(id), new StringField(title));
        }

        public Quest ToQuest()
        {
            if (quest == null) return null;
            ValidateListSizes();
            quest.SetRuntimeReferences();
            return quest;
        }

        private void ValidateListSizes()
        {
            var numStates = Enum.GetNames(typeof(QuestState)).Length;
            QuestStateInfo.ValidateStateInfoListCount(quest.stateInfoList, numStates);
            for (int i = 0; i < numStates; i++)
            {
                QuestStateInfo.ValidateCategorizedContentListCount(quest.stateInfoList[i].categorizedContentList);
            }
        }

        #endregion

        #region Counters

        public QuestCounter AddCounter(StringField counterName, int initialValue, int minValue, int maxValue, bool randomizeInitialValue, QuestCounterUpdateMode updateMode)
        {
            if (quest.counterList.Find(x => StringField.Equals(x.name, counterName)) != null)
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: Counter '" + counterName + "' already exists in QuestBuilder.");
                return null;
            }
            var counter = new QuestCounter(counterName, initialValue, minValue, maxValue, updateMode);
            quest.counterList.Add(counter);
            return counter;
        }

        public QuestCounter AddCounter(string counterName, int initialValue, int minValue, int maxValue, bool randomizeInitialValue, QuestCounterUpdateMode updateMode)
        {
            return AddCounter(new StringField(counterName), initialValue, minValue, maxValue, randomizeInitialValue, updateMode); ;
        }

        public void AddCounterMessageEvent(StringField counterName, StringField targetID, StringField message, StringField parameter,
            QuestCounterMessageEvent.Operation operation, int literalValue = 0)
        {
            var counter = quest.GetCounter(counterName);
            if (counter == null)
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: Counter '" + counterName + "' isn't present in QuestBuilder.");
                return;
            }
            counter.updateMode = QuestCounterUpdateMode.Messages;
            counter.messageEventList.Add(new QuestCounterMessageEvent(targetID, message, parameter, operation, literalValue));
        }

        public void AddCounterMessageEvent(StringField counterName, string targetID, string message, string parameter,
            QuestCounterMessageEvent.Operation operation, int literalValue = 0)
        {
            AddCounterMessageEvent(counterName, new StringField(targetID), new StringField(message), new StringField(parameter), operation, literalValue);
        }

        public void AddCounterMessageEvent(string counterName, string targetID, string message, string parameter,
            QuestCounterMessageEvent.Operation operation, int literalValue = 0)
        {
            AddCounterMessageEvent(new StringField(counterName), new StringField(targetID), new StringField(message), new StringField(parameter), operation, literalValue);
        }

        #endregion

        #region Rewards

        public void AssignRewards(RewardSystem[] rewardSystems, int points)
        {
            if (rewardSystems == null) return;
            var pointsRemaining = points;
            for (int i = 0; i < rewardSystems.Length; i++)
            {
                var rewardSystem = rewardSystems[i];
                if (rewardSystem == null) continue;
                pointsRemaining = rewardSystem.DetermineReward(pointsRemaining, quest);
                if (pointsRemaining <= 0) break;
            }
        }

        #endregion

        #region Offer Content

        public void AddOfferContents(params QuestContent[] contents)
        {
            AddContents(quest.offerContentList, contents);
        }

        public void AddOfferUnmetContents(params QuestContent[] contents)
        {
            AddContents(quest.offerConditionsUnmetContentList, contents);
        }

        #endregion

        #region Create Content 

        //[TODO] Add error reporting to all null checks in QuestBuilder.

        public void AddContents(QuestContentSet contentSet, params QuestContent[] contents)
        {
            if (contentSet == null) return;
            AddContents(contentSet.contentList, contents);
        }

        public void AddContents(List<QuestContent> contentList, params QuestContent[] contents)
        {
            if (contentList == null || contents == null) return;
            contentList.AddRange(contents);
        }

        public QuestContent CreateTitleContent()
        {
            var content = HeadingTextQuestContent.CreateInstance<HeadingTextQuestContent>();
            content.name = "title";
            content.useQuestTitle = true;
            content.headingLevel = 1;
            return content;
        }

        public QuestContent CreateHeadingContent(StringField text, int level)
        {
            var content = HeadingTextQuestContent.CreateInstance<HeadingTextQuestContent>();
            content.name = "heading";
            content.useQuestTitle = false;
            content.headingText = text;
            content.headingLevel = level;
            return content;
        }

        public QuestContent CreateHeadingContent(string text, int level)
        {
            return CreateHeadingContent(new StringField(text), level);
        }

        public QuestContent CreateBodyContent(StringField text)
        {
            var content = BodyTextQuestContent.CreateInstance<BodyTextQuestContent>();
            content.name = "text";
            content.bodyText = new StringField(text);
            return content;
        }

        public QuestContent CreateBodyContent(string text)
        {
            return CreateBodyContent(new StringField(text));
        }

        //[TODO] Other content types in QuestBuilder.

        #endregion

        #region Nodes

        public QuestNode GetStartNode()
        {
            return quest.nodeList[0];
        }

        public QuestNode AddNode(QuestNode parent, StringField id, StringField internalName, QuestNodeType nodeType, bool isOptional = false)
        {
            if (parent == null)
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: QuestBuilder.AddNode must be provided a valid parent node.");
                return null;
            }
            if (parent.childIndexList == null) return null;
            parent.childIndexList.Add(quest.nodeList.Count);
            var node = new QuestNode(id, internalName, nodeType, isOptional);
            node.canvasRect = new Rect(parent.canvasRect.x, parent.canvasRect.y + 20 + QuestNode.DefaultNodeHeight, QuestNode.DefaultNodeWidth, QuestNode.DefaultNodeHeight);
            quest.nodeList.Add(node);
            QuestStateInfo.ValidateStateInfoListCount(node.stateInfoList);
            QuestStateInfo.ValidateCategorizedContentListCount(node.stateInfoList[(int)QuestNodeState.Active].categorizedContentList);
            QuestStateInfo.ValidateCategorizedContentListCount(node.stateInfoList[(int)QuestNodeState.Inactive].categorizedContentList);
            QuestStateInfo.ValidateCategorizedContentListCount(node.stateInfoList[(int)QuestNodeState.True].categorizedContentList);
            return node;
        }

        public QuestNode AddNode(QuestNode parent, string id, string internalName, QuestNodeType nodeType, bool isOptional = false)
        {
            return AddNode(parent, new StringField(id), new StringField(internalName), nodeType, isOptional);
        }

        public QuestNode AddSuccessNode(QuestNode parent)
        {
            return AddNode(parent, "success", "Success", QuestNodeType.Success);
        }

        public QuestNode AddFailureNode(QuestNode parent)
        {
            return AddNode(parent, "failure", "Failure", QuestNodeType.Failure);
        }

        public QuestNode AddPassthroughNode(QuestNode parent, StringField id, StringField internalName)
        {
            return AddNode(parent, id, internalName, QuestNodeType.Passthrough);
        }

        public QuestNode AddPassthroughNode(QuestNode parent, string id, string internalName)
        {
            return AddPassthroughNode(parent, new StringField(id), new StringField(internalName));
        }

        public QuestNode AddConditionNode(QuestNode parent, StringField id, StringField internalName, ConditionCountMode conditionCountMode = ConditionCountMode.All, bool isOptional = false)
        {
            var node = AddNode(parent, id, internalName, QuestNodeType.Condition, isOptional);
            if (node == null) return null;
            node.conditionSet.conditionCountMode = conditionCountMode;
            return node;
        }

        public QuestNode AddConditionNode(QuestNode parent, string id, string internalName, ConditionCountMode conditionCountMode = ConditionCountMode.All, bool isOptional = false)
        {
            return AddConditionNode(parent, new StringField(id), new StringField(internalName), conditionCountMode, isOptional);
        }

        public QuestNode AddDiscussQuestNode(QuestNode parent, QuestMessageParticipant targetSpecifier, StringField targetID, bool isOptional = false)
        {
            var node = AddConditionNode(parent, "talkTo" + targetID, "Talk to " + targetID, ConditionCountMode.All, isOptional);
            AddMessageCondition(node, QuestMessageParticipant.Quester, new StringField(), targetSpecifier, targetID, new StringField(QuestMachineMessages.DiscussedQuestMessage), quest.id);
            return node;
        }

        #endregion

        #region Conditions

        public CounterQuestCondition AddCounterCondition(QuestNode node, StringField counterName, CounterValueConditionMode conditionMode, QuestNumber requiredValue)
        {
            var condition = CounterQuestCondition.CreateInstance<CounterQuestCondition>();
            condition.name = "counterCondition";
            condition.counterIndex = quest.GetCounterIndex(counterName);
            condition.counterValueMode = conditionMode;
            condition.requiredCounterValue = requiredValue;
            node.conditionSet.conditionList.Add(condition);
            return condition;
        }

        public CounterQuestCondition AddCounterCondition(QuestNode node, StringField counterName, CounterValueConditionMode conditionMode, int requiredValue)
        {
            return AddCounterCondition(node, counterName, conditionMode, new QuestNumber(requiredValue));
        }

        public CounterQuestCondition AddCounterCondition(QuestNode node, string counterName, CounterValueConditionMode conditionMode, QuestNumber requiredValue)
        {
            return AddCounterCondition(node, new StringField(counterName), conditionMode, requiredValue);
        }

        public CounterQuestCondition AddCounterCondition(QuestNode node, string counterName, CounterValueConditionMode conditionMode, int requiredValue)
        {
            return AddCounterCondition(node, new StringField(counterName), conditionMode, new QuestNumber(requiredValue));
        }

        public MessageQuestCondition AddMessageCondition(QuestNode node,
            QuestMessageParticipant senderSpecifier, StringField senderID,
            QuestMessageParticipant targetSpecifier, StringField targetID,
            StringField message, StringField parameter, MessageValue value = null)
        {
            var condition = MessageQuestCondition.CreateInstance<MessageQuestCondition>();
            condition.name = "messageCondition";
            condition.senderSpecifier = senderSpecifier;
            condition.senderID = senderID;
            condition.targetSpecifier = targetSpecifier;
            condition.targetID = targetID;
            condition.message = message;
            condition.parameter = parameter;
            condition.value = (value != null) ? value : new MessageValue();
            node.conditionSet.conditionList.Add(condition);
            return condition;
        }

        public MessageQuestCondition AddMessageCondition(QuestNode node,
            QuestMessageParticipant senderSpecifier, string senderID,
            QuestMessageParticipant targetSpecifier, string targetID,
            string message, string parameter, MessageValue value = null)
        {
            return AddMessageCondition(node, senderSpecifier, new StringField(targetID), targetSpecifier, new StringField(targetID),
                new StringField(message), new StringField(parameter), value);
        }

        #endregion

        #region Actions

        public QuestAction CreateAlertAction(StringField text)
        {
            return CreateAlertAction(StringField.GetStringValue(text));
        }

        public QuestAction CreateAlertAction(string text)
        {
            var alertAction = AlertQuestAction.CreateInstance<AlertQuestAction>();
            AddContents(alertAction.contentList, CreateBodyContent(text));
            return alertAction;
        }

        public QuestAction CreateMessageAction(StringField message, StringField parameter)
        {
            return CreateMessageAction(StringField.GetStringValue(message), StringField.GetStringValue(parameter));
        }

        public QuestAction CreateMessageAction(string message, string parameter)
        {
            var messageAction = MessageQuestAction.CreateInstance<MessageQuestAction>();
            messageAction.message = new StringField(message);
            messageAction.parameter = new StringField(parameter);
            return messageAction;
        }

        public QuestAction CreateMessageAction(StringField text)
        {
            return CreateMessageAction(StringField.GetStringValue(text));
        }

        public QuestAction CreateMessageAction(string text)
        {
            if (text.Contains(":")) // Parameter?
            {
                var colonPos = text.IndexOf(':');
                return CreateMessageAction(text.Substring(colonPos + 1), text.Substring(0, colonPos));
            }
            else
            {
                return CreateMessageAction(text, string.Empty);
            }
        }

        public QuestAction CreateSetIndicatorAction(StringField questID, StringField entityID, QuestIndicatorState indicatorState)
        {
            var indicatorAction = SetIndicatorQuestAction.CreateInstance<SetIndicatorQuestAction>();
            indicatorAction.questID = questID;
            indicatorAction.entityID = entityID;
            indicatorAction.questIndicatorState = indicatorState;
            return indicatorAction;
        }

        //[TODO] Other action types in QuestBuilder.

        #endregion

    }

}
