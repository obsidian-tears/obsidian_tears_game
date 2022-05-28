// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Sends a QuestAlert message with some UI content.
    /// </summary>
    public class AlertQuestAction : QuestAction
    {

        [SerializeField]
        private List<QuestContent> m_contentList = new List<QuestContent>();

        [SerializeField]
        private QuestContentProxy[] m_contentListSerializationProxy; // Temporary variable for proxy serialization.

        /// <summary>
        /// The content to show in the alert UI.
        /// </summary>
        public List<QuestContent> contentList
        {
            get { return m_contentList; }
            set { m_contentList = value; }
        }

        public override string GetEditorName()
        {
            var hasContent = (contentList != null && contentList.Count > 0 && contentList[0] != null);
            if (!hasContent) return "Alert";
            var firstContent = contentList[0].GetEditorName();
            return "Alert: " + firstContent + ((contentList.Count > 1) ? "..." : string.Empty);
        }

        public override void SetRuntimeReferences(Quest quest, QuestNode questNode)
        {
            base.SetRuntimeReferences(quest, questNode);
            for (int i = 0; i < contentList.Count; i++)
            {
                if (contentList[i] != null) contentList[i].SetRuntimeReferences(quest, questNode);
            }
        }

        public override void Execute()
        {
            if (quest != null)
            {
                QuestMachineMessages.QuestAlert(quest, quest.id, contentList);
            }
            else // We may be at the end of the quest, and it may have been removed, so pass null for the quest:
            {
                QuestMachineMessages.QuestAlert(null, StringField.empty, contentList);
            }
        }

        public override Sprite[] GetImages()
        {
            var images = new List<Sprite>();
            for (int i = 0; i < contentList.Count; i++)
            {
                if (contentList[i] == null) continue;
                var contentImages = contentList[i].GetImages();
                if (contentImages != null)
                {
                    images.AddRange(contentImages);
                }
            }
            return images.ToArray();
        }

        public override AudioClip[] GetAudioClips()
        {
            var audioClips = new List<AudioClip>();
            for (int i = 0; i < contentList.Count; i++)
            {
                if (contentList[i] == null) continue;
                var contentAudioClips = contentList[i].GetAudioClips();
                if (contentAudioClips != null)
                {
                    audioClips.AddRange(contentAudioClips);
                }
            }
            return audioClips.ToArray();
        }

        public override void OnBeforeProxySerialization()
        {
            base.OnBeforeProxySerialization();
            m_contentListSerializationProxy = QuestContentProxy.NewArray(contentList);
        }

        public override void OnAfterProxyDeserialization()
        {
            base.OnAfterProxyDeserialization();
            contentList = QuestContentProxy.CreateList(m_contentListSerializationProxy);
            m_contentListSerializationProxy = null; // After deserializing, free proxy memory.
        }

        public override void CloneSubassetsInto(QuestSubasset copy)
        {
            base.CloneSubassetsInto(copy);
            var copyAlertQuestAction = copy as AlertQuestAction;
            if (copyAlertQuestAction == null) return;
            copyAlertQuestAction.contentList = CloneList(contentList);
        }

        public override void DestroySubassets()
        {
            base.DestroySubassets();
            DestroyList(contentList);
        }

    }

}
