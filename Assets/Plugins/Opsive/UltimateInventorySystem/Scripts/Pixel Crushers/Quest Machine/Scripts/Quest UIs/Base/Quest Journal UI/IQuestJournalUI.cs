// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Interface for quest journal UIs.
    /// </summary>
    public interface IQuestJournalUI
    {

        bool isVisible { get; }

        /// <summary>
        /// Show the quest journal contents in the UI.
        /// </summary>
        void Show(QuestJournal questJournal);

        /// <summary>
        /// Hide the UI.
        /// </summary>
        void Hide();

        /// <summary>
        /// Toggle the visibility of the UI.
        /// </summary>
        void Toggle(QuestJournal questJournal);

        /// <summary>
        /// Repaint the UI.
        /// </summary>
        /// <param name="questJournal"></param>
        void Repaint(QuestJournal questJournal);

        /// <summary>
        /// True if the group is expanded in the UI.
        /// </summary>
        bool IsGroupExpanded(string groupName);

        /// <summary>
        /// Toggles whether a group is expanded or not.
        /// </summary>
        /// <param name="groupName">Group to toggle.</param>
        void ToggleGroup(string groupName);

        /// <summary>
        /// Selects a quest to show more details.
        /// </summary>
        /// <param name="quest">The quest to select.</param>
        void SelectQuest(Quest quest);

    }

}
