// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Inspector attribute for IQuestDialogueUI fields. Lets users assign
    /// the C# interface in the inspector.
    /// </summary>
    public class IQuestDialogueUIInspectorFieldAttribute : PropertyAttribute
    {

        public IQuestDialogueUIInspectorFieldAttribute()
        {
        }

    }
}