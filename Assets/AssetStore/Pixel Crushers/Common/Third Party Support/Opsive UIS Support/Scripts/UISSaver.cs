namespace PixelCrushers.UISSupport
{
    using UnityEngine;
    using Opsive.UltimateInventorySystem.SaveSystem;

    /// <summary>
    /// Incorporates Opsive Ultimate Inventory System's Save System Manager into
    /// the Pixel Crushers Save System.
    /// </summary>
    [AddComponentMenu("Pixel Crushers/Common/Save System/Opsive/UIS Saver")]
    public class UISSaver : Saver
    {
        public int saveSlot = 0;

        public override string RecordData()
        {
            // Save the game without saving to disk by specifying the "false" parameter
            SaveSystemManager.Save(saveSlot, false);
            var saveDataInfo = SaveSystemManager.GetCurrentSaveDataInfo();
            var saveData = saveDataInfo.Data;
            return SaveSystem.Serialize(saveData);
        }

        public override void ApplyData(string s)
        {
            if (string.IsNullOrEmpty(s)) return;
            var saveData = SaveSystem.Deserialize<SaveData>(s);
            if (saveData == null) return;
            // Load the save data directly without reading from disk.
            SaveSystemManager.Load(saveSlot, saveData);
        }

        public override void OnRestartGame()
        {
            SaveSystemManager.DeleteSave(saveSlot);
        }
    }
}
