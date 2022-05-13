// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Custom property drawer for QuestAudioSourceIdentifier.
    /// </summary>
    [CustomPropertyDrawer(typeof(QuestAudioSourceIdentifier))]
    public class QuestAudioSourceIdentifierDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property == null) return base.GetPropertyHeight(property, label);
            var typeProperty = property.FindPropertyRelative("m_type");
            UnityEngine.Assertions.Assert.IsNotNull(typeProperty, "Quest Machine: Internal error - m_type is null.");
            if (typeProperty == null) return base.GetPropertyHeight(property, label);

            var needsID = typeProperty.enumValueIndex == (int)QuestAudioSourceIdentifierType.GameObjectWithTag || typeProperty.enumValueIndex == (int)QuestAudioSourceIdentifierType.GameObjectWithName;
            var numLines = needsID ? 2 : 1;
            return numLines * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            try
            {
                var typeProperty = property.FindPropertyRelative("m_type");
                UnityEngine.Assertions.Assert.IsNotNull(typeProperty, "Quest Machine: Internal error - m_type is null.");
                if (typeProperty == null) return;

                var y = rect.y;
                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                    typeProperty, new GUIContent("Identify Audio Source By", "The counter's name."));
                y += EditorGUIUtility.singleLineHeight;

                var idProperty = property.FindPropertyRelative("m_id");
                UnityEngine.Assertions.Assert.IsNotNull(idProperty, "Quest Machine: Internal error - m_id is null.");
                if (idProperty == null) return;
                var idRect = new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight);
                switch ((QuestAudioSourceIdentifierType)typeProperty.enumValueIndex)
                {
                    case QuestAudioSourceIdentifierType.GameObjectWithTag:
                        EditorGUI.PropertyField(idRect, idProperty,
                            new GUIContent("Tag", "Tag of GameObject with audio source to use."));
                        break;
                    case QuestAudioSourceIdentifierType.GameObjectWithName:
                        EditorGUI.PropertyField(idRect, idProperty,
                            new GUIContent("GameObject Name", "Name of GameObject with audio source to use."));
                        break;
                }
            }
            finally
            {
                EditorGUI.EndProperty();
            }
        }

    }

}
