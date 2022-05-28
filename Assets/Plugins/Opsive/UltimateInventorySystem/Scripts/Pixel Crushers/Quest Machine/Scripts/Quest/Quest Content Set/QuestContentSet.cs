// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Contains a list of UI content elements.
    /// </summary>
    [Serializable]
    public class QuestContentSet
    {

        [SerializeField]
        private List<QuestContent> m_contentList = new List<QuestContent>();

        /// <summary>
        /// The content contained in this content set.
        /// </summary>
        public List<QuestContent> contentList
        {
            get { return m_contentList; }
            set { m_contentList = value; }
        }

        public void SetRuntimeReferences(Quest quest, QuestNode questNode)
        {
            if (contentList == null) return;
            for (int i = 0; i < contentList.Count; i++)
            {
                if (contentList[i] != null) contentList[i].SetRuntimeReferences(quest, questNode);
            }
        }

        public void DestroySubassets()
        {
            QuestSubasset.DestroyList(contentList);
        }

    }
}
