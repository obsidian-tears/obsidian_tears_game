// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    public enum DomainSpecifierType { ThisEntityDomain, QuestGiverDomain, QuesterDomain, Other }

    /// <summary>
    /// This class provides a method of specifying a domain.
    /// </summary>
    [Serializable]
    public class DomainSpecifier
    {

        [Tooltip("The type of domain being specified.")]
        [SerializeField]
        private DomainSpecifierType m_domainSpecifierType;

        [Tooltip("If Domain Specified Type is Other, this is the specified domain.")]
        [SerializeField]
        private DomainType m_domainType;

        /// <summary>
        /// The type of domain being specified.
        /// </summary>
        public DomainSpecifierType domainSpecifierType
        {
            get { return m_domainSpecifierType; }
            set { m_domainSpecifierType = value; }
        }

        /// <summary>
        /// If domainSpecifierType is Other, this is the specified domain.
        /// </summary>
        public DomainType domainType
        {
            get { return m_domainType; }
            set { m_domainType = value; }
        }

        public string typeName
        {
            get
            {
                switch (domainSpecifierType)
                {
                    case DomainSpecifierType.ThisEntityDomain:
                        return "This Entity's Domain";
                    case DomainSpecifierType.QuesterDomain:
                        return "Quester's Domain";
                    case DomainSpecifierType.QuestGiverDomain:
                        return "Quest Giver's Domain";
                    case DomainSpecifierType.Other:
                    default:
                        return (domainType != null) ? domainType.name : "Unspecified";
                }
            }
        }

        /// <summary>
        /// Searches a world model for the domain type specified by this domain specifier.
        /// </summary>
        /// <param name="worldModel">The world model to search.</param>
        /// <returns>The domain type specified by this domain specifier.</returns>
        public DomainType GetDomainType(WorldModel worldModel)
        {
            switch (domainSpecifierType)
            {
                case DomainSpecifierType.ThisEntityDomain:
                    return worldModel.observed.domainType;
                case DomainSpecifierType.QuesterDomain:
                    return DomainType.playerDomainInstance;
                case DomainSpecifierType.QuestGiverDomain:
                    return worldModel.observer.domainType;
                case DomainSpecifierType.Other:
                default:
                    return domainType;
            }
        }
    }

}