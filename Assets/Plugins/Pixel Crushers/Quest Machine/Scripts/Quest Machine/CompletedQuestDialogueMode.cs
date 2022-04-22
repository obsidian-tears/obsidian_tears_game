// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Specifies what to show in dialogue when quest givers only have completed quests.
    /// </summary>
    public enum CompletedQuestGlobalDialogueMode
    {
        ShowCompletedQuest,
        ShowNoQuests
    }

    /// <summary>
    /// Allows a specific quest giver to override the global mode.
    /// </summary>
    public enum CompletedQuestDialogueMode
    {
        SameAsGlobal,
        ShowCompletedQuest,
        ShowNoQuests
    }
}
