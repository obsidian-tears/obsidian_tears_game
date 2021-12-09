using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Text itemNameText;
    public InventoryItem item;
    public void Setup(InventoryItem newItem) {
        item = newItem;
        itemNameText.text = newItem.itemName;
    }
}