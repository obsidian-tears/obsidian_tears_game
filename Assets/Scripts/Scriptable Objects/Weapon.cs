using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon")]
public class Weapon : InventoryItem
{
    public int baseDamage;
    public float critRatio;
    public EquipmentSize size;
}
