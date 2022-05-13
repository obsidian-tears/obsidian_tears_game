// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// This is a custom editor for AbstractGreetingAnimationTrigger
    /// that uses reorderable lists.
    /// </summary>
    [CustomEditor(typeof(AbstractGreetingAnimationTrigger), true)]
    [CanEditMultipleObjects]
    public class AbstractGreetingAnimationTriggerEditor : Editor
    {

        private ReorderableList animsList;

        private void OnEnable()
        {
            animsList = new ReorderableList(
                serializedObject, serializedObject.FindProperty("greetings"),
                true, true, true, true);
            animsList.drawHeaderCallback = OnDrawAnimsHeader;
            animsList.drawElementCallback = OnDrawAnimsElement;
        }

        public override void OnInspectorGUI()
        {
            Undo.RecordObject(target, "GreetingTrigger");
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cacheSize"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("timeBetweenGreetings"));
            animsList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private void OnDrawAnimsHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Animation Triggers");
        }

        private void OnDrawAnimsElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = animsList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            float floatWidth = Mathf.Min(rect.width / 4, 60);
            float maskWidth = Mathf.Min(rect.width / 4, 120);
            var triggerWidth = rect.width - (2 * floatWidth) - maskWidth - 6;
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, triggerWidth, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("triggerParameter"), GUIContent.none);
            element.FindPropertyRelative("temperament").intValue =
                (int)EditorGUI.MaskField(new Rect(rect.x + 2 + triggerWidth, rect.y, maskWidth, EditorGUIUtility.singleLineHeight),
                                          GUIContent.none, element.FindPropertyRelative("temperament").intValue,
                                          RangeAnimation.AllTemperamentNames);
            EditorGUI.PropertyField(
                new Rect(rect.x + 2 + triggerWidth + 2 + maskWidth, rect.y, floatWidth, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("min"), GUIContent.none);
            EditorGUI.PropertyField(
                new Rect(rect.x + 2 + triggerWidth + 2 + maskWidth + 2 + floatWidth, rect.y, floatWidth, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("max"), GUIContent.none);
        }

    }

}