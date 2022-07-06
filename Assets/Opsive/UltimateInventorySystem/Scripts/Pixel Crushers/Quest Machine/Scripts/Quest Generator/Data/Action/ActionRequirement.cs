// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Defines a precondition to doing an action.
    /// </summary>
    [Serializable]
    public class ActionRequirement
    {

        [SerializeField]
        private bool m_not = false;

        [SerializeField]
        private DomainSpecifier m_domainSpecifier;

        [SerializeField]
        private EntitySpecifier m_entitySpecifier;

        [SerializeField]
        private int m_min = 1;

        [SerializeField]
        private int m_max = 65535;

        [SerializeField]
        private RequirementFunction m_requirementFunction;

        /// <summary>
        /// The requirement must be false.
        /// </summary>
        public bool not
        {
            get { return m_not; }
            set { m_not = value; }
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

        public int min
        {
            get { return m_min; }
            set { m_min = value; }
        }

        public int max
        {
            get { return m_max; }
            set { m_max = value; }
        }

        public RequirementFunction requirementFunction
        {
            get { return m_requirementFunction; }
            set { m_requirementFunction = value; }
        }
    }

}