using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleResult
{
    public BattleResult(int level, float strength, float agility, float magicPower, float maxMagic, float maxHealth, Spell spell)
    {
        dLevel = level;
        dStrength = strength;
        dAgility = agility;
        dMagicPower = magicPower;
        dMaxMagic = maxMagic;
        dMaxHealth = maxHealth;
        newSpell = spell;
    }
    public int dLevel;
    public float dStrength;
    public float dAgility;
    public float dMagicPower;
    public float dMaxMagic;
    public float dMaxHealth;
    public Spell newSpell;
}
