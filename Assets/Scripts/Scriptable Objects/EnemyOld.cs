using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName ="New Enemy", menuName ="Enemies/Enemy")]
public class EnemyOld : ScriptableObject
{
    public string enemyName;
    public int level;
    public float strength;
    public float agility;
    public float magicPower;
    public float maxMagic;
    public float maxHealth;
    public int xp;
    public int gold;
    public Sprite sprite;
}
