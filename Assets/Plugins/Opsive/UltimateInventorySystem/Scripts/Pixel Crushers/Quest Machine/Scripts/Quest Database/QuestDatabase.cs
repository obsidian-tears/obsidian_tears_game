// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// A ScriptableObject that holds a list of quest assets.
    /// </summary>
    //---  No [CreateAssetMenu]. Hide from asset menu; use wrapper class instead.
    public class QuestDatabase : ScriptableObject
    {

        [Tooltip("This description field is for your internal reference. Not seen by the player.")]
        [TextArea]
        [SerializeField]
        private string m_description;

        [Tooltip("Quests to include in the database.")]
        [SerializeField]
        private List<Quest> m_questAssets = new List<Quest>();

        [Tooltip("Images used by EntityType assets. Click 'Collect Images' automatically fill list this from a folder of EntityTypes.")]
        [SerializeField]
        private List<Sprite> m_images;

        /// <summary>
        /// This description field is for your internal reference. Not seen by the player.
        /// </summary>
        public string description
        {
            get { return m_description; }
            set { m_description = value; }
        }

        /// <summary>
        /// Quest assets to include in the database.
        /// </summary>
        public List<Quest> questAssets { get { return m_questAssets; } }

        /// <summary>
        /// Images used by EntityType assets. Click 'Collect Images' automatically fill list this from a folder of EntityTypes.
        /// </summary>
        public List<Sprite> images
        {
            get { return m_images; }
            set { m_images = value; }
        }

        /// <summary>
        /// Registers images with Quest Machine so procedural quests can reattach them when loading saved games.
        /// </summary>
        public void RegisterImages()
        {
            for (int i = 0; i < images.Count; i++)
            {
                QuestMachine.RegisterImage(images[i]);
            }
        }

    }
}
