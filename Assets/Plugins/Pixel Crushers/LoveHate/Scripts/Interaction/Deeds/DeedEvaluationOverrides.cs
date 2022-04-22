// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;

namespace PixelCrushers.LoveHate
{

    [Serializable]
    public class DeedOverrideInfo
    {
        /// <summary>
        /// The type of deed (e.g., "attack", "compliment").
        /// </summary>
        [Tooltip("The type of deed (e.g., 'attack', 'compliment').")]
        public string tag;

        /// <summary>
        /// The target faction that the deed was done to.
        /// </summary>
        [Tooltip("The target faction that the deed was done to.")]
        public int targetFactionID;

        /// <summary>
        /// The impact of the deed, where -100 is the worst and +100 is the best.
        /// For example, killing a target in an awful way approaches -100, while
        /// saving a target's family approaches +100.
        /// </summary>
        [Tooltip("The impact of the deed, where -100 is the worst and +100 is the best. For example, killing a target in an awful way approaches -100, while saving a target's family approaches +100.")]
        [Range(-100, 100)]
        public float impact;

        /// <summary>
        /// How aggressive or submissive the deed is, where -100 is the most 
        /// submissive and +100 is the most aggressive.
        /// </summary>
        [Tooltip("How aggressive or submissive the deed is, where -100 is the most submissive and +100 is the most aggressive.")]
        [Range(-100, 100)]
        public float aggression;

        /// <summary>
        /// The traits associated with the deed.
        /// </summary>
        [Tooltip("The traits associated with the deed.")]
        public float[] traits;
    }

    [RequireComponent(typeof(FactionMember))]
    [AddComponentMenu("")] // Use wrapper.
    public class DeedEvaluationOverrides : MonoBehaviour
    {

        /// <summary>
        /// The deed overrides for this faction member.
        /// </summary>
        public DeedOverrideInfo[] deedOverrides = new DeedOverrideInfo[0];

        private FactionMember m_member = null;

        private void Awake()
        {
            m_member = GetComponent<FactionMember>();
        }

        private void Start()
        {
            m_member.EvaluateRumor = EvaluateRumor;
        }

        public Rumor EvaluateRumor(Rumor rumor, FactionMember source)
        {
            if (rumor == null) return null;
            for (int i = 0; i < deedOverrides.Length; i++)
            {
                var deedOverride = deedOverrides[i];
                if (string.Equals(deedOverride.tag, rumor.tag) &&
                    (deedOverride.targetFactionID == rumor.targetFactionID ||
                     m_member.factionManager.factionDatabase.FactionHasAncestor(rumor.targetFactionID, deedOverride.targetFactionID)))
                {
                    if (m_member.debugEvalFunc) Debug.Log("Love/Hate: Using deed evaluation override on " + name + " to evaluate deed '" + rumor.tag + "'.", this);
                    var tempRumor = Rumor.GetNew(rumor);
                    tempRumor.impact = deedOverride.impact;
                    tempRumor.aggression = deedOverride.aggression;
                    Traits.Copy(deedOverride.traits, ref tempRumor.traits);
                    var result = m_member.DefaultEvaluateRumor(tempRumor, source);
                    Rumor.Release(tempRumor);
                    return result;
                }
            }
            return m_member.DefaultEvaluateRumor(rumor, source);
        }

        /// <summary>
        /// For optional UtopiaWorx Zone Controller integration.
        /// </summary>
        /// <returns>The properties that Zone Controller can control.</returns>
        public static List<string> ZonePluginActivator()
        {
            List<string> controllable = new List<string>();
            return controllable;
        }

    }

}
