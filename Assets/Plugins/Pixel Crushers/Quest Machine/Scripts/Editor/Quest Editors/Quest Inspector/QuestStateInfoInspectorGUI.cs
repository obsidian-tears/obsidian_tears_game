// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Inspector GUI for QuestStateInfo.
    /// </summary>
    public class QuestStateInfoInspectorGUI
    {

        private CategorizedQuestContentInspectorGUI m_categorizedContentDrawer = null;
        private QuestActionListInspectorGUI m_questActionListDrawer = null;

        public void Draw(SerializedObject serializedObject, SerializedProperty stateInfoProperty, int nodeIndex, QuestState questState)
        {
            if (serializedObject == null || stateInfoProperty == null) return;

            var foldout = QuestEditorPrefs.GetQuestStateFoldout(questState, nodeIndex);
            var newFoldout = QuestEditorUtility.EditorGUILayoutFoldout(questState.ToString(), string.Empty, foldout, false);
            if (newFoldout != foldout)
            {
                QuestEditorPrefs.ToggleQuestStateFoldout(questState, nodeIndex);
            }
            if (!newFoldout) return;

            if (m_categorizedContentDrawer == null) m_categorizedContentDrawer = new CategorizedQuestContentInspectorGUI();
            if (m_questActionListDrawer == null) m_questActionListDrawer = new QuestActionListInspectorGUI(new GUIContent("Actions", "Actions that run when the quest enters this state."));
            QuestEditorUtility.EditorGUILayoutBeginIndent();

            var categorizedContentListProperty = stateInfoProperty.FindPropertyRelative("m_categorizedContentList");
            m_categorizedContentDrawer.Draw(serializedObject, categorizedContentListProperty);

            var actionListProperty = stateInfoProperty.FindPropertyRelative("m_actionList");
            m_questActionListDrawer.Draw(actionListProperty);

            QuestEditorUtility.EditorGUILayoutEndIndent();
            EditorGUILayout.Space();

        }

    }

}
