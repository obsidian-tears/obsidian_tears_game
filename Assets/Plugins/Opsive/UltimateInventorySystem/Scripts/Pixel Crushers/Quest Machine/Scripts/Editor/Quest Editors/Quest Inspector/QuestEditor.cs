// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Top level inspector for Quest objects. Delegates the actual GUI 
    /// work to QuestInspectorGUI.
    /// </summary>
    [CustomEditor(typeof(Quest), true)]
    public class QuestEditor : Editor
    {

        private QuestInspectorGUI inspectorGUI { get; set; }

        private void OnEnable()
        {
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemOnGUI;
            Undo.undoRedoPerformed += RepaintEditorWindow;
            QuestEditorWindow.currentEditor = this;
            if (inspectorGUI == null) inspectorGUI = new QuestInspectorGUI();
        }

        private void OnDisable()
        {
            EditorApplication.projectWindowItemOnGUI -= OnProjectWindowItemOnGUI;
            Undo.undoRedoPerformed -= RepaintEditorWindow;
            QuestEditorWindow.currentEditor = null;
        }

        private void OnProjectWindowItemOnGUI(string guid, Rect selectionRect)
        {
            if (Event.current.type == EventType.MouseDown && Event.current.clickCount == 2 && selectionRect.Contains(Event.current.mousePosition))
            {
                QuestEditorWindow.ShowWindow();
            }
        }

        private void RepaintEditorWindow()
        {
            QuestEditorWindow.RepaintNow();
        }

        public override void OnInspectorGUI()
        {
            #if DEBUG_QUEST_EDITOR
            var key = "PixelCrushers.QuestMachine.EditorPrefsDebug.InspectorFoldout";
            var foldout = EditorPrefs.GetBool(key);
            var newFoldout = EditorGUILayout.Foldout(foldout, "Default Inspector");
            if (newFoldout != foldout) EditorPrefs.SetBool(key, newFoldout);
            if (newFoldout) base.OnInspectorGUI();
            #endif

            if (QuestEditorWindow.instance == null)
            {
                if (GUILayout.Button(new GUIContent("Open Quest Editor", "Edit or inspect this quest in the Quest Editor window.")))
                {
                    QuestEditorWindow.ShowWindow();
                }
            }
            else
            {
                serializedObject.Update();
                inspectorGUI.Draw(serializedObject);
                serializedObject.ApplyModifiedProperties();
                if (GUI.changed) RepaintEditorWindow();
            }
        }

    }
}
