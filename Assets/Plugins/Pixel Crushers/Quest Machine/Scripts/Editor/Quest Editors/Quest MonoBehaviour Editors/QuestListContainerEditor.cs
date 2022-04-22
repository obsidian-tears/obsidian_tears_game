// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Custom inspector for QuestListContainer.
    /// </summary>
    [CustomEditor(typeof(QuestListContainer), true)]
    public class QuestListContainerEditor : Editor
    {

        private static bool questListFoldout = true;

        private ReorderableList questReorderableList { get; set; }
        private QuestInspectorGUI inspectorGUI { get; set; }
        private SerializedObject questSerializedObject { get; set; }

        private bool removedQuest = false;

        protected virtual void OnEnable()
        {
            Undo.undoRedoPerformed += RepaintEditorWindow;
            QuestEditorWindow.currentEditor = this;
            questSerializedObject = null;
            inspectorGUI = new QuestInspectorGUI();
            var notPlaying = !Application.isPlaying;
            var questListProperty = serializedObject.FindProperty("m_questList");
            if (questListProperty == null) return;
            questReorderableList = new ReorderableList(serializedObject, questListProperty, notPlaying, true, notPlaying, notPlaying);
            questReorderableList.drawHeaderCallback = OnDrawHeader;
            questReorderableList.onSelectCallback = OnChangeSelection;
            questReorderableList.onAddCallback = OnAddElement;
            questReorderableList.onRemoveCallback = OnRemoveElement;
            if (notPlaying)
            {
                questReorderableList.drawElementCallback = OnDrawEditableElement;
                questReorderableList.onReorderCallback = OnReorder;
            }
            else
            {
                questReorderableList.drawElementCallback = OnDrawRuntimeElement;
            }
        }

        protected virtual void OnDisable()
        {
            Undo.undoRedoPerformed -= RepaintEditorWindow;
            QuestEditorWindow.currentEditor = null;
        }

        protected void RepaintEditorWindow()
        {
            QuestEditorWindow.RepaintNow();
        }

        public override void OnInspectorGUI()
        {
            DrawDebugFoldout();

            serializedObject.Update();
            DrawSaveSettings();
            DrawOtherSettings();
            DrawQuestList();
            serializedObject.ApplyModifiedProperties();
            DrawSelectedQuestInspector();
        }

        protected void DrawDebugFoldout()
        {
#if DEBUG_QUEST_EDITOR
            var key = "PixelCrushers.QuestMachine.EditorPrefsDebug.DefaultInspectorFoldout";
            var foldout = EditorPrefs.GetBool(key);
            var newFoldout = EditorGUILayout.Foldout(foldout, "Default Inspector");
            if (newFoldout != foldout) EditorPrefs.SetBool(key, newFoldout);
            if (newFoldout) base.OnInspectorGUI();
#endif
        }

        protected void DrawID(string tooltip) // Only used for specific subclasses.
        {
            QuestEditorPrefs.questGiverIDFoldout = QuestEditorUtility.EditorGUILayoutFoldout("ID", tooltip, QuestEditorPrefs.questGiverIDFoldout);
            if (!QuestEditorPrefs.questGiverIDFoldout) return;

            try
            {
                QuestEditorUtility.EditorGUILayoutBeginGroup();
                var idProperty = serializedObject.FindProperty("m_id");
                var displayNameProperty = serializedObject.FindProperty("m_displayName");
                var imageProperty = serializedObject.FindProperty("m_image");
                var textTableProperty = serializedObject.FindProperty("m_textTable");
                if (idProperty != null) EditorGUILayout.PropertyField(idProperty);
                if (displayNameProperty != null) EditorGUILayout.PropertyField(displayNameProperty);
                if (imageProperty != null) EditorGUILayout.PropertyField(imageProperty);
                if (textTableProperty != null) EditorGUILayout.PropertyField(textTableProperty);
            }
            finally
            {
                QuestEditorUtility.EditorGUILayoutEndGroup();
            }
        }

        protected virtual void DrawSaveSettings()
        {
            QuestEditorPrefs.questListContainerSaveSettingsFoldout = QuestEditorUtility.EditorGUILayoutFoldout("Save Settings", "What to include in saved games.", QuestEditorPrefs.questListContainerSaveSettingsFoldout);
            if (!QuestEditorPrefs.questListContainerSaveSettingsFoldout) return;

            try
            {
                QuestEditorUtility.EditorGUILayoutBeginGroup();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_includeInSavedGameData"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_saveAcrossSceneChanges"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_addNewQuestsSinceSavedGame"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_key"), new GUIContent("Save Key"));
            }
            finally
            {
                QuestEditorUtility.EditorGUILayoutEndGroup();
            }
        }

        protected virtual void DrawOtherSettings()
        {
            QuestEditorPrefs.questListContainerOtherSettingsFoldout = QuestEditorUtility.EditorGUILayoutFoldout("Other Settings", "Miscellaneous settings.", QuestEditorPrefs.questListContainerOtherSettingsFoldout);
            if (!QuestEditorPrefs.questListContainerOtherSettingsFoldout) return;

            try
            {
                QuestEditorUtility.EditorGUILayoutBeginGroup();
                DrawOtherSettingsInterior();
            }
            finally
            {
                QuestEditorUtility.EditorGUILayoutEndGroup();
            }
        }

        protected virtual void DrawOtherSettingsInterior()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_forwardEventsToListeners"));
        }

        protected virtual void DrawQuestList()
        {
            //---Changed: Don't auto-focus first quest if no quest is selected:
            //if (!(0 <= questReorderableList.index && questReorderableList.index <= questReorderableList.count))
            //{
            //    questReorderableList.index = 0;
            //    SetQuestInEditorWindow(questReorderableList.index);
            //}

            questListFoldout = QuestEditorUtility.EditorGUILayoutFoldout("Quests", "Quests on this GameObject.", questListFoldout);
            if (!questListFoldout) return;

            try
            {
                QuestEditorUtility.EditorGUILayoutBeginGroup();
                removedQuest = false;
                questReorderableList.DoLayoutList();
            }
            finally
            {
                if (!removedQuest) QuestEditorUtility.EditorGUILayoutEndGroup();
                removedQuest = false;
            }
        }

        private void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Quests");
        }

        private void OnDrawEditableElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (!(0 <= index && index < questReorderableList.serializedProperty.arraySize)) return;
            var buttonWidth = 48f;
            var questRect = new Rect(rect.x, rect.y + 1, rect.width - buttonWidth - 2, EditorGUIUtility.singleLineHeight);
            var buttonRect = new Rect(rect.x + rect.width - buttonWidth, rect.y + 1, buttonWidth, EditorGUIUtility.singleLineHeight);
            var questProperty = questReorderableList.serializedProperty.GetArrayElementAtIndex(index);
            var isQuestAssigned = questProperty.objectReferenceValue != null;
            var buttonGUIContent = isQuestAssigned ? new GUIContent("Edit", "Edit in Quest Editor window.") : new GUIContent("New", "Create new quest in this slot.");
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(questRect, questProperty, GUIContent.none, false);
            if (EditorGUI.EndChangeCheck()) SetQuestInEditorWindow(index);
            if (GUI.Button(buttonRect, buttonGUIContent))
            {
                if (!isQuestAssigned)
                {
                    questProperty.objectReferenceValue = QuestEditorAssetUtility.CreateNewQuestAssetFromDialog();
                }
                QuestEditorWindow.ShowWindow();
                SetQuestInEditorWindow(index);
                questReorderableList.index = index;
            }
        }

        private void OnDrawRuntimeElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (!(0 <= index && index < questReorderableList.serializedProperty.arraySize)) return;
            var questProperty = questReorderableList.serializedProperty.GetArrayElementAtIndex(index);
            var quest = questProperty.objectReferenceValue as Quest;
            if (quest == null) return;
            var questName = !StringField.IsNullOrEmpty(quest.title) ? quest.title.value : string.Empty;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.TextField(new Rect(rect.x, rect.y + 1, rect.width, EditorGUIUtility.singleLineHeight), questName);
            EditorGUI.EndDisabledGroup();
        }

        private void OnReorder(ReorderableList list)
        {
            if (QuestEditorWindow.instance == null) return;
            SetQuestInEditorWindow(list.index);
        }

        private void OnChangeSelection(ReorderableList list)
        {
            SetQuestInEditorWindow(list.index);
        }

        private void OnRemoveElement(ReorderableList list)
        {
            var element = (0 <= list.index && list.index < list.count) ? list.serializedProperty.GetArrayElementAtIndex(list.index) : null;
            var quest = (element != null) ? (element.objectReferenceValue as Quest) : null;
            var permanentlyDelete = false;
            if (quest != null)
            {
                var option = EditorUtility.DisplayDialogComplex("Remove Quest",
                    "Do you want to remove the quest '" + quest.title.value + "' from this list or permanently delete the quest from your whole project?",
                    "Remove From List", "Permanently Delete", "Cancel");
                if (option == 2) return; // Cancel.
                removedQuest = true;
                permanentlyDelete = (option == 1);
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
                if (permanentlyDelete)
                {
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(quest));
                }
            }
#if !UNITY_2020_1_OR_NEWER
            // Bug in older Unity versions left empty element that also needs to be removed:
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
#endif
            OnChangeSelection(list);
        }

        private void OnAddElement(ReorderableList list)
        {
            ReorderableList.defaultBehaviours.DoAddButton(list);
            var newIndex = list.serializedProperty.arraySize - 1;
            var element = list.serializedProperty.GetArrayElementAtIndex(newIndex);
            if (element != null) element.objectReferenceValue = null;
        }

        private void SetQuestInEditorWindow(int questListIndex)
        {
            if (!QuestEditorWindow.isOpen) return;
            serializedObject.ApplyModifiedProperties();
            var questListContainer = target as QuestListContainer;
            if (questListContainer == null) return;
            if (!(0 <= questListIndex && questListIndex < questListContainer.questList.Count))
            {
                QuestEditorWindow.instance.SelectQuest(questListContainer, -1);
                questSerializedObject = null;
                Repaint();
                return;
            }
            QuestEditorWindow.ShowWindow();
            QuestEditorWindow.instance.SelectQuest(questListContainer, questListIndex);
            var quest = questListContainer.questList[questListIndex];
            questSerializedObject = (quest != null) ? new SerializedObject(quest) : null;
        }

        protected void DrawSelectedQuestInspector()
        {
            if (questSerializedObject == null || QuestEditorWindow.instance == null) return;
            if (questSerializedObject.targetObject == null)
            {
                QuestEditorWindow.instance.SelectQuest(target as QuestListContainer, -1);
                questSerializedObject = null;
                return;
            }
            questSerializedObject.Update();
            inspectorGUI.Draw(questSerializedObject);
            questSerializedObject.ApplyModifiedProperties();
            if (GUI.changed) RepaintEditorWindow();
        }

    }
}
