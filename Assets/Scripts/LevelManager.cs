using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] LearningTrack fighterTrack;
    [SerializeField] LearningTrack wizardTrack;
    [SerializeField] LearningTrack rogueTrack;
    [SerializeField] LearningTrack barbarianTrack;
    public BattleResult getLevel(PlayerType pt, int currentLevel, int xp)
    {
        if (pt == PlayerType.Fighter)
        {
            return getLevel(fighterTrack, currentLevel, xp, 1.3f, 1.0f, 0.7f, 1.3f, 0.7f);
        }
        else if (pt == PlayerType.Wizard)
        {
            return getLevel(wizardTrack, currentLevel, xp, 0.5f, 1.5f, 1.5f, 0.5f, 1.5f);
        }
        else if (pt == PlayerType.Rogue)
        {
            return getLevel(rogueTrack, currentLevel, xp, 0.7f, 2.0f, 1.3f, 0.7f, 1.3f);
        }
        else
        {
            return getLevel(barbarianTrack, currentLevel, xp, 1.5f, 0.5f, 0.5f, 1.5f, 0.5f);
        }
    }

    BattleResult getLevel(LearningTrack learningTrack, int currentLevel, int xp, float xStrength,
                            float xAgility, float xMagicPower, float xMaxHealth, float xMaxMagic)
    {
        int dLevel = 1;
        Spell newSpell = null;
        if (dLevel > 0)
        {
            int levelIndex = fighterTrack.levels.IndexOf(currentLevel + dLevel);
            newSpell = levelIndex >= 0 ? fighterTrack.spells[levelIndex] : null;
        }
        int newLevel = currentLevel + dLevel;
        float dStrength = newLevel * xStrength;
        float dAgility = newLevel * xAgility;
        float dMagicPower = newLevel * xMagicPower;
        float dMaxMagic = newLevel * xMaxMagic;
        float dMaxHealth = newLevel * xMaxHealth;
        return new BattleResult(level: dLevel, strength: dStrength, agility: dAgility, magicPower: dMagicPower,
                            maxMagic: dMaxMagic, maxHealth: dMaxHealth, spell: newSpell);
    }

}
