using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameManagers;
using Newtonsoft.Json;
using Opsive.Shared.Utility;
using Opsive.UltimateInventorySystem.Core.DataStructures;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using Opsive.UltimateInventorySystem.UI.Panels.Hotbar;
using PixelCrushers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Saver 
{
    public static GameManager Instance;

    public bool inventoryWasInit = false;
    
    public ItemSlotCollection itemSlotCollection;
    public ItemSlotSet ItemSlotSet;

    public ItemSlotCollectionView slotCollectionView;

    [SerializeField] private List<ItemSlotCollection> _itemCollection;

    [SerializeField] private InitialClasses _initialClass;


    public override void ApplyData(string s)
    {
        Debug.Log("apply data");
        var dataBase = JsonConvert.DeserializeObject<List<string>>(s);
        _initialClass = JsonConvert.DeserializeObject<InitialClasses>(dataBase[2]);

       
            
            itemSlotCollection = _itemCollection[(int)_initialClass];


            var playerInventory = FindObjectOfType<Inventory>();

            if (!playerInventory) return;

            var equippedCol = playerInventory.GetItemCollection("Equipped");

            if (equippedCol != null)
            {
                playerInventory.RemoveItemCollection(equippedCol);
            }

            if (playerInventory == null)
            {
                Debug.Log("es nulo el playerInventory ");
            }

            playerInventory.AddItemCollection(itemSlotCollection);

            playerInventory.UpdateInventory();


            if (slotCollectionView == null || GameUIManager.Exist)
            {
                slotCollectionView = GameUIManager.Instance.gameObject.GetComponentInChildren<ItemSlotCollectionView>();

                if (slotCollectionView == null)
                {
                    slotCollectionView = Resources.FindObjectsOfTypeAll<ItemSlotCollectionView>().First();
                    Debug.Log("Null?");
                }

                slotCollectionView.ItemSlotSet = itemSlotCollection.ItemSlotSet;

                Debug.Log("slotCollectionView.ItemSlotSet" + slotCollectionView.ItemSlotSet);
                Debug.Log("itemSlotCollection.ItemSlotSet" + itemSlotCollection.ItemSlotSet);
            }


        var mainCol = JsonConvert.DeserializeObject<ItemCollection>(dataBase[0]);
        var equipCol = JsonConvert.DeserializeObject<ItemCollection>(dataBase[1]);
        playerInventory.GetItemCollection(ItemCollectionPurpose.Main).RemoveAll();
        playerInventory.GetItemCollection(ItemCollectionPurpose.Equipped).RemoveAll();

       
        playerInventory.GetItemCollection(ItemCollectionPurpose.Main).GiveAllItems(mainCol);
        playerInventory.GetItemCollection(ItemCollectionPurpose.Equipped).GiveAllItems(equipCol);



    }

    public override string RecordData()
    {
        var DataList = new List<string>();
        var playerInventory = FindObjectOfType<Inventory>();
        //ItemInfo[] itemInfos = 
        DataList.Add(JsonConvert.SerializeObject(playerInventory.GetItemCollection(ItemCollectionPurpose.Main)));
        DataList.Add(JsonConvert.SerializeObject(playerInventory.GetItemCollection(ItemCollectionPurpose.Equipped)));
        DataList.Add(JsonConvert.SerializeObject(_initialClass));
        return JsonConvert.SerializeObject(DataList);
    }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Debug.Log("awake ");



        SaveSystem.RegisterSaver(this);

        string charClass = ICConnect.characterClass;

        VerifyClassICConnect(charClass);

        itemSlotCollection = _itemCollection[(int)_initialClass];

        Instance = this;

       // if (!inventoryWasInit) return;

        var playerInventory = FindObjectOfType<Inventory>();

        if (!playerInventory) return;

        var equippedCol = playerInventory.GetItemCollection("Equipped");

        if (equippedCol != null)
        {
            playerInventory.RemoveItemCollection(equippedCol);
        }

        if (playerInventory == null)
        {
            Debug.Log("es nulo el playerInventory ");
        }

        playerInventory.AddItemCollection(itemSlotCollection);

        playerInventory.UpdateInventory();


        if (slotCollectionView == null || GameUIManager.Exist)
        {
            slotCollectionView = GameUIManager.Instance.gameObject.GetComponentInChildren<ItemSlotCollectionView>();

            if (slotCollectionView == null)
            {
                slotCollectionView = Resources.FindObjectsOfTypeAll<ItemSlotCollectionView>().First();
                Debug.Log("Null?");
            }

            slotCollectionView.ItemSlotSet = itemSlotCollection.ItemSlotSet;

            Debug.Log("slotCollectionView.ItemSlotSet" + slotCollectionView.ItemSlotSet);
            Debug.Log("itemSlotCollection.ItemSlotSet" + itemSlotCollection.ItemSlotSet);

        }
    }

    private void OnDestroy()
    {
        SaveSystem.UnregisterSaver(this);
    }



    private void VerifyClassICConnect(string charClass)
    {
        if (charClass == "MAGE")
        {
            _initialClass = InitialClasses.MAGE;

        }
        if (charClass == "FIGHTER")
        {
            _initialClass = InitialClasses.FIGHTER;
        }
        if (charClass == "RANGER")
        {
            _initialClass = InitialClasses.RANGER;
        }
        if (charClass == "Default" || charClass == null)
        {
            _initialClass = InitialClasses.FIGHTER;
        }




    }
}