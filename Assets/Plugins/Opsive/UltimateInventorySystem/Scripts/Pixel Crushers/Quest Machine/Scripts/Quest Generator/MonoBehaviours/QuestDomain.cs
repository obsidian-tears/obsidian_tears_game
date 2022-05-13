// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;
using System;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// The physical instance of a domain type in a scene. Tracks entities
    /// that enter and leave the trigger area.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class QuestDomain : MonoBehaviour
    {

        [Tooltip("This domain's domain type.")]
        [SerializeField]
        private DomainType m_domainType;

        [Tooltip("Entities currently in the domain.")]
        [SerializeField]
        private List<QuestEntity> m_entities = new List<QuestEntity>();

        /// <summary>
        /// This domain's domain type.
        /// </summary>
        public DomainType domainType
        {
            get { return m_domainType; }
            set { m_domainType = value; }
        }

        /// <summary>
        /// Entities currently in the domain.
        /// </summary>
        public List<QuestEntity> entities
        {
            get { return m_entities; }
            set { m_entities = value; }
        }

        public void OnTriggerEnter(Collider other)
        {
            AddEntity(other.GetComponentInChildren<QuestEntity>());
        }

        public void OnTriggerExit(Collider other)
        {
            RemoveEntity(other.GetComponentInChildren<QuestEntity>());
        }

#if USE_PHYSICS2D || !UNITY_2018_1_OR_NEWER

        public void OnTriggerEnter2D(Collider2D other)
        {
            AddEntity(other.GetComponentInChildren<QuestEntity>());
        }

        public void OnTriggerExit2D(Collider2D other)
        {
            RemoveEntity(other.GetComponentInChildren<QuestEntity>());
        }

#endif

        public void AddEntity(QuestEntity entity)
        {
            if (entity != null && !entities.Contains(entity))
            {
                entities.Add(entity);
                entity.despawned += OnDespawned;
            }
        }

        public void RemoveEntity(QuestEntity entity)
        {
            OnDespawned(entity);
        }

        private void OnDespawned(QuestEntity entity)
        {
            if (entity != null)
            {
                entities.Remove(entity);
                entity.despawned -= OnDespawned;
            }
        }

        /// <summary>
        /// Adds all entities in this domain to a world model.
        /// </summary>
        /// <param name="worldModel"></param>
        public void AddEntitiesToWorldModel(WorldModel worldModel)
        {
            if (worldModel == null) return;
            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                if (entity == null) continue;
                worldModel.AddEntityType(domainType, entity.entityType, 1);
            }
        }

    }
}
