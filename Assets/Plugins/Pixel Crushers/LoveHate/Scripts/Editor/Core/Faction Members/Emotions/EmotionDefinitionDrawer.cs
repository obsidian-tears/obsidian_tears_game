// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// This custom property drawer for EmotionDefinion uses min-max sliders.
    /// </summary>
    [CustomPropertyDrawer(typeof(EmotionDefinition), true)]
    class EmotionDefinitionDrawer : PropertyDrawer
    {

        private float rangeTextWidth;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 4 * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            rangeTextWidth = Mathf.Min(80f, Mathf.Max(48f, position.width / 6));
            var nameRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            var pleasureRect = new Rect(position.x + 16, position.y + EditorGUIUtility.singleLineHeight, position.width - 16 - 2 * rangeTextWidth, EditorGUIUtility.singleLineHeight);
            var arousalRect = new Rect(position.x + 16, position.y + 2 * EditorGUIUtility.singleLineHeight, position.width - 16 - 2 * rangeTextWidth, EditorGUIUtility.singleLineHeight);
            var dominanceRect = new Rect(position.x + 16, position.y + 3 * EditorGUIUtility.singleLineHeight, position.width - 16 - 2 * rangeTextWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"), new GUIContent("Emotion"));
            ShowMinMaxSlider("Pleasure", pleasureRect, property, "pleasureMin", "pleasureMax");
            ShowMinMaxSlider("Arousal", arousalRect, property, "arousalMin", "arousalMax");
            ShowMinMaxSlider("Dominance", dominanceRect, property, "dominanceMin", "dominanceMax");

            EditorGUI.EndProperty();
        }

        private void ShowMinMaxSlider(string label, Rect rect, SerializedProperty property, string minPropertyName, string maxPropertyName)
        {
            var min = property.FindPropertyRelative(minPropertyName).floatValue;
            var max = property.FindPropertyRelative(maxPropertyName).floatValue;
#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3
            EditorGUI.MinMaxSlider(new GUIContent(label), rect, ref min, ref max, -100f, 100f);
#else
            EditorGUI.MinMaxSlider(rect, new GUIContent(label), ref min, ref max, -100f, 100f);
#endif
            min = EditorGUI.FloatField(new Rect(rect.x + rect.width, rect.y, rangeTextWidth, rect.height), min);
            max = EditorGUI.FloatField(new Rect(rect.x + rect.width + rangeTextWidth, rect.y, rangeTextWidth, rect.height), max);
            property.FindPropertyRelative(minPropertyName).floatValue = min;
            property.FindPropertyRelative(maxPropertyName).floatValue = max;
        }

    }
}
