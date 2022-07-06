// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace PixelCrushers.QuestMachine
{

    [CustomEditor(typeof(Action), true)]
    public class ActionEditor : Editor
    {

        private ReorderableList m_requirementsList = null;
        private ReorderableList m_effectsList = null;

        private static bool s_activeTextFoldout = true;
        private static bool s_completedTextFoldout = true;

        private void OnEnable()
        {
            SetupRequirementsList();
            SetupEffectsList();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Actions are tasks that quest givers can ask the player to do to an entity. However, they don't make changes to the " +
                "world. Instead, they represent changes that would occur in the world if the player were to complete the action. Actions should match up with " +
                "behaviors on GameObjects that make actual changes to the world. For example, an Attack action represents removing the entity from the world. " +
                "The actual behavior to do this should be on the entity itself, such as a Health behavior that despawns the GameObject.", MessageType.None);
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_description"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_displayName"));
            DrawMotives();
            QuestEditorUtility.EditorGUILayoutVerticalSpace(2);
            DrawText();
            QuestEditorUtility.EditorGUILayoutVerticalSpace(2);
            DrawRequirements();
            QuestEditorUtility.EditorGUILayoutVerticalSpace(2);
            DrawEffects();
            QuestEditorUtility.EditorGUILayoutVerticalSpace(2);
            DrawCompletionConditions();
            serializedObject.ApplyModifiedProperties();
        }

        #region Text & Motives

        private void DrawText()
        {
            QuestEditorPrefs.actionTextFoldout = QuestEditorUtility.EditorGUILayoutFoldout("Task Text", "The text shown to the player when this action is used in a quest.", QuestEditorPrefs.actionTextFoldout);
            if (!QuestEditorPrefs.actionTextFoldout) return;
            try
            {
                QuestEditorUtility.EditorGUILayoutBeginGroup();
                EditorGUILayout.HelpBox("The quest giver uses Task Text to ask the player to do this action. If you include words in braces like {this} they will be substituted by the quest giver's dialect table. Other valid tags:\n{TARGETDESCRIPTOR}: Action's target.\n{DOMAIN}: Target's domain.\n{#COUNTERNAME}: Counter name for task completion.\n{#COUNTERGOAL}: Goal value for counter.", MessageType.None);
                var actionTextProperty = serializedObject.FindProperty("m_actionText");
                if (actionTextProperty == null) return;
                EditorGUI.indentLevel++;
                s_activeTextFoldout = QuestEditorUtility.EditorGUILayoutFoldout("Active", "Show this text when the quest is active. Optionally specify message to send when node becomes active; use ':' to separate message and parameter.", s_activeTextFoldout, false);
                if (s_activeTextFoldout)
                {
                    EditorGUILayout.PropertyField(actionTextProperty.FindPropertyRelative("m_activeText"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_sendMessageOnActive"));
                }
                s_completedTextFoldout = QuestEditorUtility.EditorGUILayoutFoldout("Completed", "Show this text when the quest is completed. Optionally specify message to send when node is completed; use ':' to separate message and parameter.", s_completedTextFoldout, false);
                if (s_completedTextFoldout)
                {
                    EditorGUILayout.PropertyField(actionTextProperty.FindPropertyRelative("m_completedText"));
                    EditorGUILayout.PropertyField(actionTextProperty.FindPropertyRelative("m_successText"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_sendMessageOnCompletion"));
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
            finally
            {
                QuestEditorUtility.EditorGUILayoutEndGroup();
            }
        }

        private void DrawMotives()
        {
            QuestEditorPrefs.actionMotivesFoldout = QuestEditorUtility.EditorGUILayoutFoldout("Motives", "Choices of motive text shown to the player when this action is the primary goal of a quest, and the drive values that help quest givers choose which motives fit them best.", QuestEditorPrefs.actionMotivesFoldout);
            if (!QuestEditorPrefs.actionMotivesFoldout) return;
            try
            {
                QuestEditorUtility.EditorGUILayoutBeginGroup();
                EditorGUILayout.HelpBox("The quest giver uses Motive Text if this is the primary action of the quest. If you include words in braces like {this} they will be substituted by the quest giver's dialect table.", MessageType.None);
                var motivesProperty = serializedObject.FindProperty("m_motives");
                if (motivesProperty == null) return;
                int indexToDelete = -1;
                bool addNewMotive = false;
                for (int i = 0; i < motivesProperty.arraySize; i++)
                {
                    var motiveProperty = motivesProperty.GetArrayElementAtIndex(i);
                    if (motiveProperty == null) continue;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Motive:", EditorStyles.boldLabel);
                    GUILayout.FlexibleSpace();
                    var isLastMotive = (i == motivesProperty.arraySize - 1);
                    if (isLastMotive)
                    {
                        if (GUILayout.Button("-", EditorStyles.miniButtonLeft, GUILayout.Width(30))) indexToDelete = i;
                        if (i == motivesProperty.arraySize - 1 && GUILayout.Button("+", EditorStyles.miniButtonRight, GUILayout.Width(30))) addNewMotive = true;
                    }
                    else
                    {
                        if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.Width(30))) indexToDelete = i;
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(motiveProperty, GUIContent.none, true);
                    EditorGUILayout.EndHorizontal();
                }
                if (motivesProperty.arraySize == 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Add Motive", GUILayout.Width(80))) addNewMotive = true;
                    EditorGUILayout.EndHorizontal();
                }
                if (addNewMotive) motivesProperty.arraySize++;
                if (indexToDelete != -1) motivesProperty.DeleteArrayElementAtIndex(indexToDelete);
                EditorGUILayout.Space();
            }
            finally
            {
                QuestEditorUtility.EditorGUILayoutEndGroup();
            }
        }

        #endregion

        #region Requirements

        private void SetupRequirementsList()
        {
            m_requirementsList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_requirements"), true, true, true, true);
            m_requirementsList.drawHeaderCallback += OnDrawRequirementsListHeader;
            m_requirementsList.drawElementCallback += OnDrawRequirementsListElement;
            m_requirementsList.onAddCallback += OnAddRequirementsListElement;
        }

        private void DrawRequirements()
        {
            QuestEditorPrefs.actionRequirementsFoldout = QuestEditorUtility.EditorGUILayoutFoldout("Requirements", "Requirements are the world states that must already exist before this action can be undertaken.", QuestEditorPrefs.actionRequirementsFoldout);
            if (!QuestEditorPrefs.actionRequirementsFoldout) return;
            try
            {
                QuestEditorUtility.EditorGUILayoutBeginGroup();
                if (m_requirementsList.count == 0)
                {
                    EditorGUILayout.HelpBox("World states that must already exist before this action can be undertaken. If any specific world states are required before this action can be done, add them below.", MessageType.None);
                }
                else
                {
                    EditorGUILayout.HelpBox("World states that must already exist before this action can be undertaken.", MessageType.None);
                }
                m_requirementsList.DoLayoutList();
                EditorGUILayout.Space();
            }
            finally
            {
                QuestEditorUtility.EditorGUILayoutEndGroup();
            }
        }

        private void OnDrawRequirementsListHeader(Rect rect)
        {
            var notWidth = 22;
            var countWidth = 36;
            var funcWidth = 48;
            var fieldWidth = (rect.width - notWidth - 2 * countWidth - funcWidth - 12) / 2;
            EditorGUI.LabelField(new Rect(12 + rect.x, rect.y, notWidth, rect.height), "Not");
            EditorGUI.LabelField(new Rect(12 + rect.x + notWidth, rect.y, fieldWidth, rect.height), "Entity");
            EditorGUI.LabelField(new Rect(12 + rect.x + notWidth + fieldWidth, rect.y, fieldWidth, rect.height), "Domain");
            EditorGUI.LabelField(new Rect(12 + rect.x + notWidth + 2 * fieldWidth, rect.y, countWidth, rect.height), "Min");
            EditorGUI.LabelField(new Rect(12 + rect.x + notWidth + 2 * fieldWidth + countWidth, rect.y, countWidth, rect.height), "Max");
            EditorGUI.LabelField(new Rect(12 + rect.x + notWidth + 2 * fieldWidth + 2 * countWidth, rect.y, funcWidth, rect.height), "Func");
        }

        private void OnDrawRequirementsListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var arrayProperty = serializedObject.FindProperty("m_requirements");
            if (arrayProperty == null || !(0 <= index && index < arrayProperty.arraySize)) return;
            var elementProperty = arrayProperty.GetArrayElementAtIndex(index);
            if (elementProperty == null) return;
            var notProperty = elementProperty.FindPropertyRelative("m_not");
            var domainSpecifierProperty = elementProperty.FindPropertyRelative("m_domainSpecifier");
            var entitySpecifierProperty = elementProperty.FindPropertyRelative("m_entitySpecifier");
            var minProperty = elementProperty.FindPropertyRelative("m_min");
            var maxProperty = elementProperty.FindPropertyRelative("m_max");
            var funcProperty = elementProperty.FindPropertyRelative("m_requirementFunction");
            if (notProperty == null || domainSpecifierProperty == null || entitySpecifierProperty == null || minProperty == null || maxProperty == null) return;
            var notWidth = 22;
            var countWidth = 36;
            var funcWidth = 48;
            var fieldWidth = (rect.width - notWidth - 2 * countWidth - funcWidth) / 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, notWidth, rect.height), notProperty, GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + notWidth, rect.y, fieldWidth, rect.height), entitySpecifierProperty, GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + notWidth + fieldWidth, rect.y, fieldWidth, rect.height), domainSpecifierProperty, GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + notWidth + 2 * fieldWidth, rect.y, countWidth, EditorGUIUtility.singleLineHeight), minProperty, GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + notWidth + 2 * fieldWidth + countWidth, rect.y, countWidth, EditorGUIUtility.singleLineHeight), maxProperty, GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + notWidth + 2 * fieldWidth + 2 * countWidth, rect.y, funcWidth, EditorGUIUtility.singleLineHeight), funcProperty, GUIContent.none);
        }

        private void OnAddRequirementsListElement(ReorderableList list)
        {
            m_requirementsList.serializedProperty.arraySize++;
            m_requirementsList.index = m_requirementsList.serializedProperty.arraySize - 1;
            var property = m_requirementsList.serializedProperty.GetArrayElementAtIndex(m_requirementsList.serializedProperty.arraySize - 1);
            property.FindPropertyRelative("m_min").intValue = 1;
            property.FindPropertyRelative("m_max").intValue = 65535;
        }

        #endregion

        #region Effects

        private void SetupEffectsList()
        {
            m_effectsList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_effects"), true, true, true, true);
            m_effectsList.drawHeaderCallback += OnDrawEffectsListHeader;
            m_effectsList.drawElementCallback += OnDrawEffectsListElement;
        }

        private void DrawEffects()
        {
            QuestEditorPrefs.actionEffectsFoldout = QuestEditorUtility.EditorGUILayoutFoldout("Effects", "Effects are the changes to the world state that this action causes.", QuestEditorPrefs.actionEffectsFoldout);
            if (!QuestEditorPrefs.actionEffectsFoldout) return;
            try
            {
                QuestEditorUtility.EditorGUILayoutBeginGroup();
                if (m_effectsList.count == 0)
                {
                    EditorGUILayout.HelpBox("Changes to the world state that this action causes. If you want this action to represent a change in the world state, add at least one action.", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("Changes to the world state that this action causes. ", MessageType.None);
                }
                m_effectsList.DoLayoutList();
                EditorGUILayout.Space();
            }
            finally
            {
                QuestEditorUtility.EditorGUILayoutEndGroup();
            }
        }

        private void OnDrawEffectsListHeader(Rect rect)
        {
            var operationWidth = 60;
            var countWidth = 40;
            var fieldWidth = (rect.width - operationWidth - countWidth - 12) / 2;
            EditorGUI.LabelField(new Rect(12 + rect.x, rect.y, operationWidth, rect.height), "Operation");
            EditorGUI.LabelField(new Rect(12 + rect.x + operationWidth, rect.y, fieldWidth, rect.height), "Entity");
            EditorGUI.LabelField(new Rect(12 + rect.x + operationWidth + fieldWidth, rect.y, fieldWidth, rect.height), "Domain");
            EditorGUI.LabelField(new Rect(12 + rect.x + operationWidth + 2 * fieldWidth, rect.y, countWidth, rect.height), "Count");
        }

        private void OnDrawEffectsListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var arrayProperty = serializedObject.FindProperty("m_effects");
            if (arrayProperty == null || !(0 <= index && index < arrayProperty.arraySize)) return;
            var elementProperty = arrayProperty.GetArrayElementAtIndex(index);
            if (elementProperty == null) return;
            var operationProperty = elementProperty.FindPropertyRelative("m_operation");
            var domainSpecifierProperty = elementProperty.FindPropertyRelative("m_domainSpecifier");
            var entitySpecifierProperty = elementProperty.FindPropertyRelative("m_entitySpecifier");
            var countProperty = elementProperty.FindPropertyRelative("m_count");
            if (operationProperty == null || domainSpecifierProperty == null || entitySpecifierProperty == null || countProperty == null) return;
            var operationWidth = 60;
            var countWidth = 40;
            var fieldWidth = (rect.width - operationWidth - countWidth) / 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, operationWidth, rect.height), operationProperty, GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + operationWidth, rect.y, fieldWidth, rect.height), entitySpecifierProperty, GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + operationWidth + fieldWidth, rect.y, fieldWidth, rect.height), domainSpecifierProperty, GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + operationWidth + 2 * fieldWidth, rect.y, countWidth, rect.height), countProperty, GUIContent.none);
        }

        #endregion

        #region Completion

        private void DrawCompletionConditions()
        {
            QuestEditorPrefs.actionCompletionConditionsFoldout = QuestEditorUtility.EditorGUILayoutFoldout("Completion Conditions", "The action is considered complete when these conditions are met.", QuestEditorPrefs.actionCompletionConditionsFoldout);
            if (!QuestEditorPrefs.actionCompletionConditionsFoldout) return;
            try
            {
                QuestEditorUtility.EditorGUILayoutBeginGroup();
                EditorGUILayout.HelpBox("The action is considered complete when these conditions are met.", MessageType.None);
                var completionProperty = serializedObject.FindProperty("m_completion");
                if (completionProperty == null) return;
                var modeProperty = completionProperty.FindPropertyRelative("m_mode");
                EditorGUILayout.PropertyField(modeProperty);
                if (modeProperty.enumValueIndex == (int)ActionCompletion.Mode.Message)
                {
                    EditorGUILayout.PropertyField(completionProperty.FindPropertyRelative("m_senderID"), new GUIContent("Sender ID", "Required message sender ID, or any sender if blank. Can also be {QUESTERID} or {QUESTGIVERID}. Sender must have a Quest Giver or Entity component."), true);
                    EditorGUILayout.PropertyField(completionProperty.FindPropertyRelative("m_targetID"), new GUIContent("Target ID", "Required message target ID, or any target if blank. Can also be {QUESTERID} or {QUESTGIVERID}. Target must have a Quest Giver or Entity component."), true);
                    EditorGUILayout.PropertyField(completionProperty.FindPropertyRelative("m_message"), true);
                    EditorGUILayout.PropertyField(completionProperty.FindPropertyRelative("m_parameter"), true);
                }
                else
                {
                    EditorGUILayout.PropertyField(completionProperty.FindPropertyRelative("m_baseCounterName"), new GUIContent("Base Counter Name", "Counter name. Entity name will be prefixed to counter name."), true);
                    EditorGUILayout.PropertyField(completionProperty.FindPropertyRelative("m_initialValue"), true);
                    EditorGUILayout.PropertyField(completionProperty.FindPropertyRelative("m_minValue"), true);
                    EditorGUILayout.PropertyField(completionProperty.FindPropertyRelative("m_maxValue"), true);
                    EditorGUILayout.PropertyField(completionProperty.FindPropertyRelative("m_counterValueMode"), true);
                    EditorGUILayout.PropertyField(completionProperty.FindPropertyRelative("m_requiredValue"), true);
                    var updateModeProperty = completionProperty.FindPropertyRelative("m_updateMode");
                    EditorGUILayout.PropertyField(updateModeProperty, new GUIContent("Update Mode"), true);
                    if (updateModeProperty.enumValueIndex == (int)QuestCounterUpdateMode.Messages)
                    {
                        EditorGUI.indentLevel++;
                        int indexToDelete = -1;
                        var messageEventListProperty = completionProperty.FindPropertyRelative("m_messageEventList");
                        UnityEngine.Assertions.Assert.IsNotNull(messageEventListProperty, "Quest Machine: Internal error - m_messageEventList is null.");
                        if (messageEventListProperty == null) return;
                        for (int i = 0; i < messageEventListProperty.arraySize; i++)
                        {
                            var messageEventProperty = messageEventListProperty.GetArrayElementAtIndex(i);
                            var senderProperty = messageEventProperty.FindPropertyRelative("m_senderID");
                            var targetProperty = completionProperty.FindPropertyRelative("m_targetID");
                            var messageProperty = messageEventProperty.FindPropertyRelative("m_message");
                            var parameterProperty = messageEventProperty.FindPropertyRelative("m_parameter");
                            var operationProperty = messageEventProperty.FindPropertyRelative("m_operation");
                            var literalValueProperty = messageEventProperty.FindPropertyRelative("m_literalValue");
                            if (senderProperty == null || targetProperty == null || messageProperty == null || parameterProperty == null ||
                                operationProperty == null || literalValueProperty == null) return;
                            EditorGUILayout.PropertyField(senderProperty,
                                new GUIContent("Sender ID", "Required message sender ID, or any sender if blank. Can also be {QUESTERID} or {QUESTGIVERID}. Sender must have a Quest Giver or Entity component."), true);
                            EditorGUILayout.PropertyField(targetProperty,
                                new GUIContent("Target ID", "Required message target ID, or any target if blank. Can also be {QUESTERID} or {QUESTGIVERID}. Target must have a Quest Giver or Entity component."), true);
                            EditorGUILayout.PropertyField(messageProperty,
                                new GUIContent("Message", "When this message and the parameter below are sent, update the counter according to the operation below."), true);
                            EditorGUILayout.PropertyField(parameterProperty,
                                new GUIContent("Parameter", "When the message above and this parameter are sent, update the counter according to the operation below. You can use the tags {TARGETENTITY} and {DOMAIN}."), true);
                            EditorGUILayout.BeginHorizontal();
                            var operation = (QuestCounterMessageEvent.Operation)operationProperty.enumValueIndex;
                            if (operation == QuestCounterMessageEvent.Operation.ModifyByMessageValue || operation == QuestCounterMessageEvent.Operation.SetToMessageValue)
                            {
                                EditorGUILayout.PropertyField(operationProperty, GUIContent.none);
                            }
                            else
                            {
                                EditorGUILayout.PropertyField(operationProperty, GUIContent.none);
                                EditorGUILayout.PropertyField(literalValueProperty, GUIContent.none);
                            }
                            if (GUILayout.Button(new GUIContent("-", "Delete this message event.")))
                            {
                                indexToDelete = i;
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        if (indexToDelete != -1) messageEventListProperty.DeleteArrayElementAtIndex(indexToDelete);
                        if (GUILayout.Button(new GUIContent("Add Message", "Add a new message event for the counter."), EditorStyles.miniButton))
                        {
                            messageEventListProperty.arraySize++;
                            var messageEventProperty = messageEventListProperty.GetArrayElementAtIndex(messageEventListProperty.arraySize - 1);
                            messageEventProperty.FindPropertyRelative("m_message").FindPropertyRelative("m_text").stringValue = string.Empty;
                            messageEventProperty.FindPropertyRelative("m_parameter").FindPropertyRelative("m_text").stringValue = string.Empty;
                            messageEventProperty.FindPropertyRelative("m_senderID").FindPropertyRelative("m_text").stringValue = string.Empty;
                            messageEventProperty.FindPropertyRelative("m_targetID").FindPropertyRelative("m_text").stringValue = string.Empty;
                            messageEventProperty.FindPropertyRelative("m_operation").enumValueIndex = (int)QuestCounterMessageEvent.Operation.ModifyByLiteralValue;
                            messageEventProperty.FindPropertyRelative("m_literalValue").intValue = 1;
                        }
                    }
                    EditorGUI.indentLevel--;
                }
            }
            finally
            {
                QuestEditorUtility.EditorGUILayoutEndGroup();
            }
        }

        #endregion

    }

}