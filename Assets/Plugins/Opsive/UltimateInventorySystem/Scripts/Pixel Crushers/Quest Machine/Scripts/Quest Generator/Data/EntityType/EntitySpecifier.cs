// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    public enum EntitySpecifierType { ThisEntity, QuestGiver, Quester, Other }

    /// <summary>
    /// This class provides a method of specifying an entity.
    /// </summary>
    [Serializable]
    public class EntitySpecifier
    {

        [Tooltip("The type of entity being specified.")]
        [SerializeField]
        private EntitySpecifierType m_entitySpecifierType;

        [Tooltip("If Other, this is the entity type.")]
        [SerializeField]
        private EntityType m_entityType;

        /// <summary>
        /// The type of entity being specified.
        /// </summary>
        public EntitySpecifierType entitySpecifierType
        {
            get { return m_entitySpecifierType; }
            set { m_entitySpecifierType = value; }
        }

        /// <summary>
        /// If Other, this is the entity type.
        /// </summary>
        public EntityType entityType
        {
            get { return m_entityType; }
            set { m_entityType = value; }
        }

        public string typeName
        {
            get
            {
                switch (entitySpecifierType)
                {
                    case EntitySpecifierType.ThisEntity:
                        return "This Entity";
                    case EntitySpecifierType.Quester:
                        return "Quester";
                    case EntitySpecifierType.QuestGiver:
                        return "Quest Giver";
                    case EntitySpecifierType.Other:
                    default:
                        return (entityType != null) ? entityType.name : "Unspecified";
                }
            }
        }

        public EntityType GetEntityType(WorldModel worldModel)
        {
            switch (entitySpecifierType)
            {
                case EntitySpecifierType.ThisEntity:
                    return worldModel.observed.entityType;
                case EntitySpecifierType.Quester:
                    return PlayerEntityType.instance;
                case EntitySpecifierType.QuestGiver:
                    return worldModel.observer.entityType;
                case EntitySpecifierType.Other:
                default:
                    return entityType;
            }
        }

    }

}