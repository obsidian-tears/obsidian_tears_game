using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.DataStructures;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using Opsive.UltimateInventorySystem.Demo.UI.Menus.Main.Inventory;
using Opsive.UltimateInventorySystem.Equipping;
using Opsive.UltimateInventorySystem.Exchange;
using PixelCrushers;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCurrencySaver : Saver
{
    [Serializable]
    public class Data
    {
        public string[] items;

        public string[] equippedItems;

        public int currency;
    }

    public override string RecordData()
    {
        var inventory = GetComponent<Inventory>();
        var currencyOwner = GetComponent<CurrencyOwner>();
        var data = new Data();

        List<string> myItems = new List<string>();
        List<string> equippedItems = new List<string>();

        foreach (ItemCollection itemCol in inventory.ItemCollectionsReadOnly)
        {
            if (itemCol.Name == "MainItemCollection")
            {
                foreach (ItemStack itemStack in itemCol.GetAllItemStacks())
                {
                    int i = 0;
                    while (i < itemStack.Amount)
                    {
                        myItems
                            .Add(itemStack.Item.ItemDefinition.ID.ToString());
                        i++;
                    }
                }
            }
            else
            {
                foreach (ItemStack itemStack in itemCol.GetAllItemStacks())
                {
                    int i = 0;
                    while (i < itemStack.Amount)
                    {
                        equippedItems
                            .Add(itemStack.Item.ItemDefinition.ID.ToString());
                        i++;
                    }
                }
            }
        }

        data.items = myItems.ToArray();
        data.equippedItems = equippedItems.ToArray();

        CurrencyCollection currencyCollection = currencyOwner.CurrencyAmount;
        CurrencyAmount[] currencies =
            currencyCollection.GetCurrencyAmounts().ToArray();
        foreach (CurrencyAmount currencyAmount in currencies)
        {
            data.currency = currencyAmount.Amount;
        }

        return SaveSystem.Serialize(data);
    }

    public override void ApplyData(string s)
    {
        if (string.IsNullOrEmpty(s)) return;
        var data = SaveSystem.Deserialize<Data>(s);
        if (data == null) return;

        var inventory = GetComponent<Inventory>();
        var currencyOwner = GetComponent<CurrencyOwner>();

        inventory.RemoveAllItems();

        Equipper equipper = GetComponent<Equipper>();

        foreach (ItemCollection itemCol in inventory.ItemCollectionsReadOnly)
        {
            if (itemCol.Name == "MainItemCollection")
            {
                foreach (string itemDefString in data.items)
                {
                    uint defId = uint.Parse(itemDefString);
                    ItemDefinition itemDef =
                        InventorySystemManager.GetItemDefinition(defId);
                    itemCol.AddItem(itemDef, 1, false);
                }
            }
            else
            {
                foreach (string itemDefString in data.equippedItems)
                {
                    Debug.Log("Equipping an item");
                    uint defId = uint.Parse(itemDefString);
                    ItemDefinition itemDef =
                        InventorySystemManager.GetItemDefinition(defId);
                    itemCol.AddItem(itemDef, 1, false);
                }
            }
        }

        CurrencyCollection currencyCollection = currencyOwner.CurrencyAmount;

        CurrencyAmount[] currencies =
            currencyCollection.GetCurrencyAmounts().ToArray();
        foreach (CurrencyAmount currencyAmount in currencies)
        {
            currencyOwner
                .RemoveCurrency(currencyAmount.Currency, currencyAmount.Amount);
        }

        currencyOwner.AddCurrency("Gold", data.currency);
    }
}
