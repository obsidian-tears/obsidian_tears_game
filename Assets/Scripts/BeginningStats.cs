using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.DataStructures;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using Opsive.UltimateInventorySystem.Exchange;
using System.Collections;
using System.Collections.Generic;
using GameManagers;
using Opsive.UltimateInventorySystem.UI.Panels.Hotbar;
using UnityEngine;

public enum InitialClasses
{
    MAGE,
    FIGTHER,
    RANGER
}

public class BeginningStats : MonoBehaviour
{
    public GameObject player;
    public Animator animPlayer;
    public CharStats charStats;
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
    private string selectedClass;


    [SerializeField] private InitialClasses _initialClass;
    [SerializeField] private List<ItemSlotCollection> _itemCollection;
    [SerializeField] private List<ItemSlotSet> _itemSlotSets;

    public ItemSlotCollectionView slotCollectionView;

    private void Awake()
    {
        if (player != null)
        {
            animPlayer.SetFloat("moveX", 1);
            // player.transform.Rotate(0, 180, 0); //rota la camara y no sirve
        }

        //Inventory startingInventory = gameObject.GetComponent<Inventory>();
        //
        if (GameManager.Instance.inventoryWasInit)
        {
            return;
        }

        var playerInventory = player.GetComponent<Inventory>();

        playerInventory.RemoveItemCollection(_itemCollection[(int)_initialClass]);
        playerInventory.AddItemCollection(_itemCollection[(int)_initialClass]);
        //  startingInventory.AddItemCollection(_itemCollection[(int)_initialClass]);

        playerInventory.RemoveItemCollection(_itemCollection[(int)_initialClass]);
        playerInventory.UpdateInventory();
        //  startingInventory.UpdateInventory();

        if (slotCollectionView == null && GameUIManager.Exist)
        {
            slotCollectionView = GameUIManager.Instance.gameObject.GetComponentInChildren<ItemSlotCollectionView>();
        }

        slotCollectionView.ItemSlotSet = _itemSlotSets[(int)_initialClass];
        slotCollectionView.SetItemViewSlotRestrictions();

        GameManager.Instance.inventoryWasInit = true;
    }

//nice
    private void Start()
    {
        timer = 0f;

        CharStats charStats = player.GetComponent<CharStats>();
        charStats.characterClass = _initialClass.ToString();
        Debug.Log(charStats.characterClass);
    }

    // private void Update()
    // {
    //     while (timer < 5f)
    //     {
    //         //Define variables
    //         //Inventory startingInventory = gameObject.GetComponent<Inventory>();
    //
    //         Inventory playerInventory = player.GetComponent<Inventory>();
    //         CharStats playerStats = player.GetComponent<CharStats>();
    //         CurrencyOwner currencyOwner = player.GetComponent<CurrencyOwner>();
    //
    //         playerStats.level = level;
    //         playerStats.xp = xp;
    //         playerStats.xpToLevelUp = xpToLevelUp;
    //         playerStats.pointsRemaining = pointsRemaining;
    //         playerStats.healthBase = healthBase; //  ------------ change playertype 
    //         playerStats.healthTotal = healthTotal;
    //         playerStats.healthMax = healthMax; //    ------------ change playertype
    //         playerStats.magicBase = magicBase; //    ------------ change playertype
    //         playerStats.magicTotal = magicTotal;
    //         playerStats.magicMax = magicMax; //    ------------- change playertype
    //         playerStats.attackBase = attackBase; // ------------- change playertype
    //         playerStats.attackTotal = attackTotal;
    //         playerStats.magicPowerBase = magicPowerBase; // ----- change playertype
    //         playerStats.magicPowerTotal = magicPowerTotal;
    //         playerStats.defenseBase = defenseBase; // ----------- change playertype
    //         playerStats.defenseTotal = defenseTotal;
    //         playerStats.speedBase = speedBase; //   ------------ change playertype
    //         playerStats.speedTotal = speedTotal;
    //
    //
    //         //Add the proper amount of currency to the player
    //         CurrencyCollection currencyCollection = currencyOwner.CurrencyAmount;
    //
    //         CurrencyAmount[] currencies =
    //             currencyCollection.GetCurrencyAmounts().ToArray();
    //         foreach (CurrencyAmount currencyAmount in currencies)
    //         {
    //             currencyOwner
    //                 .RemoveCurrency(currencyAmount.Currency, currencyAmount.Amount);
    //         }
    //
    //         currencyOwner.AddCurrency("Gold", goldAmount);
    //
    //
    //         //Get all the items from the starting inventory
    //         List<string> myItems = new List<string>();
    //         List<string> equippedItems = new List<string>();
    //
    //
    //         // foreach (ItemCollection itemCol in startingInventory.ItemCollectionsReadOnly)
    //         // {
    //         //     if (itemCol.Name == "MainItemCollection")
    //         //     {
    //         //         foreach (ItemStack itemStack in itemCol.GetAllItemStacks())
    //         //         {
    //         //             int i = 0;
    //         //             while (i < itemStack.Amount)
    //         //             {
    //         //                 myItems
    //         //                     .Add(itemStack.Item.ItemDefinition.ID.ToString());
    //         //                 i++;
    //         //             }
    //         //         }
    //         //     }
    //         //     else
    //         //     {
    //         //         foreach (ItemStack itemStack in itemCol.GetAllItemStacks())
    //         //         {
    //         //             int i = 0;
    //         //             while (i < itemStack.Amount)
    //         //             {
    //         //                 equippedItems
    //         //                     .Add(itemStack.Item.ItemDefinition.ID.ToString());
    //         //                 i++;
    //         //             }
    //         //         }
    //         //     }
    //         // }
    //
    //
    //         //Add all the items from the starting inventory into the player inventory
    //         playerInventory.RemoveAllItems();
    //
    //
    //         foreach (ItemCollection itemCol in playerInventory.ItemCollectionsReadOnly)
    //         {
    //             if (itemCol.Name == "MainItemCollection")
    //             {
    //                 foreach (string itemDefString in myItems)
    //                 {
    //                     uint defId = uint.Parse(itemDefString);
    //                     ItemDefinition itemDef =
    //                         InventorySystemManager.GetItemDefinition(defId);
    //                     itemCol.AddItem(itemDef, 1, false);
    //                 }
    //             }
    //             else
    //             {
    //                 foreach (string itemDefString in equippedItems)
    //                 {
    //                     uint defId = uint.Parse(itemDefString);
    //                     ItemDefinition itemDef =
    //                         InventorySystemManager.GetItemDefinition(defId);
    //                     itemCol.AddItem(itemDef, 1, false);
    //                 }
    //             }
    //         }
    //
    //         timer += Time.deltaTime;
    //     }
    // }


    public void SetInitiaStats(PlayerType selectedClass)
    {
        switch (selectedClass)
        {
            case PlayerType.Fighter:
                SetFighterStats();
                break;
            case PlayerType.Wizard:
                SetWizardStats();
                break;
            case PlayerType.Ranger:
                SetRangerStats();
                break;
            case PlayerType.Barbarian:
                SetBarbarianStats();
                break;
            default:
                break;
        }
    }


    private void SetFighterStats()
    {
        healthBase = 20;
        healthMax = 100;
        magicBase = 5;
        magicMax = 30;
        attackBase = 5;
        magicPowerBase = 5;
        defenseBase = 7;
        speedBase = 4;
        criticalHitProbability = 0.5f;
    }


    private void SetWizardStats()
    {
        healthBase = 12;
        healthMax = 100;
        magicBase = 25;
        magicMax = 100;
        attackBase = 4;
        magicPowerBase = 1;
        defenseBase = 4;
        speedBase = 5;
        criticalHitProbability = 0.5f;

        //
    }

    private void SetRangerStats()
    {
        healthBase = 15;
        healthMax = 100;
        magicBase = 8;
        magicMax = 40;
        attackBase = 5;
        magicPowerBase = 7;
        defenseBase = 4;
        speedBase = 6;
        criticalHitProbability = 0.6f;
        //bow equipped as primary weapon, have 40% chance to double shoot.
    }

    private void SetBarbarianStats()
    {
    }
}