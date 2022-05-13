// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// This abstract greeting trigger is the workhorse for GreetingTrigger (3D) and
    /// GreetingTrigger2D.
    /// </summary>
    public abstract class AbstractGreetingAnimationTrigger : AbstractGreetingTrigger
    {

        /// <summary>
        /// The animations to play based on affinity.
        /// </summary>
        public RangeAnimation[] greetings = new RangeAnimation[3]
        {
            new RangeAnimation(string.Empty, -100, -25, RangeAnimation.AllTemperaments),
            new RangeAnimation(string.Empty, -25, 25, RangeAnimation.AllTemperaments),
            new RangeAnimation(string.Empty, 25, 100, RangeAnimation.AllTemperaments)
        };

        private Animator m_animator = null;

        protected override void Awake()
        {
            base.Awake();
            m_animator = GetComponentInChildren<Animator>() ?? GetComponentInParent<Animator>();
        }

        protected override void Greet(FactionMember other)
        {
            if (m_self == null || other == null || other == m_self || m_animator == null) return;
            base.Greet(other);
            var affinity = m_self.GetAffinity(other);
            for (int g = 0; g < greetings.Length; g++)
            {
                var greeting = greetings[g];
                var isAppropriateGreeting = (greeting.min <= affinity && affinity <= greeting.max) &&
                    ((greeting.temperament & m_self.pad.GetTemperament()) != 0);
                if (isAppropriateGreeting)
                {
                    if (!string.IsNullOrEmpty(greeting.triggerParameter))
                    {
                        m_animator.SetTrigger(greeting.triggerParameter);
                    }
                    break;
                }
            }
        }

    }

}
