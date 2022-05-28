using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine.UISSupport
{

    /// <summary>
    /// Custom inspector for UISCurrencyAmountQuestCondition assets.
    /// </summary>
    [CustomEditor(typeof(UISCurrencyAmountQuestCondition), true)]
    public class UISCurrencyAmountQuestConditionEditor : QuestSubassetEditor
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
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(UISCurrencyAmountQuestCondition.currencyName)), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(UISCurrencyAmountQuestCondition.currencyOwnerName)), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(UISCurrencyAmountQuestCondition.comparisonMode)), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(UISCurrencyAmountQuestCondition.amount)), true);
            var counterIndexProperty = serializedObject.FindProperty(nameof(UISCurrencyAmountQuestCondition.counterIndex));
            UnityEngine.Assertions.Assert.IsNotNull(counterIndexProperty, "Quest Machine: Internal error - counterIndex is null.");
            if (counterIndexProperty == null) return;
            QuestEditorUtility.EditorGUILayoutCounterNamePopup(counterIndexProperty, m_nameList);
            if (GUILayout.Button("Refresh Counter Names")) m_nameList = QuestEditorUtility.GetCounterNames();
        }

    }
}
