using UnityEngine;
using PixelCrushers.UISSupport;
using Opsive.UltimateInventorySystem.Core.DataStructures;

namespace PixelCrushers.QuestMachine.UISSupport
{

    /// <summary>
    /// Quest action that removes UIS currency.
    /// </summary>
    public class UISRemoveCurrencyQuestAction : QuestAction
    {
        [HelpBox("Leave Currency Owner Name blank to use Player's Currency Owner.", HelpBoxMessageType.None)]
        public StringField currencyName = new StringField();
        public StringField currencyOwnerName = new StringField();
        public QuestNumber amount = new QuestNumber(1);

        public override string GetEditorName()
        {
            if (StringField.IsNullOrEmpty(currencyName)) return "Remove UIS Currency";
            var amountValue = amount.GetValue(quest);
            return !StringField.IsNullOrEmpty(currencyOwnerName)
                ? ("Remove " + amountValue + " " + currencyName + " from " + currencyOwnerName)
                : ("Remove " + amountValue + " " + currencyName + " from Player");
        }

        public override void Execute()
        {
            base.Execute();
            var currency = UISUtility.GetCurrency(StringField.GetStringValue(currencyName));
            if (currency == null) return;
            var currencyOwner = UISUtility.GetCurrencyOwner(StringField.GetStringValue(currencyOwnerName));
            if (currencyOwner == null) return;
            currencyOwner.CurrencyAmount.RemoveCurrency(currency, amount.GetValue(quest));
        }

    }

}
