using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerInventory : ScriptableObject
{
    public Inventory equipment;
    public Inventory armorInventory;
    public Inventory potionsInventory;
    public Inventory weaponsInventory;
    public Inventory keyInventory;
    public Inventory otherInventory;
    public int weaponSlots;
    public int otherSlots;
    public void EquipItem(InventoryItem item)
    {
        equipment.myInventory.Add(item);
        switch (item.type) {
            case ItemType.Armor: 
                armorInventory.myInventory.Remove(item);
                break;
            case ItemType.Potion: 
                potionsInventory.myInventory.Remove(item);
                break;
            case ItemType.Weapon: 
                weaponsInventory.myInventory.Remove(item);
                break;
            case ItemType.KeyItem: 
                keyInventory.myInventory.Remove(item);
                break;
            case ItemType.Other: 
                otherInventory.myInventory.Remove(item);
                break;
        }
    }
    public void UnequipItem(InventoryItem item)
    {
        equipment.myInventory.Remove(item);
        switch (item.type) {
            case ItemType.Armor: 
                armorInventory.myInventory.Add(item);
                break;
            case ItemType.Potion: 
                potionsInventory.myInventory.Add(item);
                break;
            case ItemType.Weapon: 
                weaponsInventory.myInventory.Add(item);
                break;
            case ItemType.KeyItem: 
                keyInventory.myInventory.Add(item);
                break;
            case ItemType.Other: 
                otherInventory.myInventory.Add(item);
                break;
        }
    }

    public abstract bool CanEquipItem(InventoryItem item);
}
