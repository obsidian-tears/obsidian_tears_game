using PixelCrushers;
using System;
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
        var data = new Data
        {
            characterName = charStats.characterName,
            characterClass = charStats.characterClass,
            level = charStats.level,
            xp = charStats.xp,
            xpToLevelUp = charStats.xpToLevelUp,
            pointsRemaining = charStats.pointsRemaining,
            healthBase = charStats.healthBase,
            healthTotal = charStats.healthTotal,
            healthMax = charStats.healthMax,
            magicBase = charStats.magicBase,
            magicTotal = charStats.magicTotal,
            magicMax = charStats.magicMax,
            attackBase = charStats.attackBase,
            attackTotal = charStats.attackTotal,
            magicPowerBase = charStats.magicPowerBase,
            magicPowerTotal = charStats.magicPowerTotal,
            defenseBase = charStats.defenseBase,
            defenseTotal = charStats.defenseTotal,
            speedBase = charStats.speedBase,
            speedTotal = charStats.speedTotal,
            criticalHitProbability = charStats.criticalHitProbability,
            characterEffects = charStats.characterEffects
        };

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
