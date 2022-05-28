// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Custom inspector for QuestDatabase.
    /// </summary>
    [CustomEditor(typeof(QuestDatabase), true)]
    public class QuestDatabaseEditor : Editor
    {

        private ReorderableList questReorderableList { get; set; }
        private string entityTypeFolderPath = string.Empty;
        private bool showQuestRelations = false;

        protected virtual void OnEnable()
        {
            Undo.undoRedoPerformed += RepaintEditorWindow;
            QuestEditorWindow.currentEditor = this;
            questReorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_questAssets"), true, true, true, true);
            questReorderableList.drawHeaderCallback = OnDrawHeader;
            questReorderableList.onSelectCallback = OnChangeSelection;
            questReorderableList.onAddCallback = OnAddElement;
            questReorderableList.onRemoveCallback = OnRemoveElement;
            questReorderableList.drawElementCallback = OnDrawElement;
            questReorderableList.onReorderCallback = OnReorder;
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
#if DEBUG_QUEST_EDITOR
            var key = "PixelCrushers.QuestMachine.EditorPrefsDebug.DefaultInspectorFoldout";
            var foldout = EditorPrefs.GetBool(key);
            var newFoldout = EditorGUILayout.Foldout(foldout, "Default Inspector");
            if (newFoldout != foldout) EditorPrefs.SetBool(key, newFoldout);
            if (newFoldout) base.OnInspectorGUI();
#endif

            serializedObject.Update();
            DrawDescription();
            DrawQuestList();
            serializedObject.ApplyModifiedProperties();
            DrawGetAllInSceneButton();
            DrawImagesList();
        }

        private void DrawDescription()
        {
            var descriptionProperty = serializedObject.FindProperty("m_description");
            if (descriptionProperty == null) return;
            EditorGUILayout.PropertyField(descriptionProperty);
        }

        private void DrawQuestList()
        {
            if (!showQuestRelations && !(0 <= questReorderableList.index && questReorderableList.index <= questReorderableList.count))
            {
                questReorderableList.index = 0;
                SetQuestInEditorWindow(questReorderableList.index);
            }

            questReorderableList.DoLayoutList();
        }

        private void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Quest Assets");
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (!(0 <= index && index < questReorderableList.serializedProperty.arraySize)) return;
            var buttonWidth = 48f;
            var questRect = new Rect(rect.x, rect.y + 1, rect.width - buttonWidth - 2, EditorGUIUtility.singleLineHeight);
            var buttonRect = new Rect(rect.x + rect.width - buttonWidth, rect.y + 1, buttonWidth, EditorGUIUtility.singleLineHeight);
            var questProperty = questReorderableList.serializedProperty.GetArrayElementAtIndex(index);
            var isQuestAssigned = questProperty.objectReferenceValue != null;
            var buttonGUIContent = isQuestAssigned ? new GUIContent("Edit", "Edit in Quest Editor window.") : new GUIContent("New", "Create new quest asset in this slot.");
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
                    "Do you want to remove the reference to '" + quest.title.value + "' from this database or permanently delete the quest asset from your whole project?",
                    "Remove Reference", "Permanently Delete", "Cancel");
                if (option == 2) return; // Cancel.
                permanentlyDelete = (option == 1);
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
                if (permanentlyDelete)
                {
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(quest));
                }
            }
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
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
            var questDatabase = target as QuestDatabase;
            if (questDatabase == null) return;
            if (!(0 <= questListIndex && questListIndex < questDatabase.questAssets.Count)) return;
            QuestEditorWindow.ShowWindow();
            var quest = questDatabase.questAssets[questListIndex];
            QuestEditorWindow.instance.SelectQuest(quest);
        }

        private void DrawGetAllInSceneButton()
        {
            try
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent("Show Quest Relations", "Show how all quests in database are linked."), GUILayout.Width(136)))
                {
                    showQuestRelations = true;
                    QuestEditorWindow.ShowWindow();
                    QuestEditorWindow.instance.ShowQuestRelations(target as QuestDatabase);
                }
                if (GUILayout.Button("Add All In Scene", GUILayout.Width(128)))
                {
                    if (EditorUtility.DisplayDialog("Add All In Scene", "Add all quests assigned in the current scene?", "OK", "Cancel"))
                    {
                        AddAllInScene();
                    }
                }
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }
        }

        private void AddAllInScene()
        {
            var database = target as QuestDatabase;
            if (database == null) return;
            Undo.RecordObject(database, "Add Quests In Scene");
            foreach (var questListContainer in FindObjectsOfType<QuestListContainer>())
            {
                foreach (var quest in questListContainer.questList)
                {
                    if (quest.isAsset && !database.questAssets.Contains(quest))
                    {
                        database.questAssets.Add(quest);
                    }
                }
            }
        }

        private void DrawImagesList()
        {
            QuestEditorPrefs.databaseImagesFoldout = QuestEditorUtility.EditorGUILayoutFoldout("EntityType Images", "Images used by procedural entity types. If you're not procedurally generating quests, you can ignore this section.", QuestEditorPrefs.databaseImagesFoldout);
            if (!QuestEditorPrefs.databaseImagesFoldout) return;

            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_images"), true);
            serializedObject.ApplyModifiedProperties();
            try
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent("Scan EntityTypes...", "Adds images used by EntityTypes in a specified folder."), GUILayout.Width(128)))
                {
                    if (string.IsNullOrEmpty(entityTypeFolderPath)) entityTypeFolderPath = Application.dataPath;
                    entityTypeFolderPath = EditorUtility.OpenFolderPanel("Scan EntityTypes In", entityTypeFolderPath, string.Empty);
                    if (!string.IsNullOrEmpty(entityTypeFolderPath) && Directory.Exists(entityTypeFolderPath))
                    {
                        Undo.RecordObject(target, "Add EntityType Images");
                        ScanEntityTypesInFolder(entityTypeFolderPath);
                    }
                }
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }
        }

        private void ScanEntityTypesInFolder(string path)
        {
            var database = target as QuestDatabase;
            if (database == null) return;
            Undo.RecordObject(database, "Add Images");
            var dataPathLength = Application.dataPath.Length - "Assets".Length;
            var filenames = Directory.GetFiles(path, "*.asset", SearchOption.AllDirectories);
            foreach (var filename in filenames)
            {
                var relativeFilename = filename.Substring(dataPathLength).Replace("\\", "/");
                var entityType = AssetDatabase.LoadAssetAtPath<EntityType>(relativeFilename);
                if (entityType != null && entityType.image != null && !database.images.Contains(entityType.image))
                {
                    database.images.Add(entityType.image);
                }
            }
        }

    }
}
