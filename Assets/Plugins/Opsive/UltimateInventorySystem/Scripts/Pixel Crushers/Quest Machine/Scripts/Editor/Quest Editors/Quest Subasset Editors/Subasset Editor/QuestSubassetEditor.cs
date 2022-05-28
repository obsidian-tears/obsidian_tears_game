// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Base abstract editor class for quest subassets.
    /// </summary>
    public abstract class QuestSubassetEditor : Editor
    {

        protected virtual void OnEnable()
        {
            Undo.undoRedoPerformed -= RepaintEditorWindow;
            Undo.undoRedoPerformed += RepaintEditorWindow;
        }

        protected virtual void OnDisable()
        {
            Undo.undoRedoPerformed -= RepaintEditorWindow;
        }

        protected void RepaintEditorWindow()
        {
            QuestEditorWindow.RepaintNow();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Draw();
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed) RepaintEditorWindow();
        }

        protected virtual void Draw()
        {
        }

    }
}
