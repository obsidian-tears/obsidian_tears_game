// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Custom inspector for BarkQuestAction assets.
    /// </summary>
    [CustomEditor(typeof(BarkQuestAction), true)]
    public class BarkQuestActionEditor : QuestSubassetEditor
    {

        private Rect rect;

        protected override void Draw()
        {
            if (serializedObject == null) return;
            serializedObject.Update();
            var sourceProperty = serializedObject.FindProperty("m_source");
            var conversationProperty = serializedObject.FindProperty("m_conversation");
            var barkTextProperty = serializedObject.FindProperty("m_barkText");
            var barkerProperty = serializedObject.FindProperty("m_barker");
            var specifyDelayProperty = serializedObject.FindProperty("m_specifyDelay");
            var delayProperty = serializedObject.FindProperty("m_delay");
            var sequenceProperty = serializedObject.FindProperty("m_sequence");
            UnityEngine.Assertions.Assert.IsNotNull(sourceProperty, "Quest Machine: Internal error - m_source is null.");
            UnityEngine.Assertions.Assert.IsNotNull(conversationProperty, "Quest Machine: Internal error - m_conversation is null.");
            UnityEngine.Assertions.Assert.IsNotNull(barkTextProperty, "Quest Machine: Internal error - m_barkText is null.");
            UnityEngine.Assertions.Assert.IsNotNull(barkerProperty, "Quest Machine: Internal error - m_barker is null.");
            UnityEngine.Assertions.Assert.IsNotNull(specifyDelayProperty, "Quest Machine: Internal error - m_specifyDelay is null.");
            UnityEngine.Assertions.Assert.IsNotNull(delayProperty, "Quest Machine: Internal error - m_delay is null.");
            if (sourceProperty == null || conversationProperty == null || barkTextProperty == null || barkerProperty == null || delayProperty == null || specifyDelayProperty == null) return;
            EditorGUILayout.PropertyField(sourceProperty, new GUIContent("Source", "Get bark text from a conversation or a string entered here."), true);
            if (sourceProperty.enumValueIndex == (int)BarkQuestAction.Source.Conversation)
            {
                EditorGUILayout.PropertyField(conversationProperty, new GUIContent("Conversation"), true);
            }
            else
            {
                EditorGUILayout.PropertyField(barkTextProperty, new GUIContent("Bark Text"), true);
            }

            EditorGUILayout.PropertyField(barkerProperty, new GUIContent("Barker", "GameObject that says the bark. You can drag a GameObject or Dialogue Actor from the Hierarchy into this field."), true);

            // Handle drag-and-drop for Barker:
            switch (Event.current.type)
            {
                case EventType.Repaint:
                    rect = GUILayoutUtility.GetLastRect();
                    break;
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (rect.Contains(Event.current.mousePosition))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        if (Event.current.type == EventType.DragPerform)
                        {
                            DragAndDrop.AcceptDrag();
                            foreach (var obj in DragAndDrop.objectReferences)
                            {
                                Transform t = null;
                                if (obj is Component) t = (obj as Component).transform;
                                else if (obj is GameObject) t = (obj as GameObject).transform;
                                if (t != null)
                                {
                                    var dialogueActor = t.GetComponent<PixelCrushers.DialogueSystem.DialogueActor>();
                                    var barkerName = (dialogueActor != null && !string.IsNullOrEmpty(dialogueActor.actor))
                                        ? dialogueActor.actor : t.name;
                                    barkerProperty.FindPropertyRelative("m_text").stringValue = barkerName;
                                }
                            }
                        }
                    }
                    break;
            }

            EditorGUILayout.PropertyField(specifyDelayProperty, true);
            if (specifyDelayProperty.boolValue)
            {
                EditorGUILayout.PropertyField(delayProperty, true);
            }

            if (sourceProperty.enumValueIndex == (int)BarkQuestAction.Source.String)
            {
                EditorGUILayout.PropertyField(sequenceProperty, true);
            }

            serializedObject.ApplyModifiedProperties();
        }

    }
}
