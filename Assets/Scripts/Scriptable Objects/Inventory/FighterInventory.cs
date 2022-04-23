using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "NewPlayerInventory", menuName = "Inventory/FighterInventory")]
public class FighterInventory : PlayerInventory
{
    public override bool CanEquipItem(InventoryItem item)
    {
        if (item.type == ItemType.Armor)
        {
            Armor armor = (Armor)item;
            // fail if huge armor
            if (armor.size == EquipmentSize.Huge) return false;
            // fail if already wearing helmet
            if (armor.subType == EquipmentType.Helmet &&
                equipment.myInventory.Any((_item) => _item.subType == EquipmentType.Helmet)) return false;
            // fail if already wearing body armor
            if (armor.subType == EquipmentType.BodyArmor &&
                equipment.myInventory.Any((_item) => _item.subType == EquipmentType.BodyArmor)) return false;
            // fail if already wearing shield
            if (armor.subType == EquipmentType.Shield &&
                equipment.myInventory.Any((_item) => _item.subType == EquipmentType.Shield)) return false;

        }
        // fail if more than otherslots available
        else if (item.type == ItemType.Other)
        {
            if (equipment.myInventory.Count((_item) => _item.type == ItemType.Other) >= otherSlots) return false;
        }
        else if (item.type == ItemType.Weapon)
        {
            Weapon weapon = (Weapon)item;
            // fail if trying to attach huge weapon.
            if (weapon.size == EquipmentSize.Huge) return false;
            // fail if trying to attach more than 1 weapons.
            if (equipment.myInventory.Count((_item) => _item.type == ItemType.Weapon) >= weaponSlots)
            {
                return false;
            }
            // fail if trying to attach a large weapon while holding a shield
            if (weapon.size == EquipmentSize.Large &&
                equipment.myInventory.Any((_item) => _item.subType == EquipmentType.Shield))
            {
                return false;
            }
        }
        return true;
    }
}
