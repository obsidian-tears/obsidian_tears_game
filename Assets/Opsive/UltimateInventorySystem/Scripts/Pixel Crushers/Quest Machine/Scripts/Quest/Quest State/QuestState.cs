// Copyright (c) Pixel Crushers. All rights reserved.

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// The states that a quest can be in. Note that quest nodes each
    /// have their own states specified by QuestNodeState.
    /// </summary>
    public enum QuestState
    {
        /// <summary>
        /// Not active yet.
        /// </summary>
        WaitingToStart,

        /// <summary>
        /// Waiting for the quester to complete objectives.
        /// </summary>
        Active,

        /// <summary>
        /// Quester completed the objectives successfully.
        /// </summary>
        Successful,

        /// <summary>
        /// Quester failed to complete the objectives.
        /// </summary>
        Failed,

        /// <summary>
        /// Quester abandoned the quest.
        /// </summary>
        Abandoned,

        /// <summary>
        /// Quest is disabled.
        /// </summary>
        Disabled
    }

}
