// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.QuestMachine.DialogueSystemSupport
{

    /// <summary>
    /// Associated Dialogue System conversations with an action.
    /// </summary>
    [Serializable]
    public class DialogueSystemActionConversationMapRecord
    {

        [SerializeField]
        private PixelCrushers.QuestMachine.Action m_action;

        [SerializeField]
        private List<MotiveConversation> m_motiveConversations = new List<MotiveConversation>();

        [Tooltip("Conversation to play when quest is active.")]
        [ConversationPopup]
        [SerializeField]
        private string m_activeStateConversation;

        [Tooltip("Conversation to play when player is turning in quest.")]
        [ConversationPopup]
        [SerializeField]
        private string m_turnInConversation;

        public PixelCrushers.QuestMachine.Action action
        {
            get { return m_action; }
            set { m_action = value; }
        }

        public List<MotiveConversation> motiveConversations
        {
            get { return m_motiveConversations; }
            set { m_motiveConversations = value; }
        }

        public string activeStateConversation
        {
            get { return m_activeStateConversation; }
            set { m_activeStateConversation = value; }
        }

        public string turnInConversation
        {
            get { return m_turnInConversation; }
            set { m_turnInConversation = value; }
        }

    }

    [Serializable]
    public class MotiveConversation
    {
        [ConversationPopup]
        [SerializeField]
        private string m_conversation;

        public string conversation
        {
            get { return m_conversation; }
            set { m_conversation = value; }
        }
    }

}