// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Custom inspector for QuestGiver.
    /// </summary>
    [CustomEditor(typeof(QuestGiver), true)]
    public class QuestGiverEditor : QuestListContainerEditor
    {

        public override void OnInspectorGUI()
        {
            DrawDebugFoldout();

            serializedObject.Update();
            DrawID("Quest giver identity.");
            QuestEditorUtility.EditorGUILayoutVerticalSpace(2);
            DrawSaveSettings();
            QuestEditorUtility.EditorGUILayoutVerticalSpace(2);
            DrawOtherSettings();
            QuestEditorUtility.EditorGUILayoutVerticalSpace(2);
            DrawDialogueContent();
            QuestEditorUtility.EditorGUILayoutVerticalSpace(2);
            DrawQuestList();
            serializedObject.ApplyModifiedProperties();
            DrawSelectedQuestInspector();
        }

        protected virtual void DrawDialogueContent()
        {
            QuestEditorPrefs.questGiverDialogueContentFoldout = QuestEditorUtility.EditorGUILayoutFoldout("Dialogue Content", "Quest giver-specific dialogue content.", QuestEditorPrefs.questGiverDialogueContentFoldout);
            if (!QuestEditorPrefs.questGiverDialogueContentFoldout) return;

            try
            {
                QuestEditorUtility.EditorGUILayoutBeginGroup();
                var questDialogueUIProperty = serializedObject.FindProperty("m_questDialogueUI");
                var noQuestsContentsProperty = serializedObject.FindProperty("m_noQuestsUIContents");
                var offerableContentsProperty = serializedObject.FindProperty("m_offerableQuestsUIContents");
                var activeContentsProperty = serializedObject.FindProperty("m_activeQuestsUIContents");
                var completedModeProperty = serializedObject.FindProperty("m_completedQuestDialogueMode");
                if (questDialogueUIProperty != null) IQuestDialogueUIInspectorFieldAttributeDrawer.DoLayout(questDialogueUIProperty,
                    new GUIContent("Quest Dialogue UI", "The Quest Dialogue UI to use when conversing with the player. If unassigned, uses the default dialogue UI."));
                if (completedModeProperty != null) EditorGUILayout.PropertyField(completedModeProperty, true);
                if (noQuestsContentsProperty != null) EditorGUILayout.PropertyField(noQuestsContentsProperty, true); //noQuestsContentGUI.Draw(serializedObject, noQuestsContentsProperty, false);
                if (offerableContentsProperty != null) EditorGUILayout.PropertyField(offerableContentsProperty, true); //offerableQuestsContentGUI.Draw(serializedObject, offerableContentsProperty, false);
                if (activeContentsProperty != null) EditorGUILayout.PropertyField(activeContentsProperty, true); //activeQuestsContentGUI.Draw(serializedObject, activeContentsProperty, false);
            }
            finally
            {
                QuestEditorUtility.EditorGUILayoutEndGroup();
            }
        }

        protected override void DrawOtherSettingsInterior()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_cooldownCheckFrequency"));
            base.DrawOtherSettingsInterior();
        }

    }

}
