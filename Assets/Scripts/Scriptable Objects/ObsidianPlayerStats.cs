using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO NOTES BY JAKUB
// Use this class later to create scriptables with player stats - basic and current
// This scriptable is connected to global static event bus and invokes event if needed
[CreateAssetMenu(fileName = "New Obsidian Player Stats", menuName = "Player/PlayerStats")]
public class ObsidianPlayerStats : ScriptableObject
{
    public int level;
    public int xp;
    public int xpToLevelUp;
    public int pointsRemaining;
    public int healthBase;
    public int healthTotal;
    public int healthMax;
    public int magicBase;
    public int magicTotal;
    public int magicMax;
    public int attackBase;
    public int attackTotal;
    public int magicPowerBase;
    public int magicPowerTotal;
    public int defenseBase;
    public int defenseTotal;
    public int speedBase;
    public int speedTotal;
    public float criticalHitProbability;

    /// <summary>
    /// Invokes global event with current stats. Use it to propagate stat changes to different parts of the game
    /// </summary>
    public void InvokeOnStatsChangedEvent()
    {
        GlobalEvents.InvokeOnPlayerStatsChanged(this);
    }
}
