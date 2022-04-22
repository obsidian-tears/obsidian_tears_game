// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// A quest list with an ID.
    /// </summary>
    [AddComponentMenu("")] // Just a base class.
    public class IdentifiableQuestListContainer : QuestListContainer, IQuestMachineID
    {

        #region Serialized Fields

        [Tooltip("The ID that uniquely identifies this entity. If unassigneduses Quest Entity's Display Name if present.")]
        [SerializeField]
        protected StringField m_id = new StringField();

        [Tooltip("The name shown in UIs. If unassigned, uses the Quest Entity's Display Name if present.")]
        [SerializeField]
        protected StringField m_displayName = new StringField();

        [Tooltip("The image shown in UIs. If unassigned, uses the Quest Entity's Image if present.")]
        [SerializeField]
        protected Sprite m_image;

        #endregion

        #region Property Accessors to Serialized Fields

        private StringField m_fallbackID = null; // Use this ID if m_id field isn't set.
        private StringField m_fallbackDisplayName = null;
        private QuestEntity m_questEntity = null;

        public bool hasInternallyAssignedID
        {
            get { return !StringField.IsNullOrEmpty(m_id) || !StringField.IsNullOrEmpty(m_fallbackID); }
        }

        public bool hasInternallyAssignedDisplayName
        {
            get { return !StringField.IsNullOrEmpty(m_displayName) || !StringField.IsNullOrEmpty(m_fallbackDisplayName); }
        }

        /// <summary>
        /// The ID that uniquely identifies this quest list container. If this is a quest giver, 
        /// then when the quester (e.g., player) accepts a quest from this quest giver, the 
        /// quester's instance will have a reference to this ID so the quester knows who gave the quest.
        /// </summary>
        public StringField id
        {
            get
            {
                if (!StringField.IsNullOrEmpty(m_id)) return m_id;
                if (StringField.IsNullOrEmpty(m_fallbackID))
                {
                    m_fallbackID = (questEntity != null && !StringField.IsNullOrEmpty(questEntity.id)) ? questEntity.id : new StringField(name);
                }
                return m_fallbackID;
            }
            set
            {
                m_id = value;
            }
        }

        /// <summary>
        /// The name shown in UIs.
        /// </summary>
        public StringField displayName
        {
            get
            {
                if (!StringField.IsNullOrEmpty(m_displayName)) return m_displayName;
                if (StringField.IsNullOrEmpty(m_fallbackDisplayName))
                {
                    m_fallbackDisplayName = (questEntity != null && !StringField.IsNullOrEmpty(questEntity.displayName)) ? questEntity.displayName : new StringField(name);
                }
                return m_fallbackDisplayName;
            }
            set { m_displayName = value; }
        }

        /// <summary>
        /// The image shown in UIs.
        /// </summary>
        public Sprite image
        {
            get { return (m_image != null) ? m_image : (questEntity != null) ? questEntity.image : null; }
            set { m_image = value; }
        }

        protected QuestEntity questEntity
        {
            get
            {
                if (m_questEntity == null) m_questEntity = GetComponentInChildren<QuestEntity>();
                return m_questEntity;
            }
        }

        #endregion

        #region Initialization

        public override void OnEnable()
        {
            base.OnEnable();
            QuestMachine.RegisterQuestListContainer(this);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            QuestMachine.UnregisterQuestListContainer(this);
        }

        #endregion

    }

}
