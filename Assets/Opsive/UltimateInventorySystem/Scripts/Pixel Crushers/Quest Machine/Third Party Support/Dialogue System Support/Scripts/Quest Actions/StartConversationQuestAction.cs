// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Quest action that plays a Dialogue System conversation.
    /// </summary>
    public class StartConversationQuestAction : QuestAction
    {

        [Tooltip("Conversation to start.")]
        [ConversationPopup(true)]
        [SerializeField]
        private string m_conversation;

        [Tooltip("Conversation actor (e.g., player).")]
        [SerializeField]
        private StringField m_conversationActor;

        [Tooltip("Conversation conversant (e.g., NPC).")]
        [SerializeField]
        private StringField m_conversationConversant;

        public string conversation
        {
            get { return m_conversation; }
            set { m_conversation = value; }
        }

        public StringField conversationActor
        {
            get { return m_conversationActor; }
            set { m_conversationActor = value; }
        }

        public StringField conversationConversant
        {
            get { return m_conversationConversant; }
            set { m_conversationConversant = value; }
        }

        public override string GetEditorName()
        {
            return "Conversation[" + conversationActor + "/" + conversationConversant + "]: " + conversation;
        }

        protected Transform GetParticipantTransform(StringField participantName)
        {
            var name = StringField.GetStringValue(participantName);
            if (string.IsNullOrEmpty(name)) return null;
            var t = PixelCrushers.DialogueSystem.CharacterInfo.GetRegisteredActorTransform(name);
            if (t != null) return t;
            var go = GameObject.Find(name);
            return (go != null) ? go.transform : null;
        }

        public override void Execute()
        {
            base.Execute();
            var actor = GetParticipantTransform(conversationActor);
            var conversant = GetParticipantTransform(conversationConversant);
            DialogueManager.StartConversation(conversation, actor, conversant);
        }

    }

}
