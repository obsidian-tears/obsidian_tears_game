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
            Debug.Log("hola maine 1");
            if (!inventoryWasInit) return;
            Debug.Log("hola maine 2");

            var playerInventory = FindObjectOfType<Inventory>();
            Debug.Log("hola maine 3");

            if (!playerInventory) return;
            Debug.Log("hola maine 4");

            var equippedCol = playerInventory.GetItemCollection("Equipped");
            Debug.Log("hola maine 5");

            if (equippedCol != null)
            {
                playerInventory.RemoveItemCollection(equippedCol);
            }
            if (playerInventory == null)
            {
                Debug.Log("es nulo el playerInventory ");

            }

           // playerInventory.RemoveItemCollection(equippedCol);
            Debug.Log("hola maine 6");

            playerInventory.AddItemCollection(itemSlotCollection);
            Debug.Log("hola maine 7");

            playerInventory.UpdateInventory();
            Debug.Log("hola maine 8");


        };
    }
}
