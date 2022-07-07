// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Custom inspector for ActivateGameObjectQuestAction assets.
    /// Implements drag-and-drop into the m_gameObjectName field.
    /// </summary>
    [CustomEditor(typeof(ActivateGameObjectQuestAction), true)]
    public class ActivateGameObjectQuestActionEditor : QuestSubassetEditor
    {
        protected SerializedProperty gameObjectNameProperty;
        protected SerializedProperty stateProperty;
        protected Rect gameObjectNameRect;

        protected override void OnEnable()
        {
            base.OnEnable();
            gameObjectNameProperty = serializedObject.FindProperty("m_gameObjectName");
            stateProperty = serializedObject.FindProperty("m_state");
        }

        protected override void Draw()
        {
            // DrawDefaultInspector();
            serializedObject.Update();
            EditorGUILayout.PropertyField(gameObjectNameProperty);
            if (Event.current.type == EventType.Repaint) gameObjectNameRect = GUILayoutUtility.GetLastRect();
            EditorGUILayout.PropertyField(stateProperty);
            if ((Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform) &&
                gameObjectNameRect.Contains(Event.current.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (Event.current.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    foreach (var obj in DragAndDrop.objectReferences)
                    {
                        if (obj is GameObject)
                        {
                            StringFieldDrawer.SetStringFieldValue(gameObjectNameProperty, obj.name);
                        }
                    }
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
