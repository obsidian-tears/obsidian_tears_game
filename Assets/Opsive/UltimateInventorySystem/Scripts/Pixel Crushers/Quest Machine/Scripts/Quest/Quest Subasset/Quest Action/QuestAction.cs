// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Abstract base class for quest actions, which are executed when quests or quest nodes
    /// become active.
    /// </summary>
    public abstract class QuestAction : QuestSubasset
    {

        /// <summary>
        /// Performs the quest action.
        /// </summary>
        public virtual void Execute()
        {
        }

    }

}
