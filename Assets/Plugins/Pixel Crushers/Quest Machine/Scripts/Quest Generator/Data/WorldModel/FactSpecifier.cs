// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    [Serializable]
    public class FactSpecifier
    {

        [SerializeField]
        private bool m_negate = false;

        [SerializeField]
        private DomainSpecifier m_domainSpecifier;

        [SerializeField]
        private EntitySpecifier m_entitySpecifier;

        [SerializeField]
        private int m_count = 1;

        public bool negate
        {
            get { return m_negate; }
            set { m_negate = value; }
        }

        public DomainSpecifier domainSpecifier
        {
            get { return m_domainSpecifier; }
            set { m_domainSpecifier = value; }
        }

        public EntitySpecifier entitySpecifier
        {
            get { return m_entitySpecifier; }
            set { m_entitySpecifier = value; }
        }

        public int count
        {
            get { return m_count; }
            set { m_count = value; }
        }

        public Fact GetFact(WorldModel worldModel)
        {
            return new Fact(domainSpecifier.GetDomainType(worldModel), entitySpecifier.GetEntityType(worldModel), count); //[TODO] Pool.
        }

    }

}