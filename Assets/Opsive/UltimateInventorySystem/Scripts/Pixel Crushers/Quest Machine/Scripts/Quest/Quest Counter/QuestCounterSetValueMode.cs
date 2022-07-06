// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Specifies how to inform listeners when setting a quest counter value.
    /// </summary>
    public enum QuestCounterSetValueMode
    {
        /// <summary>
        /// Inform listeners and the data synchronizer if applicable.
        /// </summary>
        InformListeners,

        /// <summary>
        /// Don't inform listeners or the data synchronizer.
        /// </summary>
        DontInformListeners,

        /// <summary>
        /// Inform listeners but not the data synchronizer.
        /// </summary>
        DontInformDataSync
    }

}
