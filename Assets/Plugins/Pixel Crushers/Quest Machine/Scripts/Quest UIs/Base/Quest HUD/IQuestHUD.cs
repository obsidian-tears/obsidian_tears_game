// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Interface for quest HUDs.
    /// </summary>
    public interface IQuestHUD
    {

        /// <summary>
        /// True if the HUD is visible, false otherwise.
        /// </summary>
        bool isVisible { get; }

        /// <summary>
        /// Show the quest journal contents in the UI.
        /// </summary>
        void Show(QuestListContainer questListContainer);

        /// <summary>
        /// Hide the UI.
        /// </summary>
        void Hide();

        /// <summary>
        /// Toggle the visibility of the UI.
        /// </summary>
        void Toggle(QuestListContainer questListContainer);

        /// <summary>
        /// 
        /// </summary>
        void Repaint(QuestListContainer questListContainer);

        /// <summary>
        /// True if the group is expanded in the UI.
        /// </summary>
        bool IsGroupExpanded(string groupName);

        /// <summary>
        /// Toggles whether a group is expanded or not.
        /// </summary>
        /// <param name="groupName">Group to toggle.</param>
        void ToggleGroup(string groupName);

    }
}