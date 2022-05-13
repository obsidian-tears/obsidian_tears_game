// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    public class QuestTagsToTextTableWizard : ScriptableWizard
    {

        public TextTable textTable;

        public static void Open()
        {
            ScriptableWizard.DisplayWizard<QuestTagsToTextTableWizard>("Tags To Text Table", "Add Tags");
        }

        private void OnWizardUpdate()
        {
            helpString = "This wizard will add any tags you've used in this quest (for example, '{Hello}') to a Text Table." +
                "\n\nQuest: " + ((QuestEditorWindow.selectedQuest != null) ? QuestEditorWindow.selectedQuest.name : "(none)");
            if (textTable == null) helpString += "\n\nPlease select a Text Table.";
        }

        private void OnWizardCreate()
        {
            QuestMachineTags.AddQuestTagsToTextTable(QuestEditorWindow.selectedQuest, textTable);
            Debug.Log("Quest Machine: Copied all tags mentioned in " + QuestEditorWindow.selectedQuest + " to " + textTable + ".", textTable);
        }

    }
}