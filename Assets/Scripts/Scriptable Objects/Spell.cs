using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Abilities/Spell")]
public class Spell : ScriptableObject
{
    public string spellName;
    public string spellDescription;
    public int spellLevel;
    public float spellPower;
    public bool canUseInBattle;
}
