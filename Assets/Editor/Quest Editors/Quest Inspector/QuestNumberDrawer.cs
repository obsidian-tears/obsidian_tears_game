// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Custom property drawer for QuestNumber.
    /// </summary>
    [CustomPropertyDrawer(typeof(QuestNumber))]
    public class QuestNumberDrawer : PropertyDrawer
    {

        private string[] m_counterNameList = null;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 2 * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            rect = EditorGUI.PrefixLabel(rect, GUIUtility.GetControlID(FocusType.Passive), label);
            try
            {
                var valueTypeProperty = property.FindPropertyRelative("m_valueType");
                UnityEngine.Assertions.Assert.IsNotNull(valueTypeProperty, "Quest Machine: Internal error - m_valueType is null.");
                if (valueTypeProperty == null) return;
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    valueTypeProperty, GUIContent.none);
                var valueType = (QuestNumber.ValueType)valueTypeProperty.enumValueIndex;
                var valueRect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width, EditorGUIUtility.singleLineHeight);
                switch (valueType)
                {
                    case QuestNumber.ValueType.Literal:
                        var literalValueProperty = property.FindPropertyRelative("m_literalValue");
                        UnityEngine.Assertions.Assert.IsNotNull(literalValueProperty, "Quest Machine: Internal error - m_literalValue is null.");
                        if (literalValueProperty == null) return;
                        EditorGUI.PropertyField(valueRect, literalValueProperty, GUIContent.none);
                        break;
                    case QuestNumber.ValueType.CounterValue:
                    case QuestNumber.ValueType.CounterMinValue:
                    case QuestNumber.ValueType.CounterMaxValue:
                        var counterIndexProperty = property.FindPropertyRelative("m_counterIndex");
                        if (m_counterNameList == null) m_counterNameList = QuestEditorUtility.GetCounterNames();
                        if (counterIndexProperty != null)
                        {
                            QuestEditorUtility.EditorGUICounterNamePopup(valueRect, counterIndexProperty, m_counterNameList);
                        }
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
