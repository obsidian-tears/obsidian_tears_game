// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Manages quest indicators for an entity. Uses a QuestIndicatorUI to actually
    /// show the indicators.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class QuestIndicatorManager : MonoBehaviour, IMessageHandler
    {

        [Tooltip("Quest Indicator UI containing visual indicators for each indicator state. Can be a prefab.")]
        [SerializeField]
        private QuestIndicatorUI m_questIndicatorUI;

        [Tooltip("Show this state when the quest giver has a quest to offer.")]
        [SerializeField]
        private QuestIndicatorState m_hasQuestToOfferState = QuestIndicatorState.Offer;

        [Tooltip("Show this state when the quest giver only has quests whose offer conditions aren't met yet.")]
        [SerializeField]
        private QuestIndicatorState m_hasQuestButCannotOfferState = QuestIndicatorState.None;

        [Tooltip("Single player game. Check player's QuestJournal for active/completed quests.")]
        [SerializeField]
        private bool m_checkSinglePlayerJournal = true;

        /// <summary>
        /// Quest Indicator UI containing visual indicators for each indicator state. Can be a prefab.
        /// </summary>
        public QuestIndicatorUI questIndicatorUI
        {
            get { return m_questIndicatorUI; }
            set { m_questIndicatorUI = value; }
        }

        /// <summary>
        /// Show this state when the quest giver has a quest to offer.
        /// </summary>
        public QuestIndicatorState hasQuestToOfferState
        {
            get { return m_hasQuestToOfferState; }
            set { m_hasQuestToOfferState = value; }
        }

        /// <summary>
        /// Show this state when the quest giver only has quests whose offer conditions aren't met yet.
        /// </summary>
        public QuestIndicatorState hasQuestButCannotOfferState
        {
            get { return m_hasQuestButCannotOfferState; }
            set { m_hasQuestButCannotOfferState = value; }
        }

        /// <summary>
        /// Single player game. Check player's QuestJournal for active/completed quests.
        /// </summary>
        public bool checkSinglePlayerJournal
        {
            get { return m_checkSinglePlayerJournal; }
            set { m_checkSinglePlayerJournal = value; }
        }

        private string m_id = null;

        private List<string>[] m_states;

        private Coroutine m_refreshCoroutine = null;

        protected string myID
        {
            get { return m_id; }
            set { m_id = value; }
        }

        public List<string>[] states
        {
            get { return m_states; }
            set { m_states = value; }
        }

        protected virtual void Awake()
        {
            myID = StringField.GetStringValue(QuestMachineMessages.GetID(gameObject));
            InitializeQuestIndicatorUI();
            InitializeStates();
            RegisterWithMessageSystem();
        }

        protected virtual void OnDestroy()
        {
            UnregisterWithMessageSystem();
        }

        protected virtual void Start()
        {
            Repaint();
        }

        protected virtual void OnEnable()
        {
            Repaint();
        }

        protected virtual void OnDisable()
        {
            m_refreshCoroutine = null;
        }

        protected virtual void InitializeQuestIndicatorUI()
        {
            if (questIndicatorUI == null) questIndicatorUI = GetComponentInChildren<QuestIndicatorUI>();
            if (questIndicatorUI == null) return;
            if (questIndicatorUI.gameObject != this.gameObject && 
                (!questIndicatorUI.gameObject.activeInHierarchy || questIndicatorUI.transform.parent == null))
            {
                // Instantiate from prefab:
                questIndicatorUI = Instantiate(questIndicatorUI, transform.position, transform.rotation) as QuestIndicatorUI;
                questIndicatorUI.transform.SetParent(transform);
            }
        }

        protected virtual void RegisterWithMessageSystem()
        {
            MessageSystem.AddListener(this, QuestMachineMessages.SetIndicatorStateMessage, string.Empty);
            MessageSystem.AddListener(this, QuestMachineMessages.RefreshIndicatorMessage, string.Empty);
            MessageSystem.AddListener(this, QuestMachineMessages.RefreshUIsMessage, string.Empty);
        }

        protected virtual void UnregisterWithMessageSystem()
        {
            MessageSystem.RemoveListener(this, QuestMachineMessages.SetIndicatorStateMessage, string.Empty);
            MessageSystem.RemoveListener(this, QuestMachineMessages.RefreshIndicatorMessage, string.Empty);
            MessageSystem.RemoveListener(this, QuestMachineMessages.RefreshUIsMessage, string.Empty);
        }

        public virtual void OnMessage(MessageArgs messageArgs)
        {
            var target = messageArgs.targetString;
            if (!(string.IsNullOrEmpty(target) || string.Equals(target, myID) || StringField.Equals(QuestMachineMessages.GetID(target), myID))) return;
            switch (messageArgs.message)
            {
                case QuestMachineMessages.SetIndicatorStateMessage:
                    SetIndicatorState(messageArgs.parameter, (QuestIndicatorState)messageArgs.firstValue);
                    break;
                case QuestMachineMessages.RefreshIndicatorMessage:
                case QuestMachineMessages.RefreshUIsMessage:
                    Repaint();
                    break;
            }
        }

        protected virtual void InitializeStates()
        {
            var numStates = Enum.GetNames(typeof(QuestIndicatorState)).Length;
            states = new List<string>[numStates];
            for (int i = 0; i < numStates; i++)
            {
                states[i] = new List<string>();
            }
        }

        public virtual void SetIndicatorState(string questID, QuestIndicatorState state)
        {
            if (states == null) InitializeStates();
            for (int i = 0; i < states.Length; i++)
            {
                if (states != null && states[i].Contains(questID)) states[i].Remove(questID);
            }
            states[(int)state].Add(questID);
            ShowHighestPriorityIndicator();
        }

        public virtual void Repaint()
        {
            if (!(enabled && gameObject.activeInHierarchy)) return;
            if (m_refreshCoroutine == null) m_refreshCoroutine = StartCoroutine(RefreshAtEndOfFrame());
        }

        private IEnumerator RefreshAtEndOfFrame()
        {
            // Wait until end of frame so we only refresh once in case we receive multiple
            // requests to refresh during the same frame.
            yield return new WaitForEndOfFrame();
            m_refreshCoroutine = null;
            RefreshFromAllQuests();
        }

        public virtual void RefreshFromAllQuests()
        {
            QuestJournal playerQuestJournal = checkSinglePlayerJournal ? QuestMachine.GetQuestJournal() : null;
            InitializeStates();
            var allQuests = QuestMachine.GetAllQuestInstances();
            foreach (var kvp in allQuests)
            {
                var quests = kvp.Value;
                if (quests == null) continue;
                for (int i = 0; i < quests.Count; i++)
                {
                    var quest = quests[i];
                    if (quest == null) continue;
                    var questState = quest.GetState();
                    if (questState == QuestState.Active && quest.indicatorStates != null && quest.indicatorStates.ContainsKey(myID))
                    {
                        // If the quest specifies an indicator for me, record it:
                        var state = quest.indicatorStates[myID];
                        states[(int)state].Add(StringField.GetStringValue(quest.id));
                    }
                    else if (questState == QuestState.WaitingToStart && StringField.Equals(quest.questGiverID, myID) && string.IsNullOrEmpty(quest.questerID))
                    {
                        // Otherwise if it's offerable by me, record it unless single player already has it:
                        if (!(checkSinglePlayerJournal && DoesJournalHaveQuest(playerQuestJournal, quest)))
                        {
                            var state = quest.canOffer ? hasQuestToOfferState : hasQuestButCannotOfferState;
                            states[(int)state].Add(StringField.GetStringValue(quest.id));
                        }
                    }
                }
            }
            ShowHighestPriorityIndicator();
        }

        protected virtual bool DoesJournalHaveQuest(QuestJournal questJournal, Quest quest)
        {
            if (quest == null || questJournal == null) return false;
            var questInJournal = questJournal.FindQuest(quest.id);
            if (questInJournal == null) return false;
            var state = questInJournal.GetState();
            return (state == QuestState.Active) ||(state == QuestState.Successful && quest.timesAccepted < quest.maxTimes);
        }

        public virtual void ShowHighestPriorityIndicator()
        {
            if (questIndicatorUI == null) return;

            // Hide all indicators:
            questIndicatorUI.HideAllIndicators();

            // Then activate the highest priority indicator:
            if (states.Length > 0)
            {
                for (int i = states.Length - 1; i >= 0; i--)
                {
                    if (states[i].Count > 0)
                    {
                        questIndicatorUI.SetIndicator(i, true);
                        break;
                    }
                }
            }
        }

    }

}
