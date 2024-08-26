using System.Collections;
using System.Collections.Generic;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.DataStructures;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using Opsive.UltimateInventorySystem.Exchange;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Obsidian Tears/Enemy") ] 
public class Enemy : ScriptableObject
{
    public string enemyName;
    public int enemyServerId;
    public Sprite sprite;
    public int hp;
    public int mp;
    public int attackDamage;
    public int defense;
    public int speed;

    public int xpDrop;

    public int goldDrop;

    public ItemInfo[] itemDrops;

    [Range(0f, 1.0f)]
    public float runSuccessProbability;

    [Range(0f, 1.0f)]
    public float criticalHitProbability;

    [Range(0f, 1.0f)]
    public float fireSusceptibility;

    [Range(0f, 1.0f)]
    public float frostSusceptibility;

    [Range(0f, 1.0f)]
    public float shockSusceptibility;
}
