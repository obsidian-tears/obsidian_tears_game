using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { Armor, Weapon, Potion, KeyItem, Other }
public enum EquipmentType { Weapon, Helmet, Ring, Shield, BodyArmor, Shoes }

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public Sprite itemImage;
    public int numberHeld;
    public bool isUsable;
    public bool isEquipable;
    public bool isKeyItem;
    public bool canUseInBattle;
}
