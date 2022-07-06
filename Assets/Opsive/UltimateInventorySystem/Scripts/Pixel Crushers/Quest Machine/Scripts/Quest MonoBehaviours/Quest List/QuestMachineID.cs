// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Provides an ID to GameObjects that don't have a QuestEntity or 
    /// IdentifiableQuestListContainer component.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class QuestMachineID : MonoBehaviour, IQuestMachineID
    {

        #region Serialized Fields

        [Tooltip("The ID that uniquely identifies this GameObject.")]
        [SerializeField]
        protected StringField m_id = new StringField();

        [Tooltip("The name shown in UIs.")]
        [SerializeField]
        protected StringField m_displayName = new StringField();

        [Tooltip("The image shown in UIs.")]
        [SerializeField]
        protected Sprite m_image;

        [Tooltip("Optional Quest List Container (e.g., Quest Journal) associated with this GameObject.")]
        [SerializeField]
        protected QuestListContainer m_questListContainer;

        #endregion

        #region Property Accessors to Serialized Fields

        private StringField m_fallbackID = null; // Use this ID if m_id field isn't set.
        private StringField m_fallbackDisplayName = null;

        /// <summary>
        /// The ID that uniquely identifies this GameObject.
        /// </summary>
        public StringField id
        {
            get
            {
                if (!StringField.IsNullOrEmpty(m_id)) return m_id;
                if (StringField.IsNullOrEmpty(m_fallbackID)) m_fallbackID = new StringField(name);
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
                if (StringField.IsNullOrEmpty(m_fallbackDisplayName)) m_fallbackDisplayName = new StringField(name);
                return m_fallbackDisplayName;
            }
            set { m_displayName = value; }
        }

        /// <summary>
        /// The image shown in UIs. If blank, uses the Quest Entity's Image if present.
        /// </summary>
        public Sprite image
        {
            get { return m_image; }
            set { m_image = value; }
        }

        /// <summary>
        /// QuestListContainer (such as QuestJournal) associated with this GameObject.
        /// </summary>
        public QuestListContainer questListContainer
        {
            get { return m_questListContainer; }
            set { m_questListContainer = value; }
        }

        #endregion

    }

}
