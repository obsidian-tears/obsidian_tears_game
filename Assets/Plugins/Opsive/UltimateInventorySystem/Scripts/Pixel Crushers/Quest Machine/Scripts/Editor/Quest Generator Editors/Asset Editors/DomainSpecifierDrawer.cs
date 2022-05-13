// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    [CustomPropertyDrawer(typeof(DomainSpecifier), true)]
    public class DomainSpecifierDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            var domainSpecifierTypeProperty = property.FindPropertyRelative("m_domainSpecifierType");
            var domainTypeProperty = property.FindPropertyRelative("m_domainType");
            var isOther = (DomainSpecifierType)domainSpecifierTypeProperty.enumValueIndex == DomainSpecifierType.Other;
            float enumWidth = isOther ? 50 : rect.width;
            float dropdownWidth = 16;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, enumWidth, rect.height), domainSpecifierTypeProperty, GUIContent.none);
            if (isOther)
            {
                EditorGUI.PropertyField(new Rect(rect.x + enumWidth, rect.y, rect.width - enumWidth - dropdownWidth, rect.height), domainTypeProperty, GUIContent.none);
            }

            EditorGUI.EndProperty();
        }

    }

}
