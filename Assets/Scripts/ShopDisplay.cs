using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopDisplay : MonoBehaviour
{
    [SerializeField] SelectedItem selectedItem;
    [SerializeField] MySignal stopShop;
    [SerializeField] Stats playerStats;
    [SerializeField] PlayerInventory playerInventory;
    [SerializeField] InventoryObject shopInventory;
    [SerializeField] Text itemName;
    [SerializeField] Text itemPrice;
    [SerializeField] Text itemDesc;
    [SerializeField] GameObject inventorySlotTemplate;
    [SerializeField] GameObject buyButton;
    [SerializeField] GameObject inventoryPanel;
    void OnEnable()
    {
        itemName.text = "Pick an item";
        itemDesc.text = "";
        itemPrice.text = "";

        selectedItem.item = null;
        //1. instantiate all objects
        foreach (InventoryItem item in shopInventory.myInventory)
        {
            InventorySlot newSlot = Instantiate(inventorySlotTemplate, inventoryPanel.transform.position,
                                                inventoryPanel.transform.rotation).GetComponent<InventorySlot>();
            newSlot.transform.SetParent(inventoryPanel.transform);
            newSlot.Setup(item);
        }
    }

    void DestroyItems() {
        foreach (Transform transform in inventoryPanel.transform)
        {
            GameObject.Destroy(transform.gameObject);
        }
        itemName.text = "";
        itemDesc.text = "";
        itemPrice.text = "";
    }

    public void OnBuy()
    {
        //1. subtract gold and add selectedItem to player inventory
        playerStats.gold.value -= selectedItem.item.value;
        playerInventory.AddItem(selectedItem.item);
        //2. update UI for gold
        //3. tell user "thanks for buying"
    }

    public void OnSelectItem()
    {
        //1. check if buying is possible (you have required items (gold), and capacity)
        //2. enable the buy button
        if (selectedItem.item.value <= playerStats.gold.value)
        {
            buyButton.SetActive(true);
        }
        else
        {
            buyButton.SetActive(false);
        }
        //3. set item name, description, price
        itemName.text = selectedItem.item.name;
        itemPrice.text = selectedItem.item.value.ToString();
        itemDesc.text = selectedItem.item.itemDescription;
    }

    public void OnLeave()
    {
        selectedItem.item = null;
        DestroyItems();
        stopShop.Raise();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
