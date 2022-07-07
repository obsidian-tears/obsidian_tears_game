// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Drives define quest givers' personalities, which in turn influence the quests they generate.
    /// </summary>
    public class Drive : ScriptableObject
    {

        [Tooltip("Description of this drive.")]
        [TextArea]
        [SerializeField]
        private string m_description;

        /// <summary>
        /// Description of this drive.
        /// </summary>
        public string description
        {
            get { return m_description; }
            set { m_description = value; }
        }

    }

}