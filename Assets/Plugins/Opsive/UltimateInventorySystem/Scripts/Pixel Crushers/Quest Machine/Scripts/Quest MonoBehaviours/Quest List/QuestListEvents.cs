// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.Events;
using System;

namespace PixelCrushers.QuestMachine
{

    [Serializable]
    public class QuestEvent : UnityEvent<Quest> { }

    [Serializable]
    public class QuestNodeEvent : UnityEvent<QuestNode> { }

    /// <summary>
    /// Exposes UnityEvents for QuestListContainer.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    [RequireComponent(typeof(QuestListContainer))]
    public class QuestListEvents : MonoBehaviour
    {

        public QuestEvent onAddQuest = new QuestEvent();

        public QuestEvent onRemoveQuest = new QuestEvent();

        public QuestEvent onQuestOfferable = new QuestEvent();

        public QuestEvent onQuestStateChange = new QuestEvent();

        public QuestNodeEvent onQuestNodeStateChange = new QuestNodeEvent();

        private QuestListContainer m_questListContainer;
        private QuestListContainer questListContainer {
            get { return m_questListContainer; }
            set { m_questListContainer = value; }
        }

        private void Awake()
        {
            questListContainer = GetComponent<QuestListContainer>();
        }

        private void OnEnable()
        {
            if (questListContainer == null) return;
            questListContainer.forwardEventsToListeners = true;
            questListContainer.questAdded += OnQuestAdded;
            questListContainer.questRemoved += OnQuestRemoved;
            questListContainer.questBecameOfferable += OnQuestBecameOfferable;
            questListContainer.questStateChanged += OnQuestStateChanged;
            questListContainer.questNodeStateChanged += OnQuestNodeStateChanged;
        }

        private void OnDisable()
        {
            if (questListContainer == null) return;
            questListContainer.questAdded -= OnQuestAdded;
            questListContainer.questRemoved -= OnQuestRemoved;
            questListContainer.questBecameOfferable -= OnQuestBecameOfferable;
            questListContainer.questStateChanged -= OnQuestStateChanged;
            questListContainer.questNodeStateChanged -= OnQuestNodeStateChanged;
        }

        private void OnQuestAdded(Quest quest)
        {
            onAddQuest.Invoke(quest);
        }

        private void OnQuestRemoved(Quest quest)
        {
            onRemoveQuest.Invoke(quest);
        }

        private void OnQuestBecameOfferable(Quest quest)
        {
            onQuestOfferable.Invoke(quest);
        }

        private void OnQuestStateChanged(Quest quest)
        {
            onQuestStateChange.Invoke(quest);
        }

        private void OnQuestNodeStateChanged(QuestNode questNode)
        {
            onQuestNodeStateChange.Invoke(questNode);
        }

    }
}