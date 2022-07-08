using Opsive.UltimateInventorySystem.Demo.UI.Menus.Main.Inventory;
using Opsive.UltimateInventorySystem.Equipping;
using PixelCrushers;
using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharStatsSaver : Saver
{

[DllImport("__Internal")] private static extern void SaveGame(String data);
    [Serializable]
    public class Data
    {

        public string characterName;

        public int level;
        public int xp;
        public int xpToLevelUp;

        public int healthBase;
        public int healthTotal;
        public int healthMax;

        public int magicBase;
        public int magicTotal;
        public int magicMax;

        public int attackBase;
        public int attackTotal;

        public int defenseBase;
        public int defenseTotal;

        public int speedBase;
        public int speedTotal;

        public float criticalHitProbability;

        public string[] characterEffects;
        public string test;
    }

    public override string RecordData()
    {
        var charStats = GetComponent<CharStats>();
        var data = new Data();
        data.characterName = charStats.characterName;
        data.level = charStats.level;
        data.xp = charStats.xp;
        data.xpToLevelUp = charStats.xpToLevelUp;
        data.healthBase = charStats.healthBase;
        data.healthTotal = charStats.healthTotal;
        data.healthMax = charStats.healthMax;
        data.magicBase = charStats.magicBase;
        data.magicTotal = charStats.magicTotal;
        data.magicMax = charStats.magicMax;
        data.attackBase = charStats.attackBase;
        data.attackTotal = charStats.attackTotal;
        data.defenseBase = charStats.defenseBase;
        data.defenseTotal = charStats.defenseTotal;
        data.speedBase = charStats.speedBase;
        data.speedTotal = charStats.speedTotal;
        data.criticalHitProbability = charStats.criticalHitProbability;
        data.characterEffects = charStats.characterEffects;
        data.test = "test";
Debug.Log(SaveSystem.Serialize(data));
Debug.Log(SaveSystem.Serialize(data).GetType());
#if UNITY_WEBGL == true && UNITY_EDITOR == false
                SaveGame(SaveSystem.Serialize(data));
#endif
        return SaveSystem.Serialize(data);
    }

    public override void ApplyData(string s)
    {
        if (string.IsNullOrEmpty(s)) return;
        var data = SaveSystem.Deserialize<Data>(s);
        if (data == null) return;

        var charStats = GetComponent<CharStats>();

        charStats.characterName = data.characterName;
        charStats.level = data.level;
        charStats.xp = data.xp;
        charStats.xpToLevelUp = data.xpToLevelUp;
        charStats.healthBase = data.healthBase;
        charStats.healthTotal = data.healthTotal;
        charStats.healthMax = data.healthMax;
        charStats.magicBase = data.magicBase;
        charStats.magicTotal = data.magicTotal;
        charStats.magicMax = data.magicMax;
        charStats.attackBase = data.attackBase;
        charStats.attackTotal = data.attackTotal;
        charStats.defenseBase = data.defenseBase;
        charStats.defenseTotal = data.defenseTotal;
        charStats.speedBase = data.speedBase;
        charStats.speedTotal = data.speedTotal;
        charStats.criticalHitProbability = data.criticalHitProbability;
        charStats.characterEffects = data.characterEffects;
    }
}
