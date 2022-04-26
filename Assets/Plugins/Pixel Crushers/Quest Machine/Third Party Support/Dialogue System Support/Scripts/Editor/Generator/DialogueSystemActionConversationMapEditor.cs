// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace PixelCrushers.QuestMachine.DialogueSystemSupport
{

    /// <summary>
    /// Custom editor for DialogueSystemActionConversationMap that uses
    /// reorderable lists to make editing nicer.
    /// </summary>
    [CustomEditor(typeof(DialogueSystemActionConversationMap), true)]
    public class DialogueSystemActionConversationMapEditor : Editor
    {

        [SerializeField]
        private int m_databaseInstanceID = 0;

        private ReorderableList recordList { get; set; }
        private ReorderableList motiveConversationList { get; set; }
        private int currentActionIndex { get; set; }

        private void OnEnable()
        {
            if (m_databaseInstanceID != 0) DialogueSystem.EditorTools.selectedDatabase = EditorUtility.InstanceIDToObject(m_databaseInstanceID) as DialogueSystem.DialogueDatabase;
            DialogueSystem.EditorTools.SetInitialDatabaseIfNull();
            SetupRecordList();
        }

        public override void OnInspectorGUI()
        {
            //--- For debugging: base.OnInspectorGUI();

            EditorGUILayout.HelpBox("This asset maps Quest Machine actions to Dialogue System conversations. Add actions to the list. Then click on each row in the list to assign Dialogue System conversations.", MessageType.None);
            DrawDialogueDatabase();
            serializedObject.Update();
            recordList.DoLayoutList();
            DrawSelectedRecordListElement();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawDialogueDatabase()
        {
            if (DialogueSystem.EditorTools.selectedDatabase == null)
            {
                EditorGUILayout.HelpBox("Select a dialogue database or open a scene with a Dialogue Manager that has a dialogue database.", MessageType.Warning);
                DialogueSystem.EditorTools.SetInitialDatabaseIfNull();
            }
            DialogueSystem.EditorTools.selectedDatabase = EditorGUILayout.ObjectField("Dialogue Database", DialogueSystem.EditorTools.selectedDatabase, typeof(DialogueSystem.DialogueDatabase), false) as DialogueSystem.DialogueDatabase;
            if (DialogueSystem.EditorTools.selectedDatabase != null)
            {
                m_databaseInstanceID = DialogueSystem.EditorTools.selectedDatabase.GetInstanceID();
            }
        }

        #region Record List

        private void SetupRecordList()
        {
            recordList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_records"), true, true, true, true);
            recordList.drawHeaderCallback += DrawRecordListHeader;
            recordList.drawElementCallback += DrawRecordListElement;
            recordList.onSelectCallback += SelectRecordListElement;
            recordList.onAddCallback += AddRecordListElement;
            recordList.onRemoveCallback += QuestEditorUtility.RemoveReorderableListElementWithoutLeavingNull;
            motiveConversationList = null;
            currentActionIndex = -1;
        }

        private void DrawRecordListHeader(Rect rect)
        {
            EditorGUI.LabelField(new Rect(rect.x + 12, rect.y, rect.width, rect.height), "Actions");
        }

        private void DrawRecordListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var recordProperty = GetRecordProperty(index);
            if (recordProperty == null) return;
            var actionProperty = recordProperty.FindPropertyRelative("m_action");
            var action = actionProperty.objectReferenceValue as PixelCrushers.QuestMachine.Action;
            var summary = string.Empty;
            if (action != null)
            {
                var motiveConversationsProperty = recordProperty.FindPropertyRelative("m_motiveConversations");
                var activeStateConversationProperty = recordProperty.FindPropertyRelative("m_activeStateConversation");
                var turnInConversationProperty = recordProperty.FindPropertyRelative("m_turnInConversation");
                var incomplete = string.IsNullOrEmpty(activeStateConversationProperty.stringValue) || string.IsNullOrEmpty(turnInConversationProperty.stringValue);
                var numMotives = action.motives.Length;
                for (int i = 0; i < motiveConversationsProperty.arraySize; i++)
                {
                    if (string.IsNullOrEmpty(motiveConversationsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("m_conversation").stringValue)) incomplete = true;
                }
                summary = numMotives + ((numMotives == 1) ? " Motive" : " Motives") + (incomplete ? "*" : string.Empty);
            }
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width / 2, rect.height), actionProperty, GUIContent.none, true);
            EditorGUI.LabelField(new Rect(rect.x + rect.width / 2, rect.y, rect.width / 2, rect.height), summary);
        }

        private SerializedProperty GetRecordProperty(int index)
        {
            return (0 <= index && index < recordList.count) ? recordList.serializedProperty.GetArrayElementAtIndex(index) : null;
        }

        private void AddRecordListElement(ReorderableList list)
        {
            ReorderableList.defaultBehaviours.DoAddButton(list);
            var recordProperty = GetRecordProperty(list.index);
            if (recordProperty == null) return;
            var actionProperty = recordProperty.FindPropertyRelative("m_action");
            var motiveConversationsProperty = recordProperty.FindPropertyRelative("m_motiveConversations");
            var activeStateConversationProperty = recordProperty.FindPropertyRelative("m_activeStateConversation");
            actionProperty.objectReferenceValue = null;
            activeStateConversationProperty.stringValue = string.Empty;
            motiveConversationsProperty.arraySize = 0;
        }

        private void SelectRecordListElement(ReorderableList list)
        {
            motiveConversationList = null;
            currentActionIndex = list.index;
        }

        private void DrawSelectedRecordListElement()
        {
            var recordProperty = GetRecordProperty(recordList.index);
            if (recordProperty == null) return;
            var actionProperty = recordProperty.FindPropertyRelative("m_action");
            var action = actionProperty.objectReferenceValue as PixelCrushers.QuestMachine.Action;
            if (action == null) return;
            var motiveConversationsProperty = recordProperty.FindPropertyRelative("m_motiveConversations");
            var activeStateConversationProperty = recordProperty.FindPropertyRelative("m_activeStateConversation");
            var turnInConversationProperty = recordProperty.FindPropertyRelative("m_turnInConversation");
            if (motiveConversationList == null || currentActionIndex != recordList.index) SetupMotiveConversationList(action, motiveConversationsProperty);
            if (motiveConversationList != null) motiveConversationsProperty.arraySize = action.motives.Length;
            EditorGUILayout.LabelField(action.name, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(activeStateConversationProperty);
            EditorGUILayout.PropertyField(turnInConversationProperty);
            if (motiveConversationList != null) motiveConversationList.DoLayoutList();
        }

        #endregion

        #region Motive Conversation List

        private void SetupMotiveConversationList(PixelCrushers.QuestMachine.Action action, SerializedProperty motiveConversationsProperty)
        {
            if (action == null) return;
            if (action.motives == null) return;
            if (motiveConversationsProperty == null) return;
            motiveConversationsProperty.arraySize = action.motives.Length;
            motiveConversationList = new ReorderableList(serializedObject, motiveConversationsProperty, false, true, false, false);
            motiveConversationList.drawHeaderCallback += DrawMotiveConversationListHeader;
            motiveConversationList.drawElementCallback += DrawMotiveConversationListElement;
        }

        private void DrawMotiveConversationListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Motives");
        }

        private void DrawMotiveConversationListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (!(0 <= index && index < motiveConversationList.count)) return;
            var property = motiveConversationList.serializedProperty.GetArrayElementAtIndex(index);
            var conversationProperty = property.FindPropertyRelative("m_conversation");
            var recordProperty = GetRecordProperty(index);
            if (recordProperty == null) return;
            var actionProperty = recordProperty.FindPropertyRelative("m_action");
            var action = actionProperty.objectReferenceValue as PixelCrushers.QuestMachine.Action;
            var motive = action.motives[index];
            var summary = "[" + index + "]";
            for (int i = 0; i < motive.driveValues.Length; i++)
            {
                var driveValue = motive.driveValues[i];
                if (driveValue.drive != null)
                {
                    var driveName = driveValue.drive.name;
                    if (driveName.Length > 6) driveName = driveName.Substring(0, 6) + ".";
                    summary += " " + driveName + ":" + driveValue.value;
                }
            }
            var summaryWidth = rect.width * 0.66f;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.LabelField(new Rect(rect.x, rect.y, summaryWidth, rect.height), summary);
            EditorGUI.EndDisabledGroup();
            EditorGUI.PropertyField(new Rect(rect.x + summaryWidth, rect.y, rect.width - summaryWidth, rect.height), conversationProperty, GUIContent.none, true);
        }

        #endregion

    }
}