// Copyright (c) Pixel Crushers. All rights reserved.

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// The states that a quest node can be in. Note that the quest's 
    /// overall state is specified by QuestState.
    /// </summary>
    public enum QuestNodeState
    {
        /// <summary>
        /// Not active yet.
        /// </summary>
        Inactive,

        /// <summary>
        /// Waiting for conditions to be met.
        /// </summary>
        Active,

        /// <summary>
        /// Conditions have been met.
        /// </summary>
        True

    }

}
