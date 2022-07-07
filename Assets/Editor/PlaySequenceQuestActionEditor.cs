// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Custom inspector for PlaySequenceQuestAction assets.
    /// </summary>
    [CustomEditor(typeof(PlaySequenceQuestAction), true)]
    public class PlaySequenceQuestActionEditor : QuestSubassetEditor
    {

        private Rect rect;

        protected override void Draw()
        {
            if (serializedObject == null) return;
            serializedObject.Update();
            var sequenceProperty = serializedObject.FindProperty("m_sequence");
            UnityEngine.Assertions.Assert.IsNotNull(sequenceProperty, "Quest Machine: Internal error - m_sequence is null.");
            if (sequenceProperty == null) return;
            sequenceProperty.stringValue = SequenceEditorTools.DrawLayout(new GUIContent("Sequence", "Dialogue System sequence to play."), sequenceProperty.stringValue, ref rect);
            serializedObject.ApplyModifiedProperties();
        }

    }
}
