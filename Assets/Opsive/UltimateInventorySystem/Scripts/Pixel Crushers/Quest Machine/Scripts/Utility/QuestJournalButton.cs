// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Since the player's quest journal and the quest journal UI may not be in the 
    /// same scene at design time, use this component to show and hide the journal UI.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class QuestJournalButton : MonoBehaviour
    {
        public void ToggleJournalUI()
        {
            var journal = QuestMachine.GetQuestJournal();
            if (journal != null)
            {
                journal.ToggleJournalUI();
            }
            else
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: No Quest Journal found in scene. Cannot toggle journal UI.");
            }
        }

        public void ShowJournalUI()
        {
            var journal = QuestMachine.GetQuestJournal();
            if (journal != null)
            {
                journal.ShowJournalUI();
            }
            else
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: No Quest Journal found in scene. Cannot show journal UI.");
            }
        }

        public void HideJournalUI()
        {
            var journal = QuestMachine.GetQuestJournal();
            if (journal != null)
            {
                journal.HideJournalUI();
            }
            else
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: No Quest Journal found in scene. Cannot hide journal UI.");
            }
        }
    }
}
