// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    [Serializable]
    public class BasicUIContent
    {
        [SerializeField]
        private Sprite m_icon = null;
        [SerializeField]
        private StringField m_headingText = null;
        [SerializeField]
        private StringField m_bodyText = null;

        public Sprite icon
        {
            get { return m_icon; }
            set { m_icon = value; }
        }

        public StringField headingText
        {
            get { return m_headingText; }
            set { m_headingText = value; }
        }

        public StringField bodyText
        {
            get { return m_bodyText; }
            set { m_bodyText = value; }
        }

        private List<QuestContent> m_contentList = null;

        public List<QuestContent> contentList
        {
            get
            {
                if (m_contentList == null) m_contentList = CreateContentList();
                return m_contentList;
            }
        }

        private List<QuestContent> CreateContentList()
        {
            var list = new List<QuestContent>();
            if (icon != null)
            {
                var iconContent = ScriptableObjectUtility.CreateScriptableObject<IconQuestContent>();
                iconContent.name = "Icon";
                iconContent.image = icon;
                list.Add(iconContent);
            }
            if (!StringField.IsNullOrEmpty(m_headingText))
            {
                var headingContent = ScriptableObjectUtility.CreateScriptableObject<HeadingTextQuestContent>();
                headingContent.name = "Heading";
                headingContent.headingLevel = 1;
                headingContent.headingText = headingText;
                list.Add(headingContent);
            }
            if (!StringField.IsNullOrEmpty(bodyText))
            {
                var bodyContent = ScriptableObjectUtility.CreateScriptableObject<BodyTextQuestContent>();
                bodyContent.name = "Body";
                bodyContent.bodyText = bodyText;
                list.Add(bodyContent);
            }
            return list;
        }

        public void DestroyContentList()
        {
            if (m_contentList != null)
            {
                for (int i = 0; i < m_contentList.Count; i++)
                {
                    var content = m_contentList[i];
                    if (content == null) continue;
                    ScriptableObject.Destroy(content);
                }
            }
            m_contentList = null;
        }

    }

}
