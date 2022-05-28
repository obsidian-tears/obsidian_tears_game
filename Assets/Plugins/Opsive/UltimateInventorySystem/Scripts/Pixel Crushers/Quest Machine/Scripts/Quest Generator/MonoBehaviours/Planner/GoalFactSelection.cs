// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Specifies how the generator should select the goal fact.
    /// </summary>
    public enum GoalFactSelection
    {
        /// <summary>
        /// Always choose the most urgent fact.
        /// </summary>
        MostUrgent,

        /// <summary>
        /// Weight facts by urgency value. A fact with urgency 20 will be chosen 66% 
        /// of the time compared to a fact with urgency 10.
        /// </summary>
        Weighted,

        /// <summary>
        /// Weight fact urgencies by the square of the urgency value. This adds more
        /// importance to differences in urgency values.
        /// </summary>
        WeightedSquared
    }

}