// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// This is a custom editor for DeedReactionEvents
    /// that uses reorderable lists.
    /// </summary>
    [CustomEditor(typeof(DeedReactionEvents), true)]
    [CanEditMultipleObjects]
    public class DeedReactionEventsEditor : Editor
    {

        private ReorderableList m_list;

        private void OnEnable()
        {
            m_list = new ReorderableList(
                serializedObject, serializedObject.FindProperty("reactions"),
                true, true, true, true);
            m_list.drawHeaderCallback = OnDrawListHeader;
            m_list.drawElementCallback = OnDrawListElement;
        }

        public override void OnInspectorGUI()
        {
            Undo.RecordObject(target, "DeedReactionEvents");
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("timeBetweenReactions"));
            m_list.DoLayoutList();
            DrawCurrentReactionEvents();
            serializedObject.ApplyModifiedProperties();
        }

        private void OnDrawListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Reaction Events");
        }

        private void OnDrawListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = m_list.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            float floatWidth = Mathf.Min(rect.width / 4, 60);
            float maskWidth = rect.width - 2 * floatWidth;
            element.FindPropertyRelative("temperament").intValue =
                (int)EditorGUI.MaskField(new Rect(rect.x + 2, rect.y, maskWidth, EditorGUIUtility.singleLineHeight),
                                          GUIContent.none, element.FindPropertyRelative("temperament").intValue,
                                          RangeAnimation.AllTemperamentNames);
            EditorGUI.PropertyField(
                new Rect(rect.x + 2 + maskWidth, rect.y, floatWidth, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("min"), GUIContent.none);
            EditorGUI.PropertyField(
                new Rect(rect.x + 2 + maskWidth + 2 + floatWidth, rect.y, floatWidth, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("max"), GUIContent.none);
        }

        private void DrawCurrentReactionEvents()
        {
            if (m_list != null && 0 <= m_list.index && m_list.index < m_list.serializedProperty.arraySize)
            {
                var element = m_list.serializedProperty.GetArrayElementAtIndex(m_list.index);
                EditorGUILayout.PropertyField(element.FindPropertyRelative("onReact"), true);
            }
        }

    }

}