// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Wizard that adds a message condition node.
    /// </summary>
    public class QuestEditorMessageWizard : QuestEditorWizard
    {
        protected string message = "Explored";
        protected string parameter = "Cave";
        protected string hudText = "Explore Cave";
        protected string journalText = "{QUESTGIVER} has asked you to explore the Cave.";
        protected string dialogueText = "I want you to explore the Cave.";
        protected bool leadsToSuccess = false;
        protected const string helpText = "This wizard adds a condition node that listens for a message from the message system. The fields have been set to example values. Change them and click Add.";
        protected GUIContent messageGUIContent = new GUIContent("Message", "Message that this node listens for. Send it using Quest Machine's message system.");
        protected GUIContent parameterGUIContent = new GUIContent("Parameter", "If specified, require this parameter with the message. If blank, allow any parameter.");
        protected GUIContent hudTextGUIContent = new GUIContent("HUD Text", "Show these instructions to the player in the HUD when this condition node is active.");
        protected GUIContent journalTextGUIContent = new GUIContent("Journal Text", "Show these instructions to the player in the journal when this condition node is active.");
        protected GUIContent dialogueTextGUIContent = new GUIContent("Dialogue Text", "Show this dialogue content when this condition node is active.");
        protected GUIContent leadsToSuccessGUIContent = new GUIContent("Leads to Success", "Add a success node immediately after this node.");

        public QuestEditorMessageWizard(int clickedIndex) : base(clickedIndex) { }

        public override bool Draw()
        {
            EditorGUILayout.LabelField(string.Empty);
            EditorGUILayout.LabelField("Add Message Node", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(helpText, MessageType.Info);
            message = EditorGUILayout.TextField(messageGUIContent, message);
            parameter = EditorGUILayout.TextField(parameterGUIContent, parameter);
            hudText = EditorGUILayout.TextField(hudTextGUIContent, hudText);
            journalText = EditorGUILayout.TextField(journalTextGUIContent, journalText);
            dialogueText = EditorGUILayout.TextField(dialogueTextGUIContent, dialogueText);
            leadsToSuccess = EditorGUILayout.Toggle(leadsToSuccessGUIContent, leadsToSuccess);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(message));
            if (GUILayout.Button("Add", GUILayout.Width(80)))
            {
                AddMessageNode();
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

        protected virtual void AddMessageNode()
        {
            var quest = QuestEditorWindow.selectedQuest;
            if (quest == null) return;

            // Add a new node:
            var parentNode = GetParentNode(quest);
            if (parentNode == null) return;
            var node = new QuestNode(new StringField(hudText), new StringField(), QuestNodeType.Condition);
            quest.nodeList.Add(node);
            node.childIndexList.AddRange(parentNode.childIndexList);
            parentNode.childIndexList.Clear();
            parentNode.childIndexList.Add(quest.nodeList.Count - 1);
            node.canvasRect = new Rect(parentNode.canvasRect.x + QuestNode.DefaultNodeWidth + 20, parentNode.canvasRect.y, QuestNode.DefaultNodeWidth, QuestNode.DefaultNodeHeight);

            // Add success node if specified:
            if (leadsToSuccess) AddSuccessNode(quest, node);

            // Set node's counter condition:
            var condition = ScriptableObjectUtility.CreateScriptableObject<MessageQuestCondition>();
            condition.name = "messageCondition";
            condition.message = new StringField(message);
            condition.parameter = new StringField(parameter);
            condition.value = new MessageValue();
            node.conditionSet.conditionList.Add(condition);
            AddAndSaveSubasset(condition);

            // Set node's UI content:
            var stateInfo = node.GetStateInfo(QuestNodeState.Active);

            var hudContentList = stateInfo.GetContentList(QuestContentCategory.HUD);
            hudContentList.Add(CreateBodyContent(hudText));

            var journalContentList = stateInfo.GetContentList(QuestContentCategory.Journal);
            journalContentList.Add(CreateBodyContent(journalText));

            var dialogueContentList = stateInfo.GetContentList(QuestContentCategory.Dialogue);
            dialogueContentList.Add(CreateBodyContent(dialogueText));

            // Add alert action:
            var alertAction = ScriptableObjectUtility.CreateScriptableObject<AlertQuestAction>();
            alertAction.contentList.Add(CreateBodyContent(hudText));
            stateInfo.actionList.Add(alertAction);
            AddAndSaveSubasset(alertAction);

            // Update quest's internal references:
            quest.SetRuntimeReferences();

            // Refresh editor windows:
            QuestEditorWindow.RepaintNow(); // Quest Editor window.
            QuestEditorWindow.RepaintCurrentEditorNow(); // Inspector.
        }
    }
}
