// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Action text for a quest node state.
    /// </summary>
    [Serializable]
    public class ActionStateText
    {
        [SerializeField]
        private StringField m_dialogueText;

        [SerializeField]
        private StringField m_journalText;

        [SerializeField]
        private StringField m_hudText;

        [SerializeField]
        private StringField m_alertText;

        /// <summary>
        /// Shown in dialogue UI when the task created by this action is active.
        /// </summary>
        public StringField dialogueText
        {
            get { return m_dialogueText; }
            set { m_dialogueText = value; }
        }

        /// <summary>
        /// Shown in journal UI when the task created by this action is active.
        /// </summary>
        public StringField journalText
        {
            get { return m_journalText; }
            set { m_journalText = value; }
        }

        /// <summary>
        /// Shown in HUD when the task created by this action is active.
        /// </summary>
        public StringField hudText
        {
            get { return m_hudText; }
            set { m_hudText = value; }
        }

        /// <summary>
        /// Shown in alert UI when 
        /// </summary>
        public StringField alertText
        {
            get { return m_alertText; }
            set { m_alertText = value; }
        }

    }

    /// <summary>
    /// UI text for actions to use when creating quests.
    /// </summary>
    [Serializable]
    public class ActionText
    {

        [Tooltip("Text to use when the task is active. Optionally specify message to send when node becomes active; use ':' to separate message and parameter.")]
        [SerializeField]
        private ActionStateText m_activeText = new ActionStateText();

        [Tooltip("Text to use when the task is complete. Optionally specify message to send when node is completed; use ':' to separate message and parameter.")]
        [SerializeField]
        private ActionStateText m_completedText = new ActionStateText();

        [SerializeField]
        private StringField m_successText;

        /// <summary>
        /// Text to use when the task is active.
        /// </summary>
        public ActionStateText activeText
        {
            get { return m_activeText; }
            set { m_activeText = value; }
        }

        /// <summary>
        /// Text to use when the task is complete.
        /// </summary>
        public ActionStateText completedText
        {
            get { return m_completedText; }
            set { m_completedText = value; }
        }

        /// <summary>
        /// Shown in dialogue UI when returning to giver if this is the goal action.
        /// </summary>
        public StringField successText
        {
            get { return m_successText; }
            set { m_successText = value; }
        }

    }

}