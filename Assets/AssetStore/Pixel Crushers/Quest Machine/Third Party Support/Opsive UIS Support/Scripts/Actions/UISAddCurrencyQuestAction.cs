using UnityEngine;
using PixelCrushers.UISSupport;
using Opsive.UltimateInventorySystem.Core.DataStructures;

namespace PixelCrushers.QuestMachine.UISSupport
{

    /// <summary>
    /// Quest action that adds UIS currency.
    /// </summary>
    public class UISAddCurrencyQuestAction : QuestAction
    {
        [HelpBox("Leave Currency Owner Name blank to use Player's Currency Owner.", HelpBoxMessageType.None)]
        public StringField currencyName = new StringField();
        public StringField currencyOwnerName = new StringField();
        public QuestNumber amount = new QuestNumber(1);

        public override string GetEditorName()
        {
            if (StringField.IsNullOrEmpty(currencyName)) return "Add UIS Currency";
            var amountValue = amount.GetValue(quest);
            return !StringField.IsNullOrEmpty(currencyOwnerName)
                ? ("Add " + amountValue + " " + currencyName + " to " + currencyOwnerName)
                : ("Add " + amountValue + " " + currencyName + " to Player");
        }

        public override void Execute()
        {
            base.Execute();
            var currency = UISUtility.GetCurrency(StringField.GetStringValue(currencyName));
            if (currency == null) return;
            var currencyOwner = UISUtility.GetCurrencyOwner(StringField.GetStringValue(currencyOwnerName));
            if (currencyOwner == null) return;
            currencyOwner.CurrencyAmount.AddCurrency(currency, amount.GetValue(quest));
        }

    }

}
