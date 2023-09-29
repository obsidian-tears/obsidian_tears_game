using Opsive.UltimateInventorySystem.Core.AttributeSystem;
using Opsive.UltimateInventorySystem.Core.DataStructures;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellsFinder : MonoBehaviour
{


    Inventory inventory;

    List<SpellScroll> GetSpells()
    {
        List<SpellScroll> spells = new List<SpellScroll>();


        foreach(ItemCollection itemCol in inventory.ItemCollectionsReadOnly)
        {
            if(itemCol.Name == "MainItemCollection")
            {
                foreach(ItemStack itemStack in itemCol.GetAllItemStacks())
                {
                    int i = 0;
                    while(i < itemStack.Amount)
                    {
                        if(itemStack.Item.GetItemCategory().name == "Spells")
                        {
                            SpellScroll spell = new SpellScroll();
                            spell.spellName = itemStack.Item.name;
                            spell.magicPowerAmount = itemStack.Item.GetAttribute<Attribute<int>>("Magic Power Added").GetValue();
                            spell.magicPowerMultiplier = itemStack.Item.GetAttribute<Attribute<float>>("Magic Power Multiplier").GetValue();
                            spell.healAmount = itemStack.Item.GetAttribute<Attribute<int>>("Heal Amount").GetValue();
                            spell.frostDamage = itemStack.Item.GetAttribute<Attribute<bool>>("Frost Damage").GetValue();
                            spell.lightningDamage = itemStack.Item.GetAttribute<Attribute<bool>>("Lightning Damage").GetValue();
                            spell.fireDamage = itemStack.Item.GetAttribute<Attribute<bool>>("Fire Damage").GetValue();
                            spells.Add(spell);
                        }
                        i++;
                    }
                }
            }
        }

        return spells;
    }
}

public class SpellScroll
{
    public string spellName;
    public int magicPowerAmount;
    public float magicPowerMultiplier;
    public int healAmount;
    public bool frostDamage;
    public bool lightningDamage;
    public bool fireDamage;
}