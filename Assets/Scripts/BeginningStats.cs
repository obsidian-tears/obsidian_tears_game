using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.DataStructures;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using Opsive.UltimateInventorySystem.Exchange;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeginningStats : MonoBehaviour
{
    public GameObject player;

    public int goldAmount;

    public int level;
    public int xp;
    public int xpToLevelUp;
    public int pointsRemaining;
    public int healthBase;
    public int healthTotal;
    public int healthMax;
    public int magicBase;
    public int magicTotal;
    public int magicMax;
    public int attackBase;
    public int attackTotal;
    public int magicPowerBase;
    public int magicPowerTotal;
    public int defenseBase;
    public int defenseTotal;
    public int speedBase;
    public int speedTotal;
    public float criticalHitProbability;

    public float timer;



    private void Start()
    {
        timer = 0f;
    }

    private void Update()
    {
        while (timer < 5f)
        {
            //Define variables
            Inventory startingInventory = gameObject.GetComponent<Inventory>();

            Inventory playerInventory = player.GetComponent<Inventory>();
            CharStats playerStats = player.GetComponent<CharStats>();
            CurrencyOwner currencyOwner = player.GetComponent<CurrencyOwner>();

            playerStats.level = level;
            playerStats.xp = xp;
            playerStats.xpToLevelUp = xpToLevelUp;
            playerStats.pointsRemaining = pointsRemaining;
            playerStats.healthBase = healthBase;
            playerStats.healthTotal = healthTotal;
            playerStats.healthMax = healthMax;
            playerStats.magicBase = magicBase;
            playerStats.magicTotal = magicTotal;
            playerStats.magicMax = magicMax;
            playerStats.attackBase = attackBase;
            playerStats.attackTotal = attackTotal;
            playerStats.magicPowerBase = magicPowerBase;
            playerStats.magicPowerTotal = magicPowerTotal;
            playerStats.defenseBase = defenseBase;
            playerStats.defenseTotal = defenseTotal;
            playerStats.speedBase = speedBase;
            playerStats.speedTotal = speedTotal;



            //Add the proper amount of currency to the player
            CurrencyCollection currencyCollection = currencyOwner.CurrencyAmount;

            CurrencyAmount[] currencies =
                currencyCollection.GetCurrencyAmounts().ToArray();
            foreach (CurrencyAmount currencyAmount in currencies)
            {
                currencyOwner
                    .RemoveCurrency(currencyAmount.Currency, currencyAmount.Amount);
            }
            currencyOwner.AddCurrency("Gold", goldAmount);


            //Get all the items from the starting inventory
            List<string> myItems = new List<string>();
            List<string> equippedItems = new List<string>();


            foreach (ItemCollection itemCol in startingInventory.ItemCollectionsReadOnly)
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









            //Add all the items from the starting inventory into the player inventory
            playerInventory.RemoveAllItems();


            foreach (ItemCollection itemCol in playerInventory.ItemCollectionsReadOnly)
            {
                if (itemCol.Name == "MainItemCollection")
                {
                    foreach (string itemDefString in myItems)
                    {
                        uint defId = uint.Parse(itemDefString);
                        ItemDefinition itemDef =
                            InventorySystemManager.GetItemDefinition(defId);
                        itemCol.AddItem(itemDef, 1, false);
                    }
                }
                else
                {
                    foreach (string itemDefString in equippedItems)
                    {
                        uint defId = uint.Parse(itemDefString);
                        ItemDefinition itemDef =
                            InventorySystemManager.GetItemDefinition(defId);
                        itemCol.AddItem(itemDef, 1, false);
                    }
                }
            }

            timer += Time.deltaTime;
        }
    }
}
