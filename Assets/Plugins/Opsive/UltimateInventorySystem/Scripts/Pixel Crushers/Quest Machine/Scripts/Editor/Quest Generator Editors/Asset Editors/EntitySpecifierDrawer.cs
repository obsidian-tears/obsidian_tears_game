// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    [CustomPropertyDrawer(typeof(EntitySpecifier), true)]
    public class EntitySpecifierDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            var entitySpecifierTypeProperty = property.FindPropertyRelative("m_entitySpecifierType");
            var entityTypeProperty = property.FindPropertyRelative("m_entityType");
            var isOther = (EntitySpecifierType)entitySpecifierTypeProperty.enumValueIndex == EntitySpecifierType.Other;
            float enumWidth = isOther ? 50 : rect.width;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, enumWidth, EditorGUIUtility.singleLineHeight), entitySpecifierTypeProperty, GUIContent.none);
            if (isOther)
            {
                EditorGUI.PropertyField(new Rect(rect.x + enumWidth, rect.y, rect.width - enumWidth, EditorGUIUtility.singleLineHeight), entityTypeProperty, GUIContent.none);
            }

            EditorGUI.EndProperty();
        }

    }

}
