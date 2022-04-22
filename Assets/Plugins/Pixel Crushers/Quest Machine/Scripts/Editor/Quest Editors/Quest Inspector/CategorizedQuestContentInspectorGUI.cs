// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Editor GUI for a set of UI content lists (dialogue, journal, HUD, alert)
    /// which is typically part of a QuestStateInfo object.
    /// </summary>
    public class CategorizedQuestContentInspectorGUI
    {

        private QuestContentListInspectorGUI m_dialogueContentListDrawer = null;
        private QuestContentListInspectorGUI m_journalContentListDrawer = null;
        private QuestContentListInspectorGUI m_hudContentListDrawer = null;

        public CategorizedQuestContentInspectorGUI()
        {
            m_dialogueContentListDrawer = new QuestContentListInspectorGUI(new GUIContent("Dialogue Text", "Content shown in the dialogue UI."), QuestContentCategory.Dialogue);
            m_journalContentListDrawer = new QuestContentListInspectorGUI(new GUIContent("Journal Text", "Content shown in the player's quest journal."), QuestContentCategory.Journal);
            m_hudContentListDrawer = new QuestContentListInspectorGUI(new GUIContent("HUD Text", "Content shown in the player's quest tracking HUD."), QuestContentCategory.HUD);
        }

        public void Draw(SerializedObject serializedObject, SerializedProperty categorizedContentListProperty)
        {
            UnityEngine.Assertions.Assert.IsNotNull(serializedObject, "Quest Machine: Internal error - SerializedObject is null in CategorizedQuestContentInspectorGUI.");
            UnityEngine.Assertions.Assert.IsNotNull(categorizedContentListProperty, "Quest Machine: Internal error - categorizedUIContentListProperty is null.");
            if (serializedObject == null || categorizedContentListProperty == null) return;

            // Get references to content lists:
            ValidateCategorizedContentListSize(categorizedContentListProperty);
            var dialogueContentList = categorizedContentListProperty.GetArrayElementAtIndex((int)QuestContentCategory.Dialogue).FindPropertyRelative("m_contentList");
            var journalContentList = categorizedContentListProperty.GetArrayElementAtIndex((int)QuestContentCategory.Journal).FindPropertyRelative("m_contentList");
            var hudContentList = categorizedContentListProperty.GetArrayElementAtIndex((int)QuestContentCategory.HUD).FindPropertyRelative("m_contentList");
            UnityEngine.Assertions.Assert.IsNotNull(dialogueContentList, "Quest Machine: Internal error - Dialogue UI content list is null.");
            UnityEngine.Assertions.Assert.IsNotNull(journalContentList, "Quest Machine: Internal error - Journal UI content list is null.");
            UnityEngine.Assertions.Assert.IsNotNull(hudContentList, "Quest Machine: Internal error - HUD UI content list is null.");
            if (dialogueContentList == null || journalContentList == null || hudContentList == null) return;

            // Draw content lists:
            m_dialogueContentListDrawer.Draw(serializedObject, dialogueContentList, true);
            m_journalContentListDrawer.Draw(serializedObject, journalContentList, true);
            m_hudContentListDrawer.Draw(serializedObject, hudContentList, true);
        }

        private void ValidateCategorizedContentListSize(SerializedProperty categorizedContentListProperty)
        {
            if (categorizedContentListProperty.arraySize < QuestStateInfo.NumContentCategories)
            {
                categorizedContentListProperty.arraySize = QuestStateInfo.NumContentCategories;
            }
        }
    }

}
