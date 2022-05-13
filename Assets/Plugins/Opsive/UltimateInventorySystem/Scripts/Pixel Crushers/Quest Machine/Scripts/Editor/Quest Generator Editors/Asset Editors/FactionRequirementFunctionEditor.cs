// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    [CustomEditor(typeof(FactionRequirementFunction), true)]
    public class FactionRequirementFunctionEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Requires that a faction have a specific affinity range to another faction.", MessageType.None);
            serializedObject.Update();

            var labelWidth = 100f;
            var floatWidth = 50;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Judge", "The entity that must have a specific affinity to a subject."), GUILayout.Width(labelWidth));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_judge"));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Subject", "The entity being judged."), GUILayout.Width(labelWidth));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_subject"));
            EditorGUILayout.EndHorizontal();

            var minFactionProperty = serializedObject.FindProperty("m_minFaction");
            var maxFactionProperty = serializedObject.FindProperty("m_maxFaction");
            var min = minFactionProperty.floatValue;
            var max = maxFactionProperty.floatValue;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Required Faction"), GUILayout.Width(labelWidth));
            min = EditorGUILayout.FloatField(min, GUILayout.Width(floatWidth));
            EditorGUILayout.MinMaxSlider(ref min, ref max, -100, 100f);
            max = EditorGUILayout.FloatField(max, GUILayout.Width(floatWidth));
            EditorGUILayout.EndHorizontal();
            if (min != minFactionProperty.floatValue) minFactionProperty.floatValue = min;
            if (max != maxFactionProperty.floatValue) maxFactionProperty.floatValue = max;

            serializedObject.ApplyModifiedProperties();
        }

    }

}