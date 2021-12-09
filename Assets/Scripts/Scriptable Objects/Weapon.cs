using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponSize {Small, Medium, Large, Huge}

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon")]
public class Weapon : InventoryItem
{
    public EquipmentType equipmentType;
    public int baseDamage;
    public float critRatio;
    public WeaponSize size;
}
