// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Wizard that adds a return to quest giver node.
    /// </summary>
    public class QuestEditorReturnWizard : QuestEditorWizard
    {
        protected string hudText = "Return to {QUESTGIVER}";
        protected string journalText = "Return to {QUESTGIVER}.";
        protected string dialogueText = "Thanks for completing the quest.";
        protected bool leadsToSuccess = true;
        protected const string helpText = "This wizard adds a 'return to quest giver' condition node. The fields have been set to example values. Change them and click Add.";
        protected GUIContent hudTextGUIContent = new GUIContent("HUD Text", "Show these instructions in the HUD when this condition node is active. The tag {QUESTGIVER} will be replaced by the quest giver's name.");
        protected GUIContent journalTextGUIContent = new GUIContent("Journal Text", "Show these instructions in the journal when this condition node is active. The tag {QUESTGIVER} will be replaced by the quest giver's name.");
        protected GUIContent dialogueTextGUIContent = new GUIContent("Dialogue Text", "Show this dialogue text when the player returns to the quest giver.");
        protected GUIContent leadsToSuccessGUIContent = new GUIContent("Leads to Success", "Add a success node immediately after this node.");

        public QuestEditorReturnWizard(int clickedIndex) : base(clickedIndex) { }

        public override bool Draw()
        {
            EditorGUILayout.LabelField(string.Empty);
            EditorGUILayout.LabelField("Add Return to Quest Giver Node", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(helpText, MessageType.Info);
            hudText = EditorGUILayout.TextField(hudTextGUIContent, hudText);
            journalText = EditorGUILayout.TextField(journalTextGUIContent, journalText);
            dialogueText = EditorGUILayout.TextField(dialogueTextGUIContent, dialogueText);
            leadsToSuccess = EditorGUILayout.Toggle(leadsToSuccessGUIContent, leadsToSuccess);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(journalText));
            if (GUILayout.Button("Add", GUILayout.Width(80)))
            {
                AddReturnNode();
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

        protected virtual void AddReturnNode()
        {
            var quest = QuestEditorWindow.selectedQuest;
            if (quest == null) return;

            // Add a new node:
            var parentNode = GetParentNode(quest);
            if (parentNode == null) return;
            var node = new QuestNode(new StringField(journalText), new StringField(), QuestNodeType.Condition);
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
            condition.message = new StringField(QuestMachineMessages.DiscussQuestMessage);
            condition.parameter = new StringField(quest.id);
            condition.value = new MessageValue();
            condition.targetSpecifier = QuestMessageParticipant.QuestGiver;
            node.conditionSet.conditionList.Add(condition);
            AddAndSaveSubasset(condition);

            // Set node's UI content:
            var stateInfo = node.GetStateInfo(QuestNodeState.Active);

            var hudContentList = stateInfo.GetContentList(QuestContentCategory.HUD);
            hudContentList.Add(CreateBodyContent(hudText));

            var journalContentList = stateInfo.GetContentList(QuestContentCategory.Journal);
            journalContentList.Add(CreateBodyContent(journalText));

            // Add dialogue content to true state, since node becomes true as soon as player talks:
            stateInfo = node.GetStateInfo(QuestNodeState.True);
            var dialogueContentList = stateInfo.GetContentList(QuestContentCategory.Dialogue);
            dialogueContentList.Add(CreateBodyContent(dialogueText));

            // Add alert action to show when player talks to quest giver:
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

