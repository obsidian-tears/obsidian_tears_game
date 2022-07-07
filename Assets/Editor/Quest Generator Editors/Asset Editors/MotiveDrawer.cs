// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    [CustomPropertyDrawer(typeof(Motive), true)]
    public class MotiveDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var driveValuesProperty = property.FindPropertyRelative("m_driveValues");
            if (driveValuesProperty == null) return base.GetPropertyHeight(property, label);
            return (StringFieldDrawer.NumExpandedLines + 2) * EditorGUIUtility.singleLineHeight +
                (driveValuesProperty.arraySize + 1) * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            var textProperty = property.FindPropertyRelative("m_text");
            if (textProperty == null) return;
            StringFieldDrawer.Draw(rect, textProperty, label, true);

            var driveValuesProperty = property.FindPropertyRelative("m_driveValues");
            if (driveValuesProperty == null) return;
            var textHeight = (StringFieldDrawer.NumExpandedLines + 2) * EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(new Rect(rect.x, rect.y + textHeight, rect.width / 2, EditorGUIUtility.singleLineHeight), "Drive");
            EditorGUI.LabelField(new Rect(rect.x + rect.width / 2, rect.y + textHeight, rect.width / 2, EditorGUIUtility.singleLineHeight), "Value");
            if (GUI.Button(new Rect(rect.x + rect.width - 30, rect.y + textHeight, 30, EditorGUIUtility.singleLineHeight), "+", EditorStyles.miniButton))
            {
                driveValuesProperty.arraySize++;
            }
            int indexToDelete = -1;
            for (int i = 0; i < driveValuesProperty.arraySize; i++)
            {
                var driveValueProperty = driveValuesProperty.GetArrayElementAtIndex(i);
                if (driveValuesProperty == null) continue;
                var driveProperty = driveValueProperty.FindPropertyRelative("m_drive");
                var valueProperty = driveValueProperty.FindPropertyRelative("m_value");
                if (driveProperty == null || valueProperty == null) continue;
                EditorGUI.PropertyField(new Rect(rect.x, rect.y + textHeight + (i + 1) * EditorGUIUtility.singleLineHeight, rect.width / 2, EditorGUIUtility.singleLineHeight), driveProperty, GUIContent.none);
                EditorGUI.PropertyField(new Rect(rect.x + rect.width / 2, rect.y + textHeight + (i + 1) * EditorGUIUtility.singleLineHeight, (rect.width / 2) - 30, EditorGUIUtility.singleLineHeight), valueProperty, GUIContent.none);
                if (GUI.Button(new Rect(rect.x + rect.width - 30, rect.y + textHeight + (i + 1) * EditorGUIUtility.singleLineHeight, 30, EditorGUIUtility.singleLineHeight), "-", EditorStyles.miniButtonRight)) indexToDelete = i;
            }
            if (indexToDelete != -1) driveValuesProperty.DeleteArrayElementAtIndex(indexToDelete);

            EditorGUI.EndProperty();
        }

    }

}
