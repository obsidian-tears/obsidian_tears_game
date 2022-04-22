// Copyright (c) Pixel Crushers. All rights reserved.

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Types of nodes supported in quests.
    /// </summary>
    public enum QuestNodeType
    {
        /// <summary>
        /// Start node is where the quest starts.
        /// </summary>
        Start,

        /// <summary>
        /// When a success node becomes active, it immediately sets the quest successful.
        /// </summary>
        Success,

        /// <summary>
        /// When a failure node becomes active, it immediately sets the quest failed.
        /// </summary>
        Failure,

        /// <summary>
        /// When passthrough node becomes active, it immediately changes to the successful state.
        /// </summary>
        Passthrough,

        /// <summary>
        /// Monitors success and failure conditions; if either become true, sets the node state and activates destinations.
        /// </summary>
        Condition
    }

}
