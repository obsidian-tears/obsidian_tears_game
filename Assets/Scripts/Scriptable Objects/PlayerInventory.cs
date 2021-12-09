using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerInventory : ScriptableObject
{
    public Inventory equipment;
    public Inventory inventory;
    public abstract bool EquipItem(InventoryItem item);

    public abstract bool CanEquipItem(InventoryItem item);
}
