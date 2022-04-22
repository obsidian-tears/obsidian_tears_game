// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Abstract base class for UI content.
    /// </summary>
    public abstract class QuestContent : QuestSubasset
    {

        [HideInInspector]
        [SerializeField]
        private int m_contentID = -1;

        public int contentID
        {
            get { return m_contentID; }
            set { m_contentID = value; }
        }

        public virtual StringField originalText
        {
            get { return StringField.empty; }
            set { }
        }

        public virtual string runtimeText { get { return QuestMachineTags.ReplaceTags(originalText, quest); } }

        public static void SetRuntimeReferences(List<QuestContent> contentList, Quest quest, QuestNode questNode)
        {
            if (contentList == null) return;
            for (int i = 0; i < contentList.Count; i++)
            {
                if (contentList[i] != null) contentList[i].SetRuntimeReferences(quest, questNode);
            }
        }

        public override void AddTagsToDictionary()
        {
            AddTagsToDictionary(originalText);
        }

    }

}
