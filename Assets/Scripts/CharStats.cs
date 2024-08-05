using GameManagers;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.DataStructures;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using Opsive.UltimateInventorySystem.Demo.Events;
using Opsive.UltimateInventorySystem.Demo.UI.Menus.Main.Inventory;
using Opsive.UltimateInventorySystem.Equipping;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EventHandler = Opsive.Shared.Events.EventHandler;


public class CharStats : MonoBehaviour
{

    [SerializeField] private ItemCategoryAmount _itemCategoryToBuff;
    [SerializeField] private float _multiplierDamage;

    public Equipper equipper;
    public Inventory inventory;
    //public CharacterStatsDisplay statsDisplay;

    // public Slider healthSlider;
    // public Slider magicSlider;

    public string characterClass;
    
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
        Debug.Log("Char Stat Start");
        if(inventory != null)
        {
            equipmentCollection = inventory.GetItemCollection("Equipped");
            
            if (equipmentCollection == null)
            {
                Debug.Log("Equipment null!!!!");
            }
            
            EventHandler.RegisterEvent(equipmentCollection, EventNames.c_ItemCollection_OnUpdate, UpdateStats);
            UpdateStatsWithHeal();
            UpdateUI();
        }
        Debug.Log("Finish Char Stat Start");
    }

    private void Update()
    {
        UpdateStats();
    }
    /// <summary>
    /// Update the character stats.
    /// </summary>
    public void UpdateStats()
    {
        //This is a player
        if(equipper != null && GameUIManager.Instance.StatsDisplay != null)
        {
            healthMax = healthBase + equipmentCollection.GetIntSum("MaxHp");
            magicMax = magicBase + equipmentCollection.GetIntSum("MaxMp");
            attackTotal = attackBase +
                           /*(inventory.GetItemCollection(1).HasItem(_itemCategoryToBuff, true) ? Mathf.RoundToInt(equipmentCollection.GetIntSum("Attack") * _multiplierDamage):*/ equipmentCollection.GetIntSum("Attack")/*)*/;

           
            defenseTotal = defenseBase + equipmentCollection.GetIntSum("Defense");
            speedTotal = speedBase + equipmentCollection.GetIntSum("Speed");
            criticalHitProbability = equipmentCollection.GetFloatSum("CriticalChance");
            magicPowerTotal = magicPowerBase + equipmentCollection.GetIntSum("MagicPower");

            UpdateUI();
        }

        // Note by Jakub - looks like this part of code is never used by enemy
        // This is an enemy
        // else
        // {
        //     healthMax = healthBase;
        //     healthTotal = healthMax;
        //     magicMax = magicBase;
        //     magicTotal = magicMax;
        //     attackTotal = attackBase;
        //     defenseTotal = defenseBase;
        //     speedTotal = speedBase;
        // }
    }

    public void UpdateStatsWithHeal()
    {
        if (equipper != null && GameUIManager.Instance.StatsDisplay != null)
        {
            healthMax = healthBase + equipmentCollection.GetIntSum("MaxHp");
            magicMax = magicBase + equipmentCollection.GetIntSum("MaxMp");
            attackTotal = attackBase +
                           /*(inventory.GetItemCollection(1).HasItem(_itemCategoryToBuff, true) ? Mathf.RoundToInt(equipmentCollection.GetIntSum("Attack") * _multiplierDamage):*/ equipmentCollection.GetIntSum("Attack")/*)*/;
            healthTotal = healthMax;
            magicTotal = magicMax;

            defenseTotal = defenseBase + equipmentCollection.GetIntSum("Defense");
            speedTotal = speedBase + equipmentCollection.GetIntSum("Speed");
            criticalHitProbability = equipmentCollection.GetFloatSum("CriticalChance");
            magicPowerTotal = magicPowerBase + equipmentCollection.GetIntSum("MagicPower");
            UpdateUI();

        }    

    }

    public void UpdateUI() 
    {
        GameUIManager.Instance.StatsDisplay.Draw(healthMax, magicMax, attackTotal, defenseTotal, speedTotal, healthTotal, magicTotal, magicPowerTotal, healthBase, magicBase, attackBase, defenseBase, speedBase, magicPowerBase, xp, xpToLevelUp);
        GameUIManager.Instance.SetHealthSlider(healthTotal, healthMax);
        GameUIManager.Instance.SetMagicSlider(magicTotal, magicMax);
        GameUIManager.Instance.SetXpSlider(xp, xpToLevelUp);
        GameUIManager.Instance.SetLevelText(level.ToString());
    }

    // Returns true if the player is dead after taking damage
    public bool TakeDamage(int dmg)
    {
        healthTotal -= dmg;

        //If is Player
        if(equipper != null && GameUIManager.Instance.StatsDisplay != null)
            UpdateUI();

        return healthTotal <= 0;
    }

    public void Heal(int healAmt)
    {
        healthTotal += healAmt;
        if(healthTotal > healthMax)
        {
            healthTotal = healthMax;
        }
        else if (healthTotal < 0)
        {
            healthTotal = 0;
        }
        if (equipper != null && GameUIManager.Instance.StatsDisplay != null)
            UpdateUI();
    }

    public void AddMana(int manaAmt)
    {
        magicTotal += manaAmt;
        if(magicTotal > magicMax)
        {
            magicTotal = magicMax;
        }
        else if (magicTotal < 0)
        {
            magicTotal = 0;
        }
        if(equipper != null && GameUIManager.Instance.StatsDisplay != null)
        {
            UpdateUI();
        }
    }

    public void AddXP(int xPAmt)
    {
        xp += xPAmt;

        if (equipper != null && GameUIManager.Instance.StatsDisplay != null)
        {
            UpdateUI();
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