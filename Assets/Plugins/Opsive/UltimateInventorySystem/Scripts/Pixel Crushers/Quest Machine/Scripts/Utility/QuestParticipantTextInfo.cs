// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Holds text info about a quest partcipant without referencing the participant,
    /// to decouple QuestGiver/Quester from Quest.
    /// </summary>
    [Serializable]
    public class QuestParticipantTextInfo
    {

        [SerializeField]
        private StringField m_id;

        [SerializeField]
        private StringField m_displayName;

        [SerializeField]
        private TextTable m_textTable;

        [SerializeField]
        private Sprite m_image;

        public StringField id { get { return m_id; } }
        
        public StringField displayName { get { return m_displayName; } }

        public Sprite image { get { return m_image; } }

        public TextTable textTable { get { return m_textTable; } }

        public QuestParticipantTextInfo(StringField id, StringField displayName, Sprite image, TextTable textTable)
        {
            m_id = id;
            m_displayName = displayName;
            m_image = image;
            m_textTable = textTable;
        }

    }
}
