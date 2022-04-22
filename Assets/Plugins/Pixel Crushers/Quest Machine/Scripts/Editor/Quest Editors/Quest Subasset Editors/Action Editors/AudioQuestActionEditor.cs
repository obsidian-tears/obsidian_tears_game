// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Custom inspector for AudioQuestAction assets.
    /// </summary>
    [CustomEditor(typeof(AudioQuestAction), true)]
    public class AudioQuestActionEditor : QuestSubassetEditor
    {

        protected override void Draw()
        {
            DrawDefaultInspector();
       }

    }
}
