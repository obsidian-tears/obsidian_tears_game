using Opsive.UltimateInventorySystem.Demo.UI.Menus.Main.Inventory;
using Opsive.UltimateInventorySystem.Equipping;
using PixelCrushers;
using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharStatsSaver : Saver
{

    [Serializable]
    public class Data
    {

        public string characterName;
        public string characterClass;

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

        public string[] characterEffects;
    }

    public override string RecordData()
    {
        var charStats = GetComponent<CharStats>();
        var data = new Data();
        data.characterName = charStats.characterName;
        data.characterClass = charStats.characterClass;
        data.level = charStats.level;
        data.xp = charStats.xp;
        data.xpToLevelUp = charStats.xpToLevelUp;
        data.pointsRemaining = charStats.pointsRemaining;
        data.healthBase = charStats.healthBase;
        data.healthTotal = charStats.healthTotal;
        data.healthMax = charStats.healthMax;
        data.magicBase = charStats.magicBase;
        data.magicTotal = charStats.magicTotal;
        data.magicMax = charStats.magicMax;
        data.attackBase = charStats.attackBase;
        data.attackTotal = charStats.attackTotal;
        data.magicPowerBase = charStats.magicPowerBase;
        data.magicPowerTotal = charStats.magicPowerTotal;
        data.defenseBase = charStats.defenseBase;
        data.defenseTotal = charStats.defenseTotal;
        data.speedBase = charStats.speedBase;
        data.speedTotal = charStats.speedTotal;
        data.criticalHitProbability = charStats.criticalHitProbability;
        data.characterEffects = charStats.characterEffects;

        return SaveSystem.Serialize(data);
    }

    public override void ApplyData(string s)
    {
        if (SceneManager.GetActiveScene().name != "GranGranFirst")
        {
            if (string.IsNullOrEmpty(s)) return;
            var data = SaveSystem.Deserialize<Data>(s);
            if (data == null) return;

            var charStats = GetComponent<CharStats>();

            charStats.characterName = data.characterName;
            charStats.characterClass = data.characterClass;
            charStats.level = data.level;
            charStats.xp = data.xp;
            charStats.xpToLevelUp = data.xpToLevelUp;
            charStats.pointsRemaining = data.pointsRemaining;
            charStats.healthBase = data.healthBase;
            charStats.healthTotal = data.healthTotal;
            charStats.healthMax = data.healthMax;
            charStats.magicBase = data.magicBase;
            charStats.magicTotal = data.magicTotal;
            charStats.magicMax = data.magicMax;
            charStats.attackBase = data.attackBase;
            charStats.attackTotal = data.attackTotal;
            charStats.magicPowerBase = data.magicPowerBase;
            charStats.magicPowerTotal = data.magicPowerTotal;
            charStats.defenseBase = data.defenseBase;
            charStats.defenseTotal = data.defenseTotal;
            charStats.speedBase = data.speedBase;
            charStats.speedTotal = data.speedTotal;
            charStats.criticalHitProbability = data.criticalHitProbability;
            charStats.characterEffects = data.characterEffects;
        }
        
    }
}
