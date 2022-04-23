using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: delete levelsxp object
[CreateAssetMenu(fileName = "New LevelXp", menuName = "Player/Level XP")]
public class LevelsXp : ScriptableObject
{
    public List<int> xp;
}
