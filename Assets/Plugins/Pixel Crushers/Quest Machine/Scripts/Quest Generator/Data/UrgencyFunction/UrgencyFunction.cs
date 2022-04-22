// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Base abstract class for urgency functions, which quest generators use to
    /// identify which entity is most urgent to generate a quest about.
    /// </summary>
    public abstract class UrgencyFunction : ScriptableObject
    {

        [Tooltip("Description of this urgency function.")]
        [TextArea]
        [SerializeField]
        private string m_description;

        /// <summary>
        /// Description of this urgency function.
        /// </summary>
        public string description
        {
            get { return m_description; }
            set { m_description = value; }
        }

        public abstract string typeName { get; }

        public abstract float Compute(WorldModel worldModel);

    }
}