using UnityEngine;
using System;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Exchange;
using PixelCrushers.UISSupport;

namespace PixelCrushers.QuestMachine.UISSupport
{

    /// <summary>
    /// Becomes true when a currency owner's currency amount reaches a specified amount.
    /// </summary>
    public class UISCurrencyAmountQuestCondition : QuestCondition
    {

        [HelpBox("Leave Currency Owner Name blank to use Player's Currency Owner.", HelpBoxMessageType.None)]
        public StringField currencyName = new StringField();
        public StringField currencyOwnerName = new StringField();
        [Tooltip("How the currency amount applies to the condition.")]
        public CounterValueConditionMode comparisonMode = CounterValueConditionMode.AtLeast;
        public QuestNumber amount = new QuestNumber(1);
        public int counterIndex = -1; // Optional counter to keep in sync.

        private Currency m_currency = null;
        private CurrencyOwner m_currencyOwner = null;
        private int m_requiredAmount = 1;

        public override string GetEditorName()
        {
            if (StringField.IsNullOrEmpty(currencyName)) return "Check Currency Amount";
            return !StringField.IsNullOrEmpty(currencyOwnerName)
                ? (currencyOwnerName + "'s " + currencyName + " " + comparisonMode + " " + amount.GetValue(quest))
                : ("Player's " + currencyName + " " + comparisonMode + " " + amount.GetValue(quest));
        }

        public override void StartChecking(System.Action trueAction)
        {
            base.StartChecking(trueAction);
            m_requiredAmount = amount.GetValue(quest);
            m_currency = UISUtility.GetCurrency(StringField.GetStringValue(currencyName));
            var currencyOwner = UISUtility.GetCurrencyOwner(StringField.GetStringValue(currencyOwnerName));
            if (IsTrue())
            {
                SetTrue();
            }
            else
            {
                Opsive.Shared.Events.EventHandler.RegisterEvent(EventNames.c_CurrencyCollection_OnUpdate, OnCurrencyChange);
            }
        }

        public override void StopChecking()
        {
            base.StopChecking();
            Opsive.Shared.Events.EventHandler.UnregisterEvent(EventNames.c_CurrencyCollection_OnUpdate, OnCurrencyChange);
        }

        private void OnCurrencyChange()
        {
            if (IsTrue())
            {
                SetTrue();
            }
        }

        private bool IsTrue()
        {
            var counter = (counterIndex != -1) ? quest.GetCounter(counterIndex) : null;
            if (m_currency == null || m_currencyOwner == null)
            {
                if (counter != null) counter.currentValue = 0;
                return false;
            }
            var currentAmount = m_currencyOwner.CurrencyAmount.GetAmountOf(m_currency);
            if (counter != null) counter.currentValue = currentAmount;
            switch (comparisonMode)
            {
                default:
                case CounterValueConditionMode.AtLeast:
                    return currentAmount >= m_requiredAmount;
                case CounterValueConditionMode.AtMost:
                    return currentAmount <= m_requiredAmount;
            }
        }
    }

}
