// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Quest action that plays a Dialogue System sequence.
    /// </summary>
    public class PlaySequenceQuestAction : QuestAction
    {

        [SerializeField]
        private string m_sequence;

        public string sequence
        {
            get { return m_sequence; }
            set { m_sequence = value; }
        }

        public override void Execute()
        {
            base.Execute();
            DialogueManager.PlaySequence(sequence);
        }

        public override string GetEditorName()
        {
            return "Sequence: " + sequence.Replace("\n", " ");
        }

    }

}
