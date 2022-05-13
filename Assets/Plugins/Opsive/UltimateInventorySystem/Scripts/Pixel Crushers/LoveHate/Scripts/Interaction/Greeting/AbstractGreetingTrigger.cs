// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// This abstract greeting trigger is the workhorse for GreetingTrigger (3D) and
    /// GreetingTrigger2D, through an intermediate AbstractGreetingAnimationTrigger.
    /// </summary>
    public abstract class AbstractGreetingTrigger : AbstractTriggerInteractor
    {

        /// <summary>
        /// At least this many seconds must pass before greeting the same character.
        /// </summary>
        [Tooltip("This many seconds must pass before greeting the same character again.")]
        public float timeBetweenGreetings = 300f;

        private Dictionary<FactionMember, float> lastGreeting = new Dictionary<FactionMember, float>();

        protected FactionMember m_self = null;

        protected virtual void Awake()
        {
            m_self = GetComponentInChildren<FactionMember>() ?? GetComponentInParent<FactionMember>();
        }

        protected virtual void HandleOnTriggerEnter(GameObject other)
        {
            TryGreeting(GetFactionMember(other));
        }

        public virtual void TryGreeting(FactionMember other)
        {
            if (ShouldGreet(other))
            {
                Greet(other);
            }
        }

        protected virtual bool ShouldGreet(FactionMember other)
        {
            if (m_self == null || other == null || other == m_self) return false;
            var tooRecent = lastGreeting.ContainsKey(other) && (GameTime.time < (lastGreeting[other] + timeBetweenGreetings));
            return !tooRecent;
        }

        protected virtual void Greet(FactionMember other)
        {
            if (m_self == null || other == null || other == m_self) return;
            lastGreeting[other] = GameTime.time;
            ExecuteEvents.Execute<IGreetEventHandler>(m_self.gameObject, null, (x, y) => x.OnGreet(other));
        }

    }

}
