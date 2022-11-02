using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Obsidian Tears/Battle")]
public class Battle : ScriptableObject
{
    public Enemy enemy;

    public int level;
    public int playerBaseHealth;
    public int playerCurrentHealth;
    public int playerBaseMagic;
    public int playerCurrentMagic;
    public int playerAttackBase;
    public int playerDefenseBase;
    public int playerSpeedBase;

    public Sprite backgroundImage;

    public AudioClip music;

    public string monsterAreaObject;

    public bool wonBattle = false;
//when running from enemy
    public bool ranBattle = false;

    [Range(0f, 1.0f)]
    public float fireSusceptibility;

    [Range(0f, 1.0f)]
    public float frostSusceptibility;

    [Range(0f, 1.0f)]
    public float shockSusceptibility;
}
