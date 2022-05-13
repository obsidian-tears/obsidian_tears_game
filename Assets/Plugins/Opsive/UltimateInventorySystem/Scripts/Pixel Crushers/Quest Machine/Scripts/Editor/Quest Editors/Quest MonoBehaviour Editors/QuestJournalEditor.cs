// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Custom inspector for QuestJournal.
    /// </summary>
    [CustomEditor(typeof(QuestJournal), true)]
    public class QuestJournalEditor : QuestListContainerEditor
    {

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawID("Quester identity.");
            DrawUI();

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }

        private void DrawUI()
        {
            QuestEditorPrefs.questListUIFoldout = QuestEditorUtility.EditorGUILayoutFoldout("UI Settings", "UIs to use.", QuestEditorPrefs.questListUIFoldout);
            if (!QuestEditorPrefs.questListUIFoldout) return;

            try
            {
                QuestEditorUtility.EditorGUILayoutBeginGroup();
                var questJournalUIProperty = serializedObject.FindProperty("m_questJournalUI");
                var questHUDProperty = serializedObject.FindProperty("m_questHUD");
                var onlyTrackOneQuestAtATimeProperty = serializedObject.FindProperty("m_onlyTrackOneQuestAtATime");
                if (questJournalUIProperty != null) IQuestJournalUIInspectorFieldAttributeDrawer.DoLayout(questJournalUIProperty,
                    new GUIContent("Quest Journal UI", "The Quest Journal UI to use. If unassigned, use the default journal UI."));
                if (questHUDProperty != null) IQuestHUDInspectorFieldAttributeDrawer.DoLayout(questHUDProperty,
                    new GUIContent("Quest HUD", "The Quest HUD to use. If unassigned, use the default HUD."));
                if (onlyTrackOneQuestAtATimeProperty != null) EditorGUILayout.PropertyField(onlyTrackOneQuestAtATimeProperty);
            }
            finally
            {
                QuestEditorUtility.EditorGUILayoutEndGroup();
            }
        }

        protected override void DrawSaveSettings()
        {
            base.DrawSaveSettings();
            if (QuestEditorPrefs.questListContainerSaveSettingsFoldout)
            {
                var rememberProperty = serializedObject.FindProperty("m_rememberCompletedQuests");
                if (rememberProperty != null) EditorGUILayout.PropertyField(rememberProperty);
            }
        }
    }
}
