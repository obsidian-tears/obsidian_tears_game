// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Interface for quest dialogue UIs.
    /// </summary>
    public interface IQuestDialogueUI
    {

        /// <summary>
        /// True if the dialogue UI is visible, false otherwise.
        /// </summary>
        bool isVisible { get; }

        /// <summary>
        /// Shows UI content.
        /// </summary>
        /// <param name="speaker">Speaker.</param>
        /// <param name="contents">Content being spoken by speaker.</param>
        void ShowContents(QuestParticipantTextInfo speaker, List<QuestContent> contents);

        /// <summary>
        /// Shows content explaining that all quests' offer conditions are unmet.
        /// </summary>
        /// <param name="speaker">Speaker.</param>
        /// <param name="contents">Content explaining that all quests' offer conditions are unmet.</param>
        /// <param name="quests">List of quests.</param>
        void ShowOfferConditionsUnmet(QuestParticipantTextInfo speaker, List<QuestContent> contents, List<Quest> quests);

        /// <summary>
        /// Shows a list of quests.
        /// </summary>
        /// <param name="speaker">Speaker.</param>
        /// <param name="activeQuestsContents">Content introducing the list of active quests.</param>
        /// <param name="activeQuests">Active quests.</param>
        /// <param name="offerableQuestsContents">Content introducing the list of offerable quests.</param>
        /// <param name="offerableQuests">Offerable quests.</param>
        /// <param name="selectHandler">Method to invoke when the player selects a quest.</param>
        void ShowQuestList(QuestParticipantTextInfo speaker, List<QuestContent> activeQuestsContents, List<Quest> activeQuests, 
            List<QuestContent> offerableQuestsContents, List<Quest> offerableQuests, QuestParameterDelegate selectHandler);

        /// <summary>
        /// Shows a quest offer.
        /// </summary>
        /// <param name="speaker">Speaker.</param>
        /// <param name="quest">Quest to offer.</param>
        /// <param name="acceptHandler">Method to invoke if the player accepts the quest.</param>
        /// <param name="declineHandler">Method to invoke if the player declines the quest.</param>
        void ShowOfferQuest(QuestParticipantTextInfo speaker, Quest quest, QuestParameterDelegate acceptHandler, QuestParameterDelegate declineHandler);

        /// <summary>
        /// Shows an active quest.
        /// </summary>
        /// <param name="speaker">Speaker.</param>
        /// <param name="quest">Active quest.</param>
        /// <param name="continueHandler">Method to invoke if the player clicks the continue button.</param>
        /// <param name="backHandler">Method to invoke if the player clicks the back button.</param>
        void ShowActiveQuest(QuestParticipantTextInfo speaker, Quest quest, QuestParameterDelegate continueHandler, QuestParameterDelegate backHandler);

        /// <summary>
        /// Shows completed quests.
        /// </summary>
        /// <param name="speaker">Speaker</param>
        /// <param name="quests">Completed quests.</param>
        void ShowCompletedQuest(QuestParticipantTextInfo speaker, List<Quest> quests);

        /// <summary>
        /// Hides the dialogue UI.
        /// </summary>
        void Hide();

    }

}
