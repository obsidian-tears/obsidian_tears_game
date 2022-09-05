using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using Opsive.UltimateInventorySystem.Demo.Events;
using Opsive.UltimateInventorySystem.Demo.UI.Menus.Main.Inventory;
using Opsive.UltimateInventorySystem.Equipping;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EventHandler = Opsive.Shared.Events.EventHandler;

public class CharStats : MonoBehaviour
{
    public Equipper equipper;
    public Inventory inventory;
    public CharacterStatsDisplay statsDisplay;

    public Slider healthSlider;
    public Slider magicSlider;

    public string characterName;

    public int level;
    public int xp;
    public int xpToLevelUp;

    public int pointsRemaining;

    public int healthBase;
    public int healthTotal;
    [ReadOnly] public int healthMax;
    

    public int magicBase;
    public int magicTotal;
    [ReadOnly] public int magicMax;
    


    public int attackBase;
    [ReadOnly] public int attackTotal;


    public int magicPowerBase;
    [ReadOnly] public int magicPowerTotal;

    public int defenseBase;
    [ReadOnly] public int defenseTotal;

    public int speedBase;
    [ReadOnly] public int speedTotal;

    public float criticalHitProbability;

    public string[] characterEffects;

    ItemCollection equipmentCollection;


    void Start()
    {
        if(inventory != null)
        {
            equipmentCollection = inventory.GetItemCollection("Equipped");
            EventHandler.RegisterEvent(equipmentCollection, EventNames.c_ItemCollection_OnUpdate, () => UpdateStats());
            UpdateStats();
        }
        
    }
    /// <summary>
    /// Update the character stats.
    /// </summary>
    public void UpdateStats()
    {
        //This is a player
        if(equipper != null && statsDisplay != null)
        {
            healthMax = healthBase + equipmentCollection.GetIntSum("MaxHp");
            magicMax = magicBase + equipmentCollection.GetIntSum("MaxMp");
            attackTotal = attackBase + equipmentCollection.GetIntSum("Attack");
            defenseTotal = defenseBase + equipmentCollection.GetIntSum("Defense");
            speedTotal = speedBase + equipmentCollection.GetIntSum("Speed");
            criticalHitProbability = equipmentCollection.GetFloatSum("CriticalChance");
            magicPowerTotal = magicPowerBase + equipmentCollection.GetIntSum("MagicPower");
            statsDisplay.Draw(healthMax, magicMax, attackTotal, defenseTotal, speedTotal, healthTotal, magicTotal, magicPowerTotal, healthBase, magicBase, attackBase, defenseBase, speedBase, magicPowerBase, xp, xpToLevelUp);
        }

        //This is an enemy
        else
        {
            healthMax = healthBase;
            healthTotal = healthMax;
            magicMax = magicBase;
            magicTotal = magicMax;
            attackTotal = attackBase;
            defenseTotal = defenseBase;
            speedTotal = speedBase;
        }

        if(healthSlider != null)
        {
            healthSlider.maxValue = healthMax;
            healthSlider.value = healthTotal;
        }
        if(magicSlider != null)
        {
            magicSlider.maxValue = magicMax;
            magicSlider.value = magicTotal;
        }
    }

    //Returns true if the player is dead after taking damage
    public bool TakeDamage(int dmg)
    {
        healthTotal -= dmg;
        if(equipper != null && statsDisplay != null)
            UpdateStats();

        if (healthTotal <= 0)
        {
            return true;
        }
        else
            return false;
    }

    public void Heal(int healAmt)
    {
        healthTotal += healAmt;
        if(healthTotal > healthMax)
        {
            healthTotal = healthMax;
        }
        if (equipper != null && statsDisplay != null)
            UpdateStats();
    }

    public void AddMana(int manaAmt)
    {
        magicTotal += manaAmt;
        if(magicTotal > magicMax)
        {
            magicTotal = magicMax;
        }
        if(equipper != null && statsDisplay != null)
        {
            UpdateStats();
        }
    }

    public void AddXP(int xPAmt)
    {
        xp += xPAmt;
        if (equipper != null && statsDisplay != null)
        {
            UpdateStats();
        }
    }
}


public class ReadOnlyAttribute : PropertyAttribute
{

}

/*[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property,
                                            GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position,
                               SerializedProperty property,
                               GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}*/