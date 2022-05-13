// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Wizard that adds a counter and condition node.
    /// </summary>
    public class QuestEditorCounterWizard : QuestEditorWizard
    {
        protected string counterName = "orcsKilled";
        protected int min = 0;
        protected int max = 99;
        protected string incrementMessage = "Killed:Orc";
        protected string hudInstructionsText = "Kill {1} Orcs";
        protected string hudCountText = "{0}/{1} Killed";
        protected string journalText = "{QUESTGIVER} has asked you to kill {1} Orcs.";
        protected string dialogueText = "I want you to kill {1} Orcs.";
        protected bool leadsToSuccess = false;
        protected const string helpText = "This wizard adds a counter and a condition node that requires the counter to reach a specified value. The fields have been set to example values. Change them and click Add. {0} will be replaced by the current counter value, {1} by the goal value.";
        protected GUIContent incrementGUIContent = new GUIContent("Message", "To increment, listen for this message. Parameter is separated by colon (:).");
        protected GUIContent hudInstructionsGUIContent = new GUIContent("HUD Text", "Show this alert/journal text when counter node becomes active. {0} is current counter value, {1} is max value. Leave blank to skip.");
        protected GUIContent hudCountGUIContent = new GUIContent("HUD Count", "{0} is current value, {1} is max value.");
        protected GUIContent journalTextGUIContent = new GUIContent("Journal Text", "Show this journal text when this node is active. {0} is current counter value, {1} is max value.");
        protected GUIContent dialogueTextGUIContent = new GUIContent("Dialogue Text", "Show this dialogue content when this node is active. {0} is current counter value, {1} is max value.");
        protected GUIContent leadsToSuccessGUIContent = new GUIContent("Leads to Success", "Add a success node immediately after this node.");

        public QuestEditorCounterWizard(int clickedIndex) : base(clickedIndex) { }

        public override bool Draw()
        {
            EditorGUILayout.LabelField(string.Empty);
            EditorGUILayout.LabelField("Add Counter & Condition Node", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(helpText, MessageType.Info);
            counterName = EditorGUILayout.TextField("Counter Name", counterName);
            min = EditorGUILayout.IntField("Min", min);
            max = EditorGUILayout.IntField("Max (Goal)", max);
            incrementMessage = EditorGUILayout.TextField(incrementGUIContent, incrementMessage);
            hudInstructionsText = EditorGUILayout.TextField(hudInstructionsGUIContent, hudInstructionsText);
            hudCountText = EditorGUILayout.TextField(hudCountGUIContent, hudCountText);
            journalText = EditorGUILayout.TextField(journalTextGUIContent, journalText);
            dialogueText = EditorGUILayout.TextField(dialogueTextGUIContent, dialogueText);
            leadsToSuccess = EditorGUILayout.Toggle(leadsToSuccessGUIContent, leadsToSuccess);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(counterName) || (max < min));
            if (GUILayout.Button("Add", GUILayout.Width(80)))
            {
                AddCounterAndNode();
                return false;
            }
            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button("Cancel", GUILayout.Width(80)))
            {
                return false;
            }
            EditorGUILayout.EndHorizontal();
            return true;
        }

        protected virtual void AddCounterAndNode()
        {
            var quest = QuestEditorWindow.selectedQuest;
            if (quest == null) return;

            // Add counter:
            var existingCounter = quest.GetCounter(counterName); // Delete existing first.
            if (existingCounter != null) quest.counterList.Remove(existingCounter);
            var counter = new QuestCounter(new StringField(counterName), Mathf.Max(0, min), min, max, QuestCounterUpdateMode.Messages);
            var messageAndParameter = incrementMessage.Split(':');
            counter.messageEventList.Add(new QuestCounterMessageEvent(new StringField(quest.id),
                new StringField(messageAndParameter[0]), new StringField(messageAndParameter[1]),
                QuestCounterMessageEvent.Operation.ModifyByLiteralValue, 1));
            quest.counterList.Add(counter);

            // Add a new node:
            var parentNode = GetParentNode(quest);
            if (parentNode == null) return;
            var node = new QuestNode(new StringField(string.Format(hudInstructionsText, min.ToString(), max.ToString())), new StringField(), QuestNodeType.Condition);
            quest.nodeList.Add(node);
            node.childIndexList.AddRange(parentNode.childIndexList);
            parentNode.childIndexList.Clear();
            parentNode.childIndexList.Add(quest.nodeList.Count - 1);
            node.canvasRect = new Rect(parentNode.canvasRect.x + QuestNode.DefaultNodeWidth + 20, parentNode.canvasRect.y, QuestNode.DefaultNodeWidth, QuestNode.DefaultNodeHeight);

            // Add success node if specified:
            if (leadsToSuccess) AddSuccessNode(quest, node);

            // Set node's counter condition:
            var condition = ScriptableObjectUtility.CreateScriptableObject<CounterQuestCondition>();
            condition.name = "counterCondition";
            condition.counterIndex = quest.GetCounterIndex(counterName);
            condition.counterValueMode = CounterValueConditionMode.AtLeast;
            condition.requiredCounterValue = new QuestNumber(max);
            node.conditionSet.conditionList.Add(condition);
            AddAndSaveSubasset(condition);

            // Set node's UI content:
            var stateInfo = node.GetStateInfo(QuestNodeState.Active);

            var hudContentList = stateInfo.GetContentList(QuestContentCategory.HUD);
            AddBodyContent(hudContentList, hudInstructionsText);
            AddBodyContent(hudContentList, hudCountText);

            var journalContentList = stateInfo.GetContentList(QuestContentCategory.Journal);
            AddBodyContent(journalContentList, journalText);

            var dialogueContentList = stateInfo.GetContentList(QuestContentCategory.Dialogue);
            AddBodyContent(dialogueContentList, dialogueText);

            // Add alert action:
            if (!string.IsNullOrEmpty(hudInstructionsText))
            {
                var alertAction = ScriptableObjectUtility.CreateScriptableObject<AlertQuestAction>();
                AddBodyContent(alertAction.contentList, hudInstructionsText);
                stateInfo.actionList.Add(alertAction);
                AddAndSaveSubasset(alertAction);
            }

            // Update quest's internal references:
            quest.SetRuntimeReferences();

            // Refresh editor windows:
            QuestEditorWindow.RepaintNow(); // Quest Editor window.
            QuestEditorWindow.RepaintCurrentEditorNow(); // Inspector.
        }

        protected void AddBodyContent(List<QuestContent> contentList, string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                contentList.Add(CreateBodyContent(AddCounterTags(text)));
            }
        }

        protected string AddCounterTags(string text)
        {
            return text.Replace("{0}", "{#" + counterName + "}").Replace("{1}", "{>#" + counterName + "}").Replace("\\n", "\n");
        }

    }
}
