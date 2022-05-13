// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// A GameObject that can offer quests. 
    /// </summary>
    [AddComponentMenu("Pixel Crushers/Quest Machine/Third Party/Dialogue System/Dialogue System Quest Giver")]
    public class DialogueSystemQuestGiver : QuestGiver
    {

        [Tooltip("The Dialogue System conversation to show when the quest giver has no quests to offer.")]
        [SerializeField]
        [ConversationPopup(true)]
        private string m_noQuestsConversation;

        /// <summary>
        /// The Dialogue System conversation to show when the quest giver has no quests to offer.
        /// </summary>
        public string noQuestsConversation
        {
            get { return m_noQuestsConversation; }
            set { m_noQuestsConversation = value; }
        }

        /// <summary>
        /// If a "no quests" conversation is set, show it instead of the default quest giver content.
        /// </summary>
        protected override void ShowNoQuestsToDiscuss()
        {
            if (!string.IsNullOrEmpty(noQuestsConversation))
            {
                var playerTransform = (player != null) ? player.transform : null;
                DialogueManager.StartConversation(noQuestsConversation, playerTransform, this.transform);
            }
            else
            {
                base.ShowNoQuestsToDiscuss();
            }
        }

    }

}
