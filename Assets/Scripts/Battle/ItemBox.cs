using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    [SerializeField] GameObject itemSelector;
    [SerializeField] InventoryObject playerInventory;
    [SerializeField] Color selectedColor;
    [SerializeField] GameObject itemSlotTemplate;
    List<ItemSlot> itemSlots = new List<ItemSlot>();

    void Start()
    {
        InstantiateSlots();
    }

    void InstantiateSlots()
    {
        if (playerInventory)
        {
            foreach (InventoryItem item in playerInventory.myInventory)
            {
                if (item.canUseInBattle)
                {
                    ItemSlot newSlot = Instantiate(itemSlotTemplate, transform.position, transform.rotation)
                        .GetComponent<ItemSlot>();
                    newSlot.transform.SetParent(itemSelector.transform);
                    newSlot.Setup(item);
                    itemSlots.Add(newSlot);
                }
            }
            if (itemSlots.Count > 0)
                itemSlots[0].itemNameText.color = selectedColor;
        }
    }

    public void UpdateItemSelection(int indexCount)
    {
        if (itemSlots.Count == 0) return;
        int selectedIndex = indexCount % itemSlots.Count;

        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (i == selectedIndex)
            {
                itemSlots[i].itemNameText.color = selectedColor;
            }
            else
            {
                itemSlots[i].itemNameText.color = Color.black;
            }
        }
    }
    public bool UseItem(int selectedItemIndex)
    {

        return true;
    }

    public void EnableItemSelector()
    {
        this.gameObject.SetActive(true);
    }
    public void DisableItemSelector()
    {
        this.gameObject.SetActive(false);
    }
}


