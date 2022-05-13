// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// This abstract aura trigger is the workhorse for AuraTrigger (3D) and
    /// AuraTrigger2D.
    /// </summary>
    [RequireComponent(typeof(Traits))]
    public abstract class AbstractAuraTrigger : AbstractTriggerInteractor
    {

        /// <summary>
        /// At least this many seconds must pass before affecting the same character.
        /// </summary>
        [Tooltip("This many seconds must pass before affecting the same character again.")]
        public float timeBetweenEffects = 300f;

        [Range(-100, 100)]
        [Tooltip("How powerfully the aura affects characters.")]
        public float impact;

        [Range(-100, 100)]
        [Tooltip("How submissive (-100) or aggressive (+100) the aura is.")]
        public float aggression;

        [Tooltip("Log to the console when applying aura effects.")]
        public bool debug = false;

        private Dictionary<FactionMember, float> lastTime = new Dictionary<FactionMember, float>();

        private Traits m_self = null;

        protected virtual void Awake()
        {
            m_self = GetComponentInChildren<Traits>() ?? GetComponentInParent<Traits>();
        }

        protected virtual void HandleOnTriggerEnter(GameObject other)
        {
            TryAffect(GetFactionMember(other));
        }

        public virtual void TryAffect(FactionMember other)
        {
            if (ShouldAffect(other))
            {
                Affect(other);
            }
        }

        protected virtual bool ShouldAffect(FactionMember other)
        {
            if (m_self == null || other == null || other.gameObject == gameObject) return false;
            var tooRecent = lastTime.ContainsKey(other) && (GameTime.time < (lastTime[other] + timeBetweenEffects));
            return !tooRecent;
        }

        protected virtual void Affect(FactionMember other)
        {
            if (m_self == null || other == null || other.faction == null || other.gameObject == gameObject) return;
            lastTime[other] = GameTime.time;
            if (debug) Debug.Log("Love/Hate: Applying aura effect " + name + " to " + other.name, this);
            var alignment = Traits.Alignment(m_self.traits, other.faction.traits);
            var pleasureChange = alignment * impact;
            var arousalChange = Mathf.Max(-alignment * impact, -other.pad.arousal);
            var dominanceChange = alignment * (aggression / 100) * impact;
            other.ModifyPAD(pleasureChange, pleasureChange, arousalChange, dominanceChange);
            ExecuteEvents.Execute<IAuraEventHandler>(gameObject, null, (x, y) => x.OnAura(other));
            ExecuteEvents.Execute<IEnterAuraEventHandler>(other.gameObject, null, (x, y) => x.OnEnterAura(this));
        }

    }

}
