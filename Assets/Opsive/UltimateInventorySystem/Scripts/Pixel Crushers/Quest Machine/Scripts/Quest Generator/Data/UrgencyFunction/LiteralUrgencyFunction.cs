// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Returns a literal urgency value, not scaled by any factors.
    /// </summary>
    public class LiteralUrgencyFunction : UrgencyFunction
    {

        [Tooltip("The literal value returned by this urgency function.")]
        [SerializeField]
        private float m_value;

        /// <summary>
        /// The literal value returned by this urgency function.
        /// </summary>
        public float value
        {
            get { return m_value; }
            set { m_value = value; }
        }

        public override string typeName { get { return value.ToString(); } }

        public override float Compute(WorldModel worldModel)
        {
            return value;
        }

    }
}