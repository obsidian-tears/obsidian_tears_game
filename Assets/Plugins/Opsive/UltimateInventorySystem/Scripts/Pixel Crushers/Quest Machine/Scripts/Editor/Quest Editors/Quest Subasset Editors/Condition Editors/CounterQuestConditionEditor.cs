// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Custom inspector for CounterQuestCondition assets.
    /// </summary>
    [CustomEditor(typeof(CounterQuestCondition), true)]
    public class CounterQuestConditionEditor : QuestSubassetEditor
    {

        private string[] m_nameList = null;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (target == null || serializedObject == null) return;
            m_nameList = QuestEditorUtility.GetCounterNames();
        }

        protected override void Draw()
        {
            if (serializedObject == null) return;
            var counterIndexProperty = serializedObject.FindProperty("m_counterIndex");
            var counterValueModeProperty = serializedObject.FindProperty("m_counterValueMode");
            var requiredCounterValueProperty = serializedObject.FindProperty("m_requiredCounterValue");
            UnityEngine.Assertions.Assert.IsNotNull(counterIndexProperty, "Quest Machine: Internal error - m_counterIndex is null.");
            UnityEngine.Assertions.Assert.IsNotNull(counterValueModeProperty, "Quest Machine: Internal error - m_counterValueMode is null.");
            UnityEngine.Assertions.Assert.IsNotNull(requiredCounterValueProperty, "Quest Machine: Internal error - m_requiredCounterValue is null.");
            if (counterIndexProperty == null || counterValueModeProperty == null || requiredCounterValueProperty == null)
            {
                Debug.LogError("Quest Machine: Internal error in CounterQuestConditionEditor. Please contact the developer.");
                return;
            }
            EditorGUILayout.HelpBox("This condition requires that the value of a counter defined in your quest meets a criteria such as being at least a specific amount.", MessageType.None);
            QuestEditorUtility.EditorGUILayoutCounterNamePopup(counterIndexProperty, m_nameList);
            EditorGUILayout.PropertyField(counterValueModeProperty);
            EditorGUILayout.PropertyField(requiredCounterValueProperty, true);
            if (GUILayout.Button("Refresh Counter Names")) m_nameList = QuestEditorUtility.GetCounterNames();
        }

    }
}
