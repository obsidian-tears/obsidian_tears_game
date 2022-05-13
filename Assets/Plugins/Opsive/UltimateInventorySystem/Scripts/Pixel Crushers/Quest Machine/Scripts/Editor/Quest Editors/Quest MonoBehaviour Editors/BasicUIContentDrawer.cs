// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Custom property drawer for BasicUIContent.
    /// </summary>
    [CustomPropertyDrawer(typeof(BasicUIContent))]
    public class BasicUIContentDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var m_headingText = property.FindPropertyRelative("m_headingText");
            var m_bodyText = property.FindPropertyRelative("m_bodyText");
            return 2 * EditorGUIUtility.singleLineHeight +
                StringFieldDrawer.GetHeight(m_headingText) +
                StringFieldDrawer.GetHeight(m_bodyText);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var m_icon = property.FindPropertyRelative("m_icon");
            var m_headingText = property.FindPropertyRelative("m_headingText");
            var m_bodyText = property.FindPropertyRelative("m_bodyText");

            EditorGUI.BeginProperty(position, label, property);
            try
            {
                var rect = position;
                var height = EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, height), label, QuestEditorStyles.CollapsibleSubheaderButtonStyleName);
                rect.y += height;

                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, height), m_icon);
                rect.y += height;

                height = StringFieldDrawer.GetHeight(m_headingText);
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, height), m_headingText);
                rect.y += height;

                height = StringFieldDrawer.GetHeight(m_bodyText);
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, height), m_bodyText);
            }
            finally
            {
                EditorGUI.EndProperty();
            }
        }

    }

}
