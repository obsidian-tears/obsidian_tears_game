// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Body text UI content that handles Dialogue System markup tags.
    /// </summary>
    public class DialogueSystemTextQuestContent : BodyTextQuestContent
    {

        public override string runtimeText
        {
            get { return FormattedText.Parse(base.runtimeText).text; }
        }

        public override string GetEditorName()
        {
            return (bodyText == null) ? "Dialogue System Text" : "Dialogue System Text: " + bodyText;
        }

    }

}
