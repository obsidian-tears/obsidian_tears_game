// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Physical instance of an entity type in a scene.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class QuestEntity : MonoBehaviour, IQuestMachineID
    {

        [Tooltip("This entity's entity type.")]
        [SerializeField]
        private EntityType m_entityType;

        private StringField m_fallbackDisplayName = null;

        /// <summary>
        /// This entity's entity type.
        /// </summary>
        public EntityType entityType
        {
            get { return m_entityType; }
            set { m_entityType = value; }
        }

        public bool hasInternallyAssignedID
        {
            get { return entityType != null; }
        }

        public bool hasInternallyAssignedDisplayName
        {
            get { return entityType != null; }
        }

        public StringField id
        {
            get
            {
                if (m_questListContainer != null && m_questListContainer.hasInternallyAssignedID) return m_questListContainer.id;
                if (entityType != null && !StringField.IsNullOrEmpty(entityType.displayName)) return entityType.displayName;
                if (m_fallbackDisplayName == null) m_fallbackDisplayName = new StringField((entityType != null) ? entityType.name : name);
                return m_fallbackDisplayName;
            }
        }

        public StringField displayName
        {
            get
            {
                if (m_questListContainer != null && m_questListContainer.hasInternallyAssignedDisplayName) return m_questListContainer.displayName;
                if (entityType != null && !StringField.IsNullOrEmpty(entityType.displayName)) return entityType.displayName;
                if (m_fallbackDisplayName == null) m_fallbackDisplayName = new StringField((entityType != null) ? entityType.name : name);
                return m_fallbackDisplayName;
            }
        }

        public Sprite image
        {
            get { return (entityType != null) ? entityType.image : null; }
        }

        public delegate void EntityDelegate(QuestEntity entity);

        public event EntityDelegate despawned = delegate { };

        private IdentifiableQuestListContainer m_questListContainer = null;

        public virtual void Awake()
        {
            m_questListContainer = GetComponent<IdentifiableQuestListContainer>();
        }

        public virtual void OnDisable()
        {
            despawned(this);
        }

        public virtual void OnDestroy()
        {
            despawned(this);
        }

    }

}