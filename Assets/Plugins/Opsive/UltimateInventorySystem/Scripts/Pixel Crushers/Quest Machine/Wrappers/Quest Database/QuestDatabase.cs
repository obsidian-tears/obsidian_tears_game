// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine.Wrappers
{

    /// <summary>
    /// This wrapper class keeps references intact if you switch between the 
    /// compiled assembly and source code versions of the original class.
    /// </summary>
    [CreateAssetMenu(menuName = "Pixel Crushers/Quest Machine/Quest Database")]
    public class QuestDatabase : PixelCrushers.QuestMachine.QuestDatabase
    {
    }

}