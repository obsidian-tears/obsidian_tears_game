using System.Collections.Generic;
using UnityEngine;

namespace Opsive.UltimateInventorySystem.SaveSystem
{
    /// <summary>
    /// In UIS 1.2+, this component is no longer used.
    /// </summary>
    public class SaveSystemManagerNoDisk : SaveSystemManager
    {
        [PixelCrushers.HelpBox("The UIS integration no longer uses this component.You can remove it.", PixelCrushers.HelpBoxMessageType.Info)]
        public bool noLongerUsed;
    }
}
