using System.Collections;
using System.Collections.Generic;
using GameManagers;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using Opsive.UltimateInventorySystem.UI.Panels.Hotbar;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool inventoryWasInit = false;

    public ItemSlotCollection itemSlotCollection;

    public ItemSlotCollectionView slotCollectionView;

    private void Awake()
    {
        Debug.Log("hola meme");

        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        DontDestroyOnLoad(this);

        SceneManager.sceneLoaded += (arg0, mode) =>
        {
            if (!inventoryWasInit) return;

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

           // playerInventory.RemoveItemCollection(equippedCol);

            playerInventory.AddItemCollection(itemSlotCollection);
            Debug.Log("hola maine 7");

            playerInventory.UpdateInventory();
            Debug.Log("hola maine 8");


            if (slotCollectionView == null  || GameUIManager.Exist)
            {
                slotCollectionView = GameUIManager.Instance.gameObject.GetComponentInChildren<ItemSlotCollectionView>();
                Debug.Log("obtengo el ite slot collection view");
              
                    Debug.Log("le asignmo");
                    slotCollectionView.ItemSlotSet = itemSlotCollection.ItemSlotSet;

                    Debug.Log("slotCollectionView.ItemSlotSet" + slotCollectionView.ItemSlotSet);
                    Debug.Log("itemSlotCollection.ItemSlotSet" + itemSlotCollection.ItemSlotSet);

               
            }
            // Debug.Log("_itemSlotSets.Count: " + _itemSlotSets.Count);



        };
    }
}
