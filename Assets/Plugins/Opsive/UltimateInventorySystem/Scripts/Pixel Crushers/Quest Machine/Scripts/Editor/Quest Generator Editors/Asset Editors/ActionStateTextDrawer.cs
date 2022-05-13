// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using System;

namespace PixelCrushers.QuestMachine
{

    [CustomPropertyDrawer(typeof(ActionStateText), true)]
    public class ActionStateTextDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return
                StringFieldDrawer.GetHeight(property.FindPropertyRelative("m_dialogueText")) +
                StringFieldDrawer.GetHeight(property.FindPropertyRelative("m_journalText")) +
                StringFieldDrawer.GetHeight(property.FindPropertyRelative("m_hudText")) +
                StringFieldDrawer.GetHeight(property.FindPropertyRelative("m_alertText"));
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            float height = 0;

            var dialogueTextProperty = property.FindPropertyRelative("m_dialogueText");
            var fieldHeight = StringFieldDrawer.GetHeight(dialogueTextProperty);
            StringFieldDrawer.Draw(new Rect(rect.x, rect.y + height, rect.width, fieldHeight), dialogueTextProperty, new GUIContent("Dialogue Text"), false);
            height += fieldHeight;

            var journalTextProperty = property.FindPropertyRelative("m_journalText");
            fieldHeight = StringFieldDrawer.GetHeight(journalTextProperty);
            StringFieldDrawer.Draw(new Rect(rect.x, rect.y + height, rect.width, fieldHeight), journalTextProperty, new GUIContent("Journal Text"), false);
            height += fieldHeight;

            var hudTextProperty = property.FindPropertyRelative("m_hudText");
            fieldHeight = StringFieldDrawer.GetHeight(hudTextProperty);
            StringFieldDrawer.Draw(new Rect(rect.x, rect.y + height, rect.width, fieldHeight), hudTextProperty, new GUIContent("HUD Text"), false);
            height += fieldHeight;

            var alertTextProperty = property.FindPropertyRelative("m_alertText");
            fieldHeight = StringFieldDrawer.GetHeight(alertTextProperty);
            StringFieldDrawer.Draw(new Rect(rect.x, rect.y + height, rect.width, fieldHeight), alertTextProperty, new GUIContent("Alert Text"), false);
            height += fieldHeight;

            EditorGUI.EndProperty();
        }

    }

}
