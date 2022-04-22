// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    [CustomPropertyDrawer(typeof(MessageValue), true)]
    public class MessageValueDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var valueTypeProperty = property.FindPropertyRelative("m_valueType");
            if (valueTypeProperty == null) return 3 * EditorGUIUtility.singleLineHeight;
            return ((valueTypeProperty.enumValueIndex == (int)MessageValueType.None) ? 1 : 2) * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueTypeProperty = property.FindPropertyRelative("m_valueType");
            var intValueProperty = property.FindPropertyRelative("m_intValue");
            var stringValueProperty = property.FindPropertyRelative("m_stringValue");
            if (valueTypeProperty == null || intValueProperty == null || stringValueProperty == null) return;

            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            var typeRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            var valueRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(typeRect, valueTypeProperty, GUIContent.none);
            switch ((MessageValueType)valueTypeProperty.enumValueIndex)
            {
                case MessageValueType.Int:
                    EditorGUI.PropertyField(valueRect, intValueProperty, GUIContent.none);
                    break;
                case MessageValueType.String:
                    EditorGUI.PropertyField(valueRect, stringValueProperty, GUIContent.none);
                    break;
            }
            EditorGUI.EndProperty();
        }

    }
}
