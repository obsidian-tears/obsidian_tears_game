// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Factions define relationships between entities.
    /// </summary>
    public class Faction : ScriptableObject
    {

        [Serializable]
        public class Relationship
        {
            [Tooltip("The other faction for whom this faction holds a negative or positive feeling.")]
            [SerializeField]
            private Faction m_faction;

            [Tooltip("The degree of negative or positive feeling for the faction, in the range [-100,+100].")]
            [Range(-100f,+100f)]
            [SerializeField]
            private float m_affinity;

            public Faction faction
            {
                get { return m_faction; }
                set { m_faction = value; }
            }

            public float affinity
            {
                get { return m_affinity; }
                set { m_affinity = value; }
            }
        }

        [SerializeField]
        private List<Relationship> m_relationships = new List<Relationship>();

        public List<Relationship> relationships
        {
            get { return m_relationships; }
            set { m_relationships = value; }
        }

        public virtual float GetAffinity(Faction other)
        {
            for (int i = 0; i < relationships.Count; i++)
            {
                var relationship = relationships[i];
                if (relationship == null) continue;
                if (relationship.faction == other)
                {
                    return relationship.affinity;
                }
            }
            return 0;
        }

    }
}
