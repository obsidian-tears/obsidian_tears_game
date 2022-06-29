using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Obsidian Tears/Enemy") ] 
public class Enemy : ScriptableObject
{
    public string enemyName;
    public Sprite sprite;
    public int hp;
    public int mp;
    public int attackDamage;
    public int defense;
    public int speed;

    public int xpDrop;
    public int goldDrop;
    public string itemDrop;

    [Range(0f, 1.0f)]
    public float runSuccessProbability;

    [Range(0f, 1.0f)]
    public float criticalHitProbability;
}
