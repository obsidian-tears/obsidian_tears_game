// Copyright (c) Pixel Crushers. All rights reserved.

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// For conditions that are based on the status of otherconditions, this
    /// describes how those other conditions are counted.
    /// </summary>
    public enum ConditionCountMode
    {
        /// <summary>
        /// The condition becomes true if any watched condition becomes true.
        /// </summary>
        Any,

        /// <summary>
        /// The condition becomes true if all watched conditions become true.
        /// </summary>
        All,

        /// <summary>
        /// The condition becomes true if a specified minimum number of watched conditions become true.
        /// </summary>
        Min
    }

}
