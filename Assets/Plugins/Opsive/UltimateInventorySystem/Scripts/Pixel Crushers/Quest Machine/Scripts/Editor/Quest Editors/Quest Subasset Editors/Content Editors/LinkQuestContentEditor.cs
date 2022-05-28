// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Custom inspector for LinkQuestContent assets.
    /// </summary>
    [CustomEditor(typeof(LinkQuestContent), true)]
    public class LinkQuestContentEditor : QuestSubassetEditor
    {
        private Quest m_quest;
        private string[] m_linkableText;
        private int[] m_linkableIDs;

        protected override void OnEnable()
        {
            base.OnEnable();
            IdentifyLinkableContent();
        }

        protected override void Draw()
        {
            if (serializedObject == null) return;
            var linkedContentIDProperty = serializedObject.FindProperty("m_linkedContentID");
            UnityEngine.Assertions.Assert.IsNotNull(linkedContentIDProperty, "Quest Machine: Internal error - m_linkedContentID is null.");
            if (linkedContentIDProperty == null) return;
            if (m_linkableText == null || m_linkableIDs == null)
            {
                EditorGUILayout.PropertyField(linkedContentIDProperty, true);
            }
            else
            {
                var index = GetLinkableTextIndex(linkedContentIDProperty.intValue);
                EditorGUI.BeginChangeCheck();
                index = EditorGUILayout.Popup("Link To:", index, m_linkableText);
                if (EditorGUI.EndChangeCheck())
                {
                    linkedContentIDProperty.intValue = GetLinkableTextID(index);
                }
            }
        }

        private int GetLinkableTextIndex(int id)
        {
            if (m_linkableIDs == null) return -1;
            for (int index = 0; index < m_linkableIDs.Length; index++)
            {
                if (m_linkableIDs[index] == id) return index;
            }
            return -1;
        }

        private int GetLinkableTextID(int index)
        {
            if (m_linkableIDs == null) return -1;
            if (0 <= index && index < m_linkableIDs.Length) return m_linkableIDs[index];
            return -1;
        }

        private void IdentifyLinkableContent()
        {
            var content = target as LinkQuestContent;
            if (content == null) return;
            m_quest = (content.quest != null) ? content.quest : QuestEditorWindow.selectedQuest;
            if (m_quest == null) return;

            var text = new List<string>();
            var ids = new List<int>();

            // Offer text:
            IdentifyLinkableContentInList("Offer", m_quest.offerContentList, text, ids);
            IdentifyLinkableContentInList("Offer Unmet", m_quest.offerConditionsUnmetContentList, text, ids);

            // Main info:
            for (int i = 0; i <= (int)QuestState.Abandoned; i++)
            {
                IdentifyLinkableContentInStateInfo("Main", m_quest.GetStateInfo((QuestState)i), text, ids);
            }

            // Nodes:
            for (int i = 0; i < m_quest.nodeList.Count; i++)
            {
                var node = m_quest.nodeList[i];
                if (node == null) continue;
                var heading = "[" + i + "]";
                if (!StringField.IsNullOrEmpty(node.id)) heading += " " + StringField.GetStringValue(node.id);
                IdentifyLinkableContentInNode(heading, node, text, ids);
            }

            m_linkableText = text.ToArray();
            m_linkableIDs = ids.ToArray();
        }

        private void IdentifyLinkableContentInNode(string heading, QuestNode node, List<string> text, List<int> ids)
        {
            if (node == null) return;
            IdentifyLinkableContentInStateInfo(heading, node.GetStateInfo(QuestNodeState.Inactive), text, ids);
            IdentifyLinkableContentInStateInfo(heading, node.GetStateInfo(QuestNodeState.Active), text, ids);
            IdentifyLinkableContentInStateInfo(heading, node.GetStateInfo(QuestNodeState.True), text, ids);
        }

        private void IdentifyLinkableContentInStateInfo(string heading, QuestStateInfo stateInfo, List<string> text, List<int> ids)
        {
            IdentifyLinkableContentInList(heading + " Dialogue", stateInfo.GetContentList(QuestContentCategory.Dialogue), text, ids);
            IdentifyLinkableContentInList(heading + " Journal", stateInfo.GetContentList(QuestContentCategory.Journal), text, ids);
            IdentifyLinkableContentInList(heading + " HUD", stateInfo.GetContentList(QuestContentCategory.HUD), text, ids);
        }

        private void IdentifyLinkableContentInList(string heading, List<QuestContent> contentList, List<string> text, List<int> ids)
        {
            if (contentList == null) return;
            for (int i = 0; i < contentList.Count; i++)
            {
                var content = contentList[i];
                if (content == null || content is LinkQuestContent) continue;
                if (content.contentID == -1) m_quest.AssignContentID(content);
                var popupText = RemoveForwardSlashes(heading) + "/" + RemoveForwardSlashes(content.GetEditorName());
                text.Add(popupText);
                ids.Add(content.contentID);
            }
        }

        private string RemoveForwardSlashes(string s) // To prevent submenus.
        {
            return (s != null) ? s.Replace("/", "\u2215") : string.Empty;
        }

    }
}
