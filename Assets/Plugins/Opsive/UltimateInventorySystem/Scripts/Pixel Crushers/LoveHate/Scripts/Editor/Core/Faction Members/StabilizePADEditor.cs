// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// This is a custom editor for StabilizePAD.
    /// </summary>
    [CustomEditor(typeof(StabilizePAD), true)]
    [CanEditMultipleObjects]
    public class StabilizePADEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            //--- For comparison with default inspector:
            //base.OnInspectorGUI();
            //EditorGUILayout.Separator();

            var script = target as StabilizePAD;
            if (script == null) return;

            Undo.RecordObject(target, "StabilizePAD");
            DrawStabilizeSettings("Happiness", script.happinessSettings);
            DrawStabilizeSettings("Pleasure", script.pleasureSettings);
            DrawStabilizeSettings("Arousal", script.arousalSettings);
            DrawStabilizeSettings("Dominance", script.dominanceSettings);
        }

        private void DrawStabilizeSettings(string label, StabilizePAD.StabilizeSettings settings)
        {
            settings.stabilize = EditorGUILayout.ToggleLeft(new GUIContent(label, "Tick to stabilize this PAD value"), settings.stabilize);
            if (settings.stabilize)
            {
                EditorGUI.indentLevel++;
                settings.target = EditorGUILayout.FloatField(new GUIContent("Target", "Target value to stabilize to"), settings.target);
                settings.changeRate = EditorGUILayout.FloatField(new GUIContent("Change Rate", "Change per second"), settings.changeRate);
                EditorGUI.indentLevel--;
            }
        }

    }

}