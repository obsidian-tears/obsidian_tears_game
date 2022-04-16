using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { Armor, Weapon, Potion, KeyItem, Other }
public enum EquipmentType { Weapon, Helmet, Ring, Shield, BodyArmor }
public enum EquipmentSize {Small, Medium, Large, Huge}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public Sprite itemImage;
    public ItemType type;
    public EquipmentType subType;
    public int numberHeld;
    public int value;
    public bool isUsable;
    public bool isEquipable;
    public bool isKeyItem;
    public bool canUseInBattle;
}
