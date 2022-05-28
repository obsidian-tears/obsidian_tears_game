// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// Triggers an animation when the character gossips.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class GossipAnimation : MonoBehaviour, IGossipEventHandler
    {

        /// <summary>
        /// The trigger parameter to set when gossiping.
        /// </summary>
        [Tooltip("Animator trigger to set when gossiping.")]
        public string triggerParameter = string.Empty;

        private Animator m_animator = null;

        protected virtual void Awake()
        {
            m_animator = GetComponentInChildren<Animator>() ?? GetComponentInParent<Animator>();
        }

        public virtual void OnGossip(FactionMember other)
        {
            if (other == null || m_animator == null || string.IsNullOrEmpty(triggerParameter)) return;
            m_animator.SetTrigger(triggerParameter);
        }

        /// <summary>
        /// For optional UtopiaWorx Zone Controller integration.
        /// </summary>
        /// <returns>The properties that Zone Controller can control.</returns>
        public static List<string> ZonePluginActivator()
        {
            List<string> controllable = new List<string>();
            controllable.Add("triggerParameter|string|0|0|1|Trigger parameter to set when gossiping.");
            return controllable;
        }

    }

}
