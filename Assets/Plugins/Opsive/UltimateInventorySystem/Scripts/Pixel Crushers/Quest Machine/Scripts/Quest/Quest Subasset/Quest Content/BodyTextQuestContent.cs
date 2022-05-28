// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Body text UI content.
    /// </summary>
    public class BodyTextQuestContent : QuestContent
    {

        [Tooltip("Text to show in regular body text style.")]
        [StringFieldTextArea]
        [SerializeField]
        private StringField m_bodyText;

        /// <summary>
        /// Text to show in regular body text style.
        /// </summary>
        public StringField bodyText
        {
            get { return m_bodyText; }
            set { m_bodyText = value; }
        }

        public override StringField originalText
        {
            get { return bodyText; }
            set { bodyText = value; }
       }

        public override string GetEditorName()
        {
            return (bodyText == null) ? "Body Text" : "Text: " + bodyText;
        }

    }

}
