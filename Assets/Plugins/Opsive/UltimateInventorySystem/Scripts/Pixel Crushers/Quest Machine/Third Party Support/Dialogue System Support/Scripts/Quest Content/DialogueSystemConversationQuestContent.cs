// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.QuestMachine.DialogueSystemSupport
{

    /// <summary>
    /// UI content that points to a Dialogue System conversation.
    /// </summary>
    public class DialogueSystemConversationQuestContent : QuestContent
    {

        [Tooltip("Run this Dialogue System conversation.")]
        [ConversationPopup(true)]
        [SerializeField]
        private string m_conversation;

        [Tooltip("Jump to this dialogue entry. Leave set to -1 to start at beginning of conversation.")]
        [SerializeField]
        private int m_entryID = -1;

        private StringField m_conversationStringField = null;
        private StringField conversationStringField
        {
            get
            {
                if (m_conversationStringField == null)
                {
                    m_conversationStringField = new StringField(m_conversation);
                }
                return m_conversationStringField;
            }
            set
            {
                m_conversationStringField = null;
                m_conversation = StringField.GetStringValue(value);
            }
        }

        /// <summary>
        /// Text to show in regular body text style.
        /// </summary>
        public string conversation
        {
            get { return m_conversation; }
            set { m_conversation = value; }
        }

        /// <summary>
        /// Jump to this dialogue entry. Leave set to -1 to start at beginning of conversation.
        /// </summary>
        public int entryID
        {
            get { return m_entryID; }
            set { m_entryID = value; }
        }

        public override StringField originalText
        {
            get { return conversationStringField; }
            set { conversationStringField = value; }
        }

        public override string GetEditorName()
        {
            return (conversation == null)
                ? "Conversation"
                : (entryID == -1)
                    ? ("Conversation: " + conversation)
                    : ("Conversation: " + conversation + " Entry ID " + entryID);
        }

    }

}
