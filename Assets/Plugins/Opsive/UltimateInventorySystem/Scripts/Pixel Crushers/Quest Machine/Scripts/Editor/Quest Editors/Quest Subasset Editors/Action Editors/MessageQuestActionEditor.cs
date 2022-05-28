// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Custom inspector for MessageQuestAction assets.
    /// </summary>
    [CustomEditor(typeof(MessageQuestAction), true)]
    public class MessageQuestActionEditor : QuestSubassetEditor
    {

        protected override void Draw()
        {
            if (serializedObject == null) return;
            serializedObject.Update();

            EditorGUILayout.HelpBox("This action sends a message to the Message System.", MessageType.None);

            var senderSpecifierProperty = serializedObject.FindProperty("m_senderSpecifier");
            var senderIDProperty = serializedObject.FindProperty("m_senderID");
            var targetSpecifierProperty = serializedObject.FindProperty("m_targetSpecifier");
            var targetIDProperty = serializedObject.FindProperty("m_targetID");
            var messageProperty = serializedObject.FindProperty("m_message");
            var parameterProperty = serializedObject.FindProperty("m_parameter");
            var valueProperty = serializedObject.FindProperty("m_value");
            if (senderSpecifierProperty == null || senderIDProperty == null || targetSpecifierProperty == null || targetIDProperty == null ||
                messageProperty == null || parameterProperty == null || valueProperty == null) return;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(senderSpecifierProperty, new GUIContent("Sender", "Required message sender."), true);
            if (EditorGUI.EndChangeCheck()) QuestEditorUtility.SetMessageParticipantID(senderSpecifierProperty, senderIDProperty);

            if (senderSpecifierProperty.enumValueIndex == (int)QuestMessageParticipant.Other)
            {
                EditorGUILayout.PropertyField(senderIDProperty,
                    new GUIContent("Sender ID", "Required message sender ID, or any sender if blank. Can also be {QUESTERID} or {QUESTGIVERID}. Sender must have a Quest Giver or Entity component."), true);
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(targetSpecifierProperty, new GUIContent("Target", "Required message target."), true);
            if (EditorGUI.EndChangeCheck()) QuestEditorUtility.SetMessageParticipantID(targetSpecifierProperty, targetIDProperty);

            if (targetSpecifierProperty.enumValueIndex == (int)QuestMessageParticipant.Other)
            {
                EditorGUILayout.PropertyField(targetIDProperty,
                    new GUIContent("Target ID", "Required message target ID, or any sender if blank. Can also be {QUESTERID} or {QUESTGIVERID}. Target must have a Quest Giver or Entity component."), true);
            }

            EditorGUILayout.PropertyField(messageProperty, true);
            EditorGUILayout.PropertyField(parameterProperty, true);
            EditorGUILayout.PropertyField(valueProperty, true);

            serializedObject.ApplyModifiedProperties();
        }

    }
}
