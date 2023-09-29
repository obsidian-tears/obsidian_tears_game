using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryController : MonoBehaviour
{
    int selectedPageIndex;
    [SerializeField] InventoryObject displayInventory;
    [SerializeField] PlayerInventory inventory;
    [SerializeField] SelectedItem selectedItem;
    [SerializeField] GameObject equipmentPanel;
    [SerializeField] GameObject descriptionPanel;
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] TextMeshProUGUI heading;
    [SerializeField] InventorySlot inventorySlotTemplate;
    [SerializeField] MySignal equipItemSignal;
    void Start()
    {
        selectedPageIndex = 0;
        SwitchInventory(inventory.armorInventory);
    }

    public void OnEquipItem()
    {
        inventory.equipment.myInventory.Add(selectedItem.item);
        bool willDestroy = false;
        switch (selectedItem.item.type)
        {
            case ItemType.Armor:
                inventory.armorInventory.myInventory.Remove(selectedItem.item);
                if (selectedPageIndex == 0) willDestroy = true;
                break;
            case ItemType.Weapon:
                inventory.weaponsInventory.myInventory.Remove(selectedItem.item);
                if (selectedPageIndex == 1) willDestroy = true;
                break;
            case ItemType.Potion:
                inventory.potionsInventory.myInventory.Remove(selectedItem.item);
                if (selectedPageIndex == 2) willDestroy = true;
                break;
            case ItemType.Other:
                inventory.otherInventory.myInventory.Remove(selectedItem.item);
                if (selectedPageIndex == 3) willDestroy = true;
                break;
            case ItemType.KeyItem:
                inventory.keyInventory.myInventory.Remove(selectedItem.item);
                if (selectedPageIndex == 4) willDestroy = true;
                break;
        }
        // todo clean up
        if (willDestroy)
            foreach (Transform transform in inventoryPanel.transform)
            {
                if (transform.GetComponent<InventorySlot>().inventoryItem.itemName == selectedItem.item.itemName)
                    GameObject.Destroy(transform.gameObject);
            }
        descriptionPanel.GetComponent<ItemDetails>().Clear();
        equipItemSignal.Raise();
    }

    public void OnUnequipItem()
    {
        bool willAdd = false;
        inventory.equipment.myInventory.Remove(selectedItem.item);
        switch (selectedItem.item.type)
        {
            case ItemType.Armor:
                inventory.armorInventory.myInventory.Add(selectedItem.item);
                if (selectedPageIndex == 0) willAdd = true;
                break;
            case ItemType.Weapon:
                inventory.weaponsInventory.myInventory.Add(selectedItem.item);
                if (selectedPageIndex == 1) willAdd = true;
                break;
            case ItemType.Potion:
                inventory.potionsInventory.myInventory.Add(selectedItem.item);
                if (selectedPageIndex == 2) willAdd = true;
                break;
            case ItemType.Other:
                inventory.otherInventory.myInventory.Add(selectedItem.item);
                if (selectedPageIndex == 3) willAdd = true;
                break;
            case ItemType.KeyItem:
                inventory.keyInventory.myInventory.Add(selectedItem.item);
                if (selectedPageIndex == 4) willAdd = true;
                break;
        }
        // todo clean up
        if (willAdd)
        {
            InventorySlot newSlot = Instantiate(inventorySlotTemplate, inventoryPanel.transform.position,
                                                inventoryPanel.transform.rotation).GetComponent<InventorySlot>();
            newSlot.transform.SetParent(inventoryPanel.transform);
            newSlot.Setup(selectedItem.item);
        }
        descriptionPanel.GetComponent<ItemDetails>().Clear();
        equipItemSignal.Raise();
    }

    void SwitchInventory(InventoryObject newInventory)
    {
        displayInventory = newInventory;
        DestroyInventory();
        PopulateInventory(newInventory);
    }
    void DestroyInventory()
    {
        foreach (Transform transform in inventoryPanel.transform)
        {
            GameObject.Destroy(transform.gameObject);
        }
    }
    void PopulateInventory(InventoryObject newInventory)
    {
        foreach (InventoryItem item in newInventory.myInventory)
        {
            InventorySlot newSlot = Instantiate(inventorySlotTemplate, inventoryPanel.transform.position,
                                                inventoryPanel.transform.rotation).GetComponent<InventorySlot>();
            newSlot.transform.SetParent(inventoryPanel.transform);
            newSlot.Setup(item);
        }
    }

    public void PageLeft()
    {
        selectedPageIndex--;
        selectedPageIndex %= 5;
        SetPage(selectedPageIndex);
    }

    public void PageRight()
    {
        selectedPageIndex++;
        selectedPageIndex %= 5;
        SetPage(selectedPageIndex);
    }

    void SetPage(int newIndex)
    {
        switch (newIndex)
        {
            case 0:
                heading.text = "Armor";
                SwitchInventory(inventory.armorInventory);
                break;
            case 1:
                heading.text = "Weapons";
                SwitchInventory(inventory.weaponsInventory);
                break;
            case 2:
                heading.text = "Potions";
                SwitchInventory(inventory.potionsInventory);
                break;
            case 3:
                heading.text = "Misc";
                SwitchInventory(inventory.otherInventory);
                break;
            case 4:
                heading.text = "Key Items";
                SwitchInventory(inventory.keyInventory);
                break;
        }
    }
}
