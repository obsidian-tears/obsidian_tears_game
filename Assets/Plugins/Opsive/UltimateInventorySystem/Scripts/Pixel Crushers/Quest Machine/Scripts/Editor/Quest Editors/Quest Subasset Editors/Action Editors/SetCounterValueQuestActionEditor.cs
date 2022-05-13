// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Custom inspector for SetCounterValueQuestAction assets.
    /// </summary>
    [CustomEditor(typeof(SetCounterValueQuestAction), true)]
    public class SetCounterValueQuestActionEditor : QuestSubassetEditor
    {

        private string[] m_nameList = null;
        private GUIContent m_operationValueTooltip = null;

        protected override void Draw()
        {
            if (serializedObject == null) return;
            if (m_nameList == null) m_nameList = QuestEditorUtility.GetCounterNames();
            serializedObject.Update();
            var counterIndexProperty = serializedObject.FindProperty("m_counterIndex");
            var operationProperty = serializedObject.FindProperty("m_operation");
            var operationValueProperty = serializedObject.FindProperty("m_operationValue");
            var maxValueProperty = serializedObject.FindProperty("m_maxValue");
            UnityEngine.Assertions.Assert.IsNotNull(counterIndexProperty, "Quest Machine: Internal error - m_counterIndex is null.");
            UnityEngine.Assertions.Assert.IsNotNull(operationProperty, "Quest Machine: Internal error - m_operation is null.");
            UnityEngine.Assertions.Assert.IsNotNull(operationValueProperty, "Quest Machine: Internal error - m_operationValue is null.");
            UnityEngine.Assertions.Assert.IsNotNull(maxValueProperty, "Quest Machine: Internal error - m_maxValue is null.");
            if (counterIndexProperty == null || operationProperty == null || operationValueProperty == null || maxValueProperty == null) return;
            QuestEditorUtility.EditorGUILayoutCounterNamePopup(counterIndexProperty, m_nameList);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(operationProperty);
            if (EditorGUI.EndChangeCheck()) m_operationValueTooltip = null;
            var operation = (SetCounterValueQuestAction.Operation)operationProperty.enumValueIndex;
            if (m_operationValueTooltip == null) m_operationValueTooltip = GetOperationValueTooltip(operation);
            EditorGUILayout.PropertyField(operationValueProperty, m_operationValueTooltip);
            if (operation == SetCounterValueQuestAction.Operation.Randomize)
            {
                EditorGUILayout.PropertyField(maxValueProperty);
            }
            serializedObject.ApplyModifiedProperties();
        }

        protected virtual GUIContent GetOperationValueTooltip(SetCounterValueQuestAction.Operation operation)
        {
            switch (operation)
            {
                case SetCounterValueQuestAction.Operation.SetToValue:
                    return new GUIContent("Set To");
                case SetCounterValueQuestAction.Operation.ModifyByValue:
                    return new GUIContent("Modify By");
                case SetCounterValueQuestAction.Operation.Randomize:
                    return new GUIContent("Min Value");
                default:
                    return new GUIContent("Operation Value");
            }
        }

    }
}
