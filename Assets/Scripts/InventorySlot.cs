using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public InventoryItem inventoryItem;
    public SelectedItem selectedItem;
    [SerializeField] Image image;
    [SerializeField] MySignal signal;

    public void Setup(InventoryItem item)
    {
        inventoryItem = item;
        image.preserveAspect = true;
        transform.localScale = new Vector3(1,1,1);
        image.sprite = item.itemImage;
    }
    public void Select() {
        selectedItem.item = inventoryItem;
        signal.Raise();
    }
}
