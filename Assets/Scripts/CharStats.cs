using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Demo.Events;
using Opsive.UltimateInventorySystem.Demo.UI.Menus.Main.Inventory;
using Opsive.UltimateInventorySystem.Equipping;
using System;
using UnityEditor;
using UnityEngine;
using EventHandler = Opsive.Shared.Events.EventHandler;

public class CharStats : MonoBehaviour
{
    public Equipper equipper;
    public CharacterStatsDisplay statsDisplay;

    public string characterName;

    public int level;
    public int xp;
    public int xpToLevelUp;

    public int healthBase;
    public int healthTotal;
    [ReadOnly] public int healthMax;
    

    public int magicBase;
    public int magicTotal;
    [ReadOnly] public int magicMax;
    


    public int attackBase;
    [ReadOnly] public int attackTotal;

    public int defenseBase;
    [ReadOnly] public int defenseTotal;

    public int speedBase;
    [ReadOnly] public int speedTotal;

    public string[] characterEffects;



    void Start()
    {

        if (equipper != null)
        {
            EventHandler.RegisterEvent(equipper, EventNames.c_Equipper_OnChange, UpdateStats);
        }
        UpdateStats();
    }
    /// <summary>
    /// Update the character stats.
    /// </summary>
    public void UpdateStats()
    {
        //This is a player
        if(equipper != null && statsDisplay != null)
        {
            healthMax = healthBase + equipper.GetEquipmentStatInt("MaxHp");
            magicMax = magicBase + equipper.GetEquipmentStatInt("MaxMp");
            attackTotal = attackBase + equipper.GetEquipmentStatInt("Attack");
            defenseTotal = defenseBase + equipper.GetEquipmentStatInt("Defense");
            speedTotal = speedBase + equipper.GetEquipmentStatInt("Speed");
            statsDisplay.Draw(healthMax, magicMax, attackTotal, defenseTotal, speedTotal);
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
    }

    //Returns true if the player is dead after taking damage
    public bool TakeDamage(int dmg)
    {
        healthTotal -= dmg;

        if (healthTotal <= 0)
        {
            return true;
        }
        else
            return false;
    }
}


public class ReadOnlyAttribute : PropertyAttribute
{

}

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
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
}