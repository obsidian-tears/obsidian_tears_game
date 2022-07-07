// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Interface for quest alert UIs.
    /// </summary>
    public interface IQuestAlertUI
    {

        /// <summary>
        /// Shows a quest alert.
        /// </summary>
        /// <param name="questID">Quest ID.</param>
        /// <param name="contents">Quest alert content.</param>
        void ShowAlert(string questID, List<QuestContent> contents);

        /// <summary>
        /// Shows a quest alert.
        /// </summary>
        /// <param name="message">Alert to show.</param>
        void ShowAlert(string message);

        /// <summary>
        /// Shows a quest alert.
        /// </summary>
        /// <param name="stringField">Alert to show.</param>
        void ShowAlert(StringField stringField);

    }

}
