using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine.UISSupport
{

    /// <summary>
    /// Custom inspector for UISItemAmountQuestCondition assets.
    /// </summary>
    [CustomEditor(typeof(UISItemAmountQuestCondition), true)]
    public class UISItemAmountQuestConditionEditor : QuestSubassetEditor
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
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(UISItemAmountQuestCondition.itemName)), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(UISItemAmountQuestCondition.inventoryName)), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(UISItemAmountQuestCondition.comparisonMode)), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(UISItemAmountQuestCondition.amount)), true);
            var counterIndexProperty = serializedObject.FindProperty(nameof(UISItemAmountQuestCondition.counterIndex));
            UnityEngine.Assertions.Assert.IsNotNull(counterIndexProperty, "Quest Machine: Internal error - counterIndex is null.");
            if (counterIndexProperty == null) return;
            QuestEditorUtility.EditorGUILayoutCounterNamePopup(counterIndexProperty, m_nameList);
            if (GUILayout.Button("Refresh Counter Names")) m_nameList = QuestEditorUtility.GetCounterNames();
        }

    }
}
