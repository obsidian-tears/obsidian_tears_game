// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// A fact is a record in a world model, specifically a count of entities in a domain.
    /// </summary>
    [Serializable]
    public class Fact
    {

        [SerializeField]
        private DomainType m_domainType;

        [SerializeField]
        private EntityType m_entityType;

        [SerializeField]
        private int m_count;

        private float m_urgency;

        /// <summary>
        /// The domain containing one or more of an entity.
        /// </summary>
        public DomainType domainType
        {
            get { return m_domainType; }
            set { m_domainType = value; }
        }
        
        /// <summary>
        /// The type of entity in the domain.
        /// </summary>
        public EntityType entityType
        {
            get { return m_entityType; }
            set { m_entityType = value; }
        }

        /// <summary>
        /// The amount of this entity in the domain.
        /// </summary>
        public int count
        {
            get { return m_count; }
            set { m_count = value; }
        }

        public float urgency
        {
            get { return m_urgency; }
            set { m_urgency = value; }
        }

        public Fact(DomainType domainType, EntityType entityType, int count, float urgency = 0)
        {
            this.domainType = domainType;
            this.entityType = entityType;
            this.count = count;
            this.urgency = urgency;
        }

        public Fact(Fact source)
        {
            this.domainType = source.domainType;
            this.entityType = source.entityType;
            this.count = source.count;
            this.urgency = source.urgency;
        }

    }

}