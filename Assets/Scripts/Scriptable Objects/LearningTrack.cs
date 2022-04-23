using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New LearningTrack", menuName = "Player/Learning Track")]
public class LearningTrack : ScriptableObject
{
    public List<Spell> spells;
    public List<int> spellLevels;
    public LevelsXp levelsExp; // TODO: change to list of int values of xp. index + 1 = level
}
