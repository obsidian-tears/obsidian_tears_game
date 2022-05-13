// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEditor;
using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Base class for wizards used by the Quest Editor window.
    /// </summary>
    public abstract class QuestEditorWizard
    {
        protected int clickedIndex { get; set; }

        public QuestEditorWizard(int clickedIndex)
        {
            this.clickedIndex = clickedIndex;
        }

        /// <summary>
        /// Draws the wizard.
        /// </summary>
        /// <returns>Returns false to stop drawing, true to continue drawing.</returns>
        public abstract bool Draw();

        protected QuestNode GetParentNode(Quest quest)
        {
            if (quest == null) return null;
            return (0 <= clickedIndex && clickedIndex < quest.nodeList.Count) ? quest.nodeList[clickedIndex]
                : GetLastNodeBeforeSuccess(quest);
        }

        protected QuestNode GetLastNodeBeforeSuccess(Quest quest)
        {
            var node = quest.startNode;
            int safeguard = 0;
            while (node.childList.Find(child => child.nodeType == QuestNodeType.Success) == null && safeguard < 999)
            {
                safeguard++;
                node = node.childList[0];
            }
            return node;
        }

        protected void AddSuccessNode(Quest quest, QuestNode node)
        {
            if (node == null) return;
            foreach (var index in node.childIndexList)
            {
                if (!(0 <= index && index < quest.nodeList.Count)) continue;
                var checkNode = quest.nodeList[index];
                if (checkNode.nodeType == QuestNodeType.Success) return; // Already leads to success. Do nothing.
            }
            for (int i = 0; i < quest.nodeList.Count; i++)
            {
                if (quest.nodeList[i].nodeType == QuestNodeType.Success)
                {
                    node.childIndexList.Add(i); // Add link to existing success node.
                    return;
                }
            }
            // Otherwise add new success node:
            var successNode = new QuestNode(new StringField("Success"), new StringField(), QuestNodeType.Success);
            quest.nodeList.Add(successNode);
            node.childIndexList.Add(quest.nodeList.Count - 1);
            successNode.canvasRect = new Rect(node.canvasRect.x, node.canvasRect.y + 10 + QuestNode.DefaultNodeHeight, QuestNode.DefaultNodeWidth, QuestNode.DefaultNodeHeight);
        }

        protected QuestContent CreateBodyContent(string text)
        {
            var content = BodyTextQuestContent.CreateInstance<BodyTextQuestContent>();
            content.name = "text";
            content.bodyText = new StringField(text);
            AddAndSaveSubasset(content);
            return content;
        }

        protected void AddAndSaveSubasset(QuestSubasset subasset)
        {
            if (QuestEditorWindow.selectedQuest != null)
            {
                AssetUtility.AddToAsset(subasset, QuestEditorWindow.selectedQuest);
                subasset.SetRuntimeReferences(QuestEditorWindow.selectedQuest, null);
            }
            QuestEditorWindow.UpdateSelectedQuestSerializedObject();
            QuestEditorWindow.ApplyModifiedPropertiesFromSelectedQuestSerializedObject();
            AssetDatabase.SaveAssets();
        }
    }
}

