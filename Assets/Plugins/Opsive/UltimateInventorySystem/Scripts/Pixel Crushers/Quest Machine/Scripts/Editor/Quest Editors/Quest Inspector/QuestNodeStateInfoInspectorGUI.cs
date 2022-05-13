// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Editor GUI for a QuestStateInfo object inside a QuestNode. This is different
    /// from QuestStateInfoInspectorGUI, which draws the QuestStateInfo for a
    /// Quest, so it can record the foldout state for each node.
    /// </summary>
    public class QuestNodeStateInfoInspectorGUI
    {

        private CategorizedQuestContentInspectorGUI m_categorizedContentDrawer = null;
        private QuestActionListInspectorGUI m_questActionListDrawer = null;

        public void Draw(SerializedObject serializedObject, SerializedProperty stateInfoProperty, int nodeIndex, QuestNodeState questNodeState)
        {
            if (serializedObject == null || stateInfoProperty == null) return;

            var foldout = QuestEditorPrefs.GetQuestNodeStateFoldout(questNodeState, nodeIndex);
            var newFoldout = QuestEditorUtility.EditorGUILayoutFoldout(questNodeState.ToString(), string.Empty, foldout, false);
            if (newFoldout != foldout)
            {
                QuestEditorPrefs.ToggleQuestNodeStateFoldout(questNodeState, nodeIndex);
            }
            if (!newFoldout) return;

            if (m_categorizedContentDrawer == null) m_categorizedContentDrawer = new CategorizedQuestContentInspectorGUI();
            if (m_questActionListDrawer == null) m_questActionListDrawer = new QuestActionListInspectorGUI(new GUIContent("Actions", "Actions that run when the quest node enters this state."));
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
