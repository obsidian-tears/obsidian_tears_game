// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Domain types are the abstract definitions of domains, which are locations where 
    /// entities exist. Quest givers generate plans based on domain types, since not all 
    /// domains may be present in the current scene.
    /// </summary>
    public class DomainType : ScriptableObject
    {

        [TextArea]
        [SerializeField]
        private string m_description;

        [Tooltip("The display name of this entity type.")]
        [SerializeField]
        private StringField m_displayName;

        [HideInInspector]
        [SerializeField]
        private bool m_isPlayerDomain = false;

        [NonSerialized]
        private StringField m_internalAssetName = null;

        public string description
        {
            get { return m_description; }
            set { m_description = value; }
        }

        /// <summary>
        /// The display name of this entity type.
        /// </summary>
        public StringField displayName
        {
            get
            {
                if (!StringField.IsNullOrEmpty(m_displayName))
                {
                    return m_displayName;
                }
                else
                {
                    if (StringField.IsNullOrEmpty(m_internalAssetName)) m_internalAssetName = new StringField(name);
                    return m_internalAssetName;
                }
            }
            set { m_displayName = value; }
        }

        public bool isPlayerDomain
        {
            get { return m_isPlayerDomain; }
            set { m_isPlayerDomain = value; }
        }

        public string typeName { get { return isPlayerDomain ? "Player's Domain" : name; } }

        public static DomainType playerDomainInstance { get; set; }

#if UNITY_2019_3_OR_NEWER && UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitStaticVariables()
        {
            playerDomainInstance = null;
        }
#endif

        private void OnEnable()
        {
            if (isPlayerDomain) playerDomainInstance = this;
        }

        public static void SetPlayerDomainInstance(DomainType newInstance)
        {
            if (newInstance != null)
            {
                playerDomainInstance = newInstance;
            }
            if (playerDomainInstance == null && Application.isPlaying)
            {
                // If none, create a new runtime instance:
                playerDomainInstance = ScriptableObject.CreateInstance<DomainType>();
                playerDomainInstance.isPlayerDomain = true;
                playerDomainInstance.displayName = new StringField("Player");
                playerDomainInstance.description = "Represents the player's inventory.";
            }

        }

    }

}