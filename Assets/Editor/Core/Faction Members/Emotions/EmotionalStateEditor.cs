// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// Custom editor for EmotionalState, which is the component that runs on a
    /// faction member to determine its current emotional state.
    /// </summary>
    [CustomEditor(typeof(EmotionalState), true)]
    class EmotionalStateEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            var me = target as EmotionalState;

            // Template:
            EditorGUILayout.BeginHorizontal();
            var newTemplate = EditorGUILayout.ObjectField("Template", me.emotionModelTemplate, typeof(EmotionModel), false) as EmotionModel;
            if (newTemplate != me.emotionModelTemplate)
            {
                me.emotionModelTemplate = newTemplate;
                ApplyTemplate(me);
            }
            EditorGUI.BeginDisabledGroup(me.emotionModelTemplate == null);
            if (GUILayout.Button("Apply", GUILayout.Width(56))) ApplyTemplate(me);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            // Custom emotion definitions:
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("emotionDefinitions"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("emotionMatchMode"), true);
            serializedObject.ApplyModifiedProperties();

            // Current emotion:
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Current State", me.currentEmotionName);
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            if (GUILayout.Button("Update", GUILayout.Width(56))) me.UpdateEmotionalState();
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }

        private void ApplyTemplate(EmotionalState me)
        {
            if (me.emotionModelTemplate == null) return;
            me.emotionDefinitions = new EmotionDefinition[me.emotionModelTemplate.emotionDefinitions.Length];
            for (int i = 0; i < me.emotionModelTemplate.emotionDefinitions.Length; i++)
            {
                me.emotionDefinitions[i] = me.emotionModelTemplate.emotionDefinitions[i];
            }

        }
    }
}
