using System.Collections;
using System.Collections.Generic;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool inventoryWasInit = false;

    public ItemSlotCollection itemSlotCollection;
    
    private void Awake()
    {
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
            playerInventory.RemoveItemCollection(equippedCol);
            playerInventory.AddItemCollection(itemSlotCollection);
            
            playerInventory.UpdateInventory();
        };
    }
}
