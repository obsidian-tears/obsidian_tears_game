// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Requires that a faction have a specific affinity range to another faction.
    /// </summary>
    public class FactionRequirementFunction : RequirementFunction
    {

        [Tooltip("The judging entity.")]
        [SerializeField]
        private EntitySpecifier m_judge;

        [Tooltip("The entity being judged.")]
        [SerializeField]
        private EntitySpecifier m_subject;

        [Tooltip("The minimum required faction.")]
        [Range(-100, 100)]
        [SerializeField]
        private float m_minFaction = 0;

        [Tooltip("The maximum required faction.")]
        [Range(-100,100)]
        [SerializeField]
        private float m_maxFaction = 50;

        /// <summary>
        /// The judging entity.
        /// </summary>
        public EntitySpecifier judge
        {
            get { return m_judge; }
            set { m_judge = value; }
        }

        /// <summary>
        /// The entity being judged.
        /// </summary>
        public EntitySpecifier subject
        {
            get { return m_subject; }
            set { m_subject = value; }
        }

        /// <summary>
        /// The minimum required faction.
        /// </summary>
        public float minFaction
        {
            get { return m_minFaction; }
            set { m_minFaction = value; }
        }

        /// <summary>
        /// The maximum required faction.
        /// </summary>
        public float maxFaction
        {
            get { return m_maxFaction; }
            set { m_maxFaction = value; }
        }

        public override string typeName { get { return "Faction " + ((judge != null) ? judge.typeName : "Unspecified") + "->" + ((subject != null) ? subject.typeName : "Unspecified") + " is [" + minFaction + "," + maxFaction + "]"; } }

        public override bool IsTrue(WorldModel worldModel)
        {
            if (judge == null || subject == null) return false;
            var judgeFaction = judge.GetEntityType(worldModel).GetFaction();
            var subjectFaction = subject.GetEntityType(worldModel).GetFaction();
            var affinity = judgeFaction.GetAffinity(subjectFaction);
            return (minFaction <= affinity && affinity <= maxFaction);
        }

    }
}