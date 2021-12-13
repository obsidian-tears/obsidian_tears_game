using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDetails : MonoBehaviour
{
    [SerializeField] SelectedItem selectedItem;
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] Button equipButton;
    [SerializeField] Button unequipButton;
    [SerializeField] Button useButton;
    [SerializeField] PlayerInventory inventory;
    void Start()
    {
        selectedItem.item = null;
        description.text = "";
        itemName.text = "";
    }

    public void Reset()
    {
        equipButton.interactable = true;
        unequipButton.interactable = true;
        useButton.interactable = true;
        equipButton.gameObject.SetActive(false);
        unequipButton.gameObject.SetActive(false);
        useButton.gameObject.SetActive(false);
    }
    public void Clear()
    {
        selectedItem.item = null;
        itemName.text = "";
        description.text = "";
        equipButton.gameObject.SetActive(false);
        unequipButton.gameObject.SetActive(false);
        useButton.gameObject.SetActive(false);
    }
    public void UpdatePage()
    {
        Reset();
        if (selectedItem.item)
            itemName.text = selectedItem.item.itemName;
        description.text = selectedItem.item.itemDescription;
        if (inventory.equipment.myInventory.Contains(selectedItem.item))
        {
            unequipButton.gameObject.SetActive(true);
        }
        else if (selectedItem.item.isEquipable)
        {
            equipButton.gameObject.SetActive(true);
            if (!inventory.CanEquipItem(selectedItem.item))
            {
                equipButton.interactable = false;
            }
            else
            {
                equipButton.interactable = true;
            }
        }
        else
        {
            equipButton.gameObject.SetActive(false);
        }
        if (selectedItem.item.isUsable)
        {
            useButton.gameObject.SetActive(true);
        }
        else
        {
            useButton.gameObject.SetActive(false);
        }
    }
}
