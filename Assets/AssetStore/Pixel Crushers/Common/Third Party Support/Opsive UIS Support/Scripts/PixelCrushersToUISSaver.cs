using Opsive.Shared.Utility;
using Opsive.UltimateInventorySystem.SaveSystem;
using UnityEngine;

namespace PixelCrushers.UISSupport
{
    /// <summary>
    /// This is an Opsive UIS saver component that saves and loads the Pixel Crushers Save System.
    /// Only use it if you want the Opsive UIS save system to be the primary save system.
    /// (Normally you will make the Pixel Crushers Save System the primary save sysing and
    /// use UISSaver instead.)
    /// </summary>
    [AddComponentMenu("Pixel Crushers/Common/Save System/Opsive/Pixel Crushers to UIS Saver")]
    public class PixelCrushersToUISSaver : SaverBase
    {
        public override Serialization SerializeSaveData()
        {
            var data = PixelCrushers.SaveSystem.RecordSavedGameData();
            return Serialization.Serialize(data);
        }

        public override void DeserializeAndLoadSaveData(Serialization serializedSaveData)
        {
            var data = serializedSaveData.DeserializeFields(MemberVisibility.All) as PixelCrushers.SavedGameData;
            if (data != null)
            {
                PixelCrushers.SaveSystem.ApplySavedGameData(data);
            }
        }
    }
}
