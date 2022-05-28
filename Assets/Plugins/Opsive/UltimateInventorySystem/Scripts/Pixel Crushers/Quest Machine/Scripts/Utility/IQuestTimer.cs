// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Interface for classes that handle timer ticks.
    /// </summary>
    public interface IQuestTimer
    {

        /// <summary>
        /// Invoked when one second has passed.
        /// </summary>
        void Tick();

    }
}