// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Custom inspector for TimerQuestCondition assets.
    /// </summary>
    [CustomEditor(typeof(TimerQuestCondition), true)]
    public class TimerQuestConditionEditor : QuestSubassetEditor
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
            EditorGUILayout.HelpBox("When this node becomes active, this condition starts a countdown using the counter defined in your quest. Make sure the counter value is set to your desired time before using this action. Every second, the counter value will decrease by one. If it reaches zero, the condition will be true.", MessageType.None);
            var counterIndexProperty = serializedObject.FindProperty("m_counterIndex");
            UnityEngine.Assertions.Assert.IsNotNull(counterIndexProperty, "Quest Machine: Internal error - m_counterIndex is null.");
            if (counterIndexProperty == null) return;
            QuestEditorUtility.EditorGUILayoutCounterNamePopup(counterIndexProperty, m_nameList);
            if (GUILayout.Button("Refresh Counter Names")) m_nameList = QuestEditorUtility.GetCounterNames();
        }

    }
}
