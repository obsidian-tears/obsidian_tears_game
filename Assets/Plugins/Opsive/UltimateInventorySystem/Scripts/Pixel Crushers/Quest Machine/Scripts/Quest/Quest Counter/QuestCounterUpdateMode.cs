// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Specifies how a quest counter updates its value.
    /// </summary>
    public enum QuestCounterUpdateMode
    {
        /// <summary>
        /// Counter uses a DataSynchronizer to synchronize its value with an external source.
        /// Synchronizes using the counter's name.
        /// </summary>
        DataSync,

        /// <summary>
        /// Counter listens for messages and adjusts its value accordingly.
        /// </summary>
        Messages
    }

}

