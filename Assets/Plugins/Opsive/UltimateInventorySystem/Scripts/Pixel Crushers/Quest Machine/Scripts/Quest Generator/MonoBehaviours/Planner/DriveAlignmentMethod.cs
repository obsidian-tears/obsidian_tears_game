// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Specifies how to compute how closely motives align with drives.
    /// </summary>
    public enum DriveAlignmentMethod
    {
        /// <summary>
        /// Align by difference.
        /// </summary>
        Difference,

        /// <summary>
        /// Align by difference squared. This makes differences in the values
        /// more important.
        /// </summary>
        DifferenceSquared
    }

}