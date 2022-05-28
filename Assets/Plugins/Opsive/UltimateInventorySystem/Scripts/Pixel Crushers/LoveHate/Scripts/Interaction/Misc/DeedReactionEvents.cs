// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.Events;
using System;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// This component provides UnityEvent blocks for reactions that cause
    /// different pleasure values.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class DeedReactionEvents : MonoBehaviour, IWitnessDeedEventHandler
    {
        /// <summary>
        /// At least this many seconds must pass before reacting to the same character.
        /// </summary>
        [Tooltip("This many seconds must pass before showing another reaction.")]
        public float timeBetweenReactions = 300f;

        [Serializable]
        public class Reaction
        {
            [Range(-100, 100)]
            public float min;

            [Range(-100, 100)]
            public float max;

            public Temperament temperament = RangeAnimation.AllTemperaments;

            public UnityEvent onReact = new UnityEvent();

            public Reaction() { }

            public Reaction(float min, float max, Temperament temperament)
            {
                this.min = min;
                this.max = max;
                this.temperament = temperament;
            }
        }

        /// <summary>
        /// The animations to play based on deed pleasure.
        /// </summary>
        public Reaction[] reactions = new Reaction[3]
        {
            new Reaction(-100, -25, RangeAnimation.AllTemperaments),
            new Reaction(-25, 25, RangeAnimation.AllTemperaments),
            new Reaction(25, 100, RangeAnimation.AllTemperaments)
        };

        private FactionMember m_self = null;
        private Animator m_animator = null;
        private float m_timeWhenCanReact = 0;

        protected virtual void Awake()
        {
            m_self = GetComponentInChildren<FactionMember>() ?? GetComponentInParent<FactionMember>();
            m_animator = GetComponentInChildren<Animator>() ?? GetComponentInParent<Animator>();
        }

        public void OnWitnessDeed(Rumor rumor)
        {
            if (rumor == null || m_self == null || m_animator == null || GameTime.time < m_timeWhenCanReact) return;
            m_timeWhenCanReact = GameTime.time + timeBetweenReactions;
            for (int i = 0; i < reactions.Length; i++)
            {
                var reaction = reactions[i];
                var isAppropriateReaction = (reaction.min <= rumor.pleasure && rumor.pleasure <= reaction.max) &&
                    ((reaction.temperament & m_self.pad.GetTemperament()) != 0);
                if (isAppropriateReaction)
                {
                    if (reaction.onReact != null)
                    {
                        reaction.onReact.Invoke();
                    }
                    break;
                }
            }
        }

    }
}