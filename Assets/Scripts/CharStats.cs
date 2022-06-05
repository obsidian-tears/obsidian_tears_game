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

    public int healthBase;
    [ReadOnly] public int healthMax;
    [ReadOnly] public int healthTotal;

    public int magicBase;
    [ReadOnly] public int magicMax;
    [ReadOnly] public int magicTotal;


    public int attackBase;
    [ReadOnly] public int attackTotal;

    public int defenseBase;
    [ReadOnly] public int defenseTotal;

    public int speedBase;
    [ReadOnly] public int speedTotal;



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

        healthMax = healthBase + equipper.GetEquipmentStatInt("MaxHp");
        magicMax = magicBase + equipper.GetEquipmentStatInt("MaxMp");
        attackTotal = attackBase + equipper.GetEquipmentStatInt("Attack");
        defenseTotal = defenseBase + equipper.GetEquipmentStatInt("Defense");
        speedTotal = speedBase + equipper.GetEquipmentStatInt("Speed");

        Debug.Log(equipper.GetEquipmentStatInt("Speed"));
        statsDisplay.Draw(healthMax, magicMax, attackTotal, defenseTotal, speedTotal);
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