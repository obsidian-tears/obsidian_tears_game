// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// This abstract class is the workhorse for GossipTrigger and GossipTrigger2D.
    /// </summary>
    public abstract class AbstractGossipTrigger : AbstractTriggerInteractor
    {

        /// <summary>
        /// At least this many seconds must pass before gossiping with the same character.
        /// </summary>
        [Tooltip("This many seconds must pass before gossiping with the same character again")]
        public float timeBetweenGossip = 300f;

        private Dictionary<FactionMember, float> lastGossip = new Dictionary<FactionMember, float>();

        private FactionMember m_self = null;

        protected virtual void Awake()
        {
            m_self = GetComponentInChildren<FactionMember>() ?? GetComponentInParent<FactionMember>();
        }

        protected virtual void HandleOnTriggerEnter(GameObject other)
        {
            TryGossip(GetFactionMember(other));
        }

        /// <summary>
        /// Tries to gossip between this faction member and another, based on affinity
        /// and when they last gossiped.
        /// </summary>
        /// <param name="other">Other faction member.</param>
        public virtual void TryGossip(FactionMember other)
        {
            if (ShouldGossip(other))
            {
                Gossip(other);
            }
        }

        protected virtual bool ShouldGossip(FactionMember other)
        {
            if (m_self == null || other == null || other == m_self ||
                other.factionID == FactionDatabase.PlayerFactionID || other.CompareTag("Player")) return false;
            var tooRecent = lastGossip.ContainsKey(other) && (GameTime.time < (lastGossip[other] + timeBetweenGossip));
            return !tooRecent && (m_self.GetAffinity(other) > 0);
        }

        protected virtual void Gossip(FactionMember other)
        {
            if (m_self == null || other == null) return;
            UpdateLastGossipTime(other);
            var otherGossipTrigger = other.GetComponent<AbstractGossipTrigger>();
            if (otherGossipTrigger != null)
            {
                otherGossipTrigger.UpdateLastGossipTime(m_self);
            }
            m_self.ShareRumors(other);
            other.ShareRumors(m_self);
            ExecuteEvents.Execute<IGossipEventHandler>(m_self.gameObject, null, (x, y) => x.OnGossip(other));
            ExecuteEvents.Execute<IGossipEventHandler>(other.gameObject, null, (x, y) => x.OnGossip(m_self));
        }

        public void UpdateLastGossipTime(FactionMember other)
        {
            lastGossip[other] = GameTime.time;
        }

    }

}
