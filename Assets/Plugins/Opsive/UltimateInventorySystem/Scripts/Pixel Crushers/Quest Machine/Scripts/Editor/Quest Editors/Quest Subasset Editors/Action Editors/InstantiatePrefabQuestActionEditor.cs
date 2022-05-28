// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Custom inspector for InstantiatePrefabQuestAction assets.
    /// </summary>
    [CustomEditor(typeof(InstantiatePrefabQuestAction), true)]
    public class InstantiatePrefabQuestActionEditor : QuestSubassetEditor
    {

        private Rect rect;

        protected override void Draw()
        {
            if (serializedObject == null) return;
            serializedObject.Update();
            var prefabProperty = serializedObject.FindProperty("m_prefab");
            var locationTransformProperty = serializedObject.FindProperty("m_locationTransform");
            UnityEngine.Assertions.Assert.IsNotNull(prefabProperty, "Quest Machine: Internal error - m_prefab is null.");
            UnityEngine.Assertions.Assert.IsNotNull(locationTransformProperty, "Quest Machine: Internal error - m_locationTransform is null.");
            if (prefabProperty == null || locationTransformProperty == null) return;
            EditorGUILayout.PropertyField(prefabProperty, new GUIContent("Prefab to Instantiate"), true);
            EditorGUILayout.PropertyField(locationTransformProperty, new GUIContent("Location Transform", "Name of GameObject (usually an empty GameObject) where prefab should be instantiated. You can drag and drop a scene object here."), true);

            // Handle drag-and-drop for Location Transform:
            switch (Event.current.type)
            {
                case EventType.Repaint:
                    rect = GUILayoutUtility.GetLastRect();
                    break;
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (rect.Contains(Event.current.mousePosition))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        if (Event.current.type == EventType.DragPerform)
                        {
                            DragAndDrop.AcceptDrag();
                            foreach (var obj in DragAndDrop.objectReferences)
                            {
                                Transform t = null;
                                if (obj is Component) t = (obj as Component).transform;
                                else if (obj is GameObject) t = (obj as GameObject).transform;
                                if (obj != null) locationTransformProperty.FindPropertyRelative("m_text").stringValue = t.name;
                            }
                        }
                    }
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }

    }
}
