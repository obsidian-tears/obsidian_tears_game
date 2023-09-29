using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using Opsive.UltimateInventorySystem.Exchange;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactGiveItems : MonoBehaviour
{
    public void GiveItems(uint itemDefinitionId, int goldAmount)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Inventory inv = player.GetComponent<Inventory>();
        var itemDefinition = InventorySystemManager.GetItemDefinition(itemDefinitionId);
        inv.AddItem(itemDefinition, 1);

        var currencyOwner = inv.GetCurrencyComponent<CurrencyCollection>() as CurrencyOwner;
        var ownerCurrencyCollection = currencyOwner.CurrencyAmount;

        var gold = InventorySystemManager.GetCurrency("Gold");

        ownerCurrencyCollection.AddCurrency(gold, goldAmount);
    }

}
