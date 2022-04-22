// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Heading text UI content.
    /// </summary>
    public class HeadingTextQuestContent : QuestContent
    {

        [Tooltip("Use the quest's Title for the heading text.")]
        [SerializeField]
        private bool m_useQuestTitle;

        [Tooltip("Text to show in heading text style.")]
        [SerializeField]
        private StringField m_headingText;

        [Tooltip("Heading level (1=main heading, 2=subheading, etc.)")]
        [Range(1, 5)]
        [SerializeField]
        private int m_headingLevel = 1;

        private static StringField UnassignedQuestNameField = new StringField("Quest");

        public bool useQuestTitle
        {
            get { return m_useQuestTitle; }
            set { m_useQuestTitle = value; }
        }

        public int headingLevel
        {
            get { return m_headingLevel; }
            set { m_headingLevel = value; }
        }

        /// <summary>
        /// Text to show in heading text style.
        /// </summary>
        public StringField headingText
        {
            get { return m_useQuestTitle ? ((quest != null) ? quest.title : UnassignedQuestNameField) : m_headingText; }
            set { m_headingText = value; }
        }

        public override StringField originalText
        {
            get { return headingText; }
            set { headingText = value; }
        }

        public override string GetEditorName()
        {
            return useQuestTitle ? "Heading: <Quest Title>" : "Heading: " + headingText;
        }

    }

}
