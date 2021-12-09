using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterInventory : PlayerInventory
{
    public override bool EquipItem(InventoryItem item) {
        return true;
    }

    public override bool CanEquipItem(InventoryItem item)
    {
        return true;
    }
}
