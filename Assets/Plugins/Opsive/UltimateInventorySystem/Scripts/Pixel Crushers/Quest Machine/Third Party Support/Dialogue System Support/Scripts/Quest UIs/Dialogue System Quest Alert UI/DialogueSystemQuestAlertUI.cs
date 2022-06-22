// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.QuestMachine.DialogueSystemSupport
{

    /// <summary>
    /// Shows quest alert UIs through the Dialogue System's alert panel.
    /// </summary>
    [AddComponentMenu("Pixel Crushers/Quest Machine/Third Party/Dialogue System/UI/Dialogue System Quest Alert UI")]
    public class DialogueSystemQuestAlertUI : MonoBehaviour, IQuestAlertUI
    {

        protected virtual void Start()
        {
            var qmConfig = GetComponentInChildren<QuestMachineConfiguration>() ?? GetComponentInParent<QuestMachineConfiguration>() ?? FindObjectOfType<QuestMachineConfiguration>();
            if (qmConfig != null) qmConfig.questAlertUI = this;
            var alertDisplayer = GetComponentInChildren<QuestAlertDisplayer>() ?? GetComponentInParent<QuestAlertDisplayer>() ?? FindObjectOfType<QuestAlertDisplayer>();
            if (alertDisplayer != null) alertDisplayer.questAlertUI = this;
        }

        /// <summary>
        /// Shows a quest alert.
        /// </summary>
        /// <param name="questID">Quest ID.</param>
        /// <param name="contents">Quest alert content.</param>
        public virtual void ShowAlert(string questID, List<QuestContent> contents)
        {
            if (contents == null) return;
            var message = string.Empty;
            var first = true;
            for (int i = 0; i < contents.Count; i++)
            {
                if (!first) message += "\n";
                message += contents[i].runtimeText;
            }
            ShowAlert(message);
        }

        /// <summary>
        /// Shows a quest alert.
        /// </summary>
        /// <param name="message">Alert to show.</param>
        public virtual void ShowAlert(string message)
        {
            DialogueManager.ShowAlert(message);
        }

        /// <summary>
        /// Shows a quest alert.
        /// </summary>
        /// <param name="stringField">Alert to show.</param>
        public virtual void ShowAlert(StringField stringField)
        {
            ShowAlert(StringField.GetStringValue(stringField));
        }

    }

}
