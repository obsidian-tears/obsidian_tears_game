using System;
using Opsive.UltimateInventorySystem.Exchange;
using PixelCrushers;

public class InventoryCurrencySaver : Saver
{
    [Serializable]
    public class Currency { public int amount; }

    public override string RecordData()
    {
        var currentCurrency = new Currency();
        var currencyOwner = GetComponent<CurrencyOwner>();
        currentCurrency.amount = currencyOwner.CurrencyAmount.GetCurrencyAmountAt(0).Amount;
        return SaveSystem.Serialize(currentCurrency);
    }

    public override void ApplyData(string s)
    {
        if (string.IsNullOrEmpty(s)) return;

        var savedCurrency = SaveSystem.Deserialize<Currency>(s);
        if (savedCurrency == null) return;

        var currencyOwner = GetComponent<CurrencyOwner>();
        var currencyAmount = currencyOwner.CurrencyAmount.GetCurrencyAmountAt(0);
        currencyOwner.RemoveCurrency(currencyAmount.Currency, currencyAmount.Amount);
        currencyOwner.AddCurrency("Gold", savedCurrency.amount);
    }
}
