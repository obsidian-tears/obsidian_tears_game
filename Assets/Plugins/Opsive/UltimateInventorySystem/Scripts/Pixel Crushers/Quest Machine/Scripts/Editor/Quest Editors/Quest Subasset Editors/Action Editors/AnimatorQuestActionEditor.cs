// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Custom inspector for AnimatorQuestAction assets.
    /// </summary>
    [CustomEditor(typeof(AnimatorQuestAction), true)]
    public class AnimatorQuestActionEditor : QuestSubassetEditor
    {

        protected override void Draw()
        {
            if (serializedObject == null) return;
            EditorGUILayout.HelpBox("This action controls an Animator.", MessageType.None);

            var gameObjectNameProperty = serializedObject.FindProperty("m_gameObjectName");
            var actionProperty = serializedObject.FindProperty("m_action");
            var targetProperty = serializedObject.FindProperty("m_target");
            var boolValueProperty = serializedObject.FindProperty("m_boolValue");
            var floatValueProperty = serializedObject.FindProperty("m_floatValue");
            if (gameObjectNameProperty == null || actionProperty == null || targetProperty == null || boolValueProperty == null || floatValueProperty == null) return;

            EditorGUILayout.PropertyField(gameObjectNameProperty);
            EditorGUILayout.PropertyField(actionProperty);
            switch ((AnimatorControlAction)actionProperty.enumValueIndex)
            {
                case AnimatorControlAction.Play:
                    EditorGUILayout.PropertyField(targetProperty, new GUIContent("State", "State to crossfade into playing."));
                    EditorGUILayout.PropertyField(floatValueProperty, new GUIContent("Crossfade Duration", "Crossfade over this duration in seconds."));
                    break;
                case AnimatorControlAction.SetTrigger:
                    EditorGUILayout.PropertyField(targetProperty, new GUIContent("Trigger Parameter", "Trigger parameter to set."));
                    break;
                case AnimatorControlAction.SetBool:
                    EditorGUILayout.PropertyField(targetProperty, new GUIContent("Boolean Parameter", "Boolean parameter to set."));
                    EditorGUILayout.PropertyField(boolValueProperty, new GUIContent("Value", "Tick to set true, untick to set false."));
                    break;
                case AnimatorControlAction.SetFloat:
                    EditorGUILayout.PropertyField(targetProperty, new GUIContent("Float Parameter", "Float parameter to set."));
                    EditorGUILayout.PropertyField(floatValueProperty, new GUIContent("Value"));
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }

    }
}
