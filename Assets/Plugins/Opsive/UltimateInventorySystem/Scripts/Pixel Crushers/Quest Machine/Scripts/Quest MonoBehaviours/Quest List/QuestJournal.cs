// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// This subclass of QuestList provides facilities to show the list in a QuestJournalUI.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class QuestJournal : IdentifiableQuestListContainer, IMessageHandler
    {

        [Tooltip("The Quest Journal UI to use. If unassigned, use the QuestMachine's default UI.")]
        [SerializeField]
        [IQuestJournalUIInspectorField]
        private UnityEngine.Object m_questJournalUI;

        [Tooltip("The Quest HUD to use. If unassigned, use the QuestMachine's default HUD.")]
        [SerializeField]
        [IQuestHUDInspectorField]
        private UnityEngine.Object m_questHUD;

        [Tooltip("Keep completed quests in the journal.")]
        [SerializeField]
        private bool m_rememberCompletedQuests = true;

        [Tooltip("If tracking is enabled for a quest, disable tracking for other quests.")]
        [SerializeField]
        private bool m_onlyTrackOneQuestAtATime;

        /// <summary>
        /// The quest journal UI to use. If not set, defaults to the QuestMachine's default UI.
        /// </summary>
        public IQuestJournalUI questJournalUI
        {
            get { return (m_questJournalUI != null) ? m_questJournalUI as IQuestJournalUI : QuestMachine.defaultQuestJournalUI; }
            set { m_questJournalUI = value as UnityEngine.Object; }
        }

        /// <summary>
        /// The quest HUD to use. If not set, defaults to the QuestMachine's default UI.
        /// </summary>
        public IQuestHUD questHUD
        {
            get { return (m_questHUD != null) ? m_questHUD as IQuestHUD : QuestMachine.defaultQuestHUD; }
            set { m_questHUD = value as UnityEngine.Object; }
        }

        /// <summary>
        /// Keep completed quests in the journal.
        /// </summary>
        public bool rememberCompletedQuests
        {
            get { return m_rememberCompletedQuests; }
            set { m_rememberCompletedQuests = value; }
        }

        /// <summary>
        /// If tracking is enabled for a quest, disable tracking for other quests.
        /// </summary>
        public bool onlyTrackOneQuestAtATime
        {
            get { return m_onlyTrackOneQuestAtATime; }
            set { m_onlyTrackOneQuestAtATime = value; }
        }

        public override void Reset()
        {
            base.Reset();
            includeInSavedGameData = true;
        }

        public override void Start()
        {
            base.Start();
            RepaintUIs();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            if (!includeInSavedGameData) SaveSystem.UnregisterSaver(this);
            MessageSystem.AddListener(this, QuestMachineMessages.QuestStateChangedMessage, string.Empty);
            MessageSystem.AddListener(this, QuestMachineMessages.QuestCounterChangedMessage, string.Empty);
            MessageSystem.AddListener(this, QuestMachineMessages.RefreshUIsMessage, string.Empty);
            MessageSystem.AddListener(this, QuestMachineMessages.QuestTrackToggleChangedMessage, string.Empty);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            MessageSystem.RemoveListener(this);
        }

        public override void OnMessage(MessageArgs messageArgs)
        {
            base.OnMessage(messageArgs);
            switch (messageArgs.message)
            {
                case QuestMachineMessages.QuestStateChangedMessage:
                    CheckQuestState(messageArgs);
                    RepaintUIs();
                    break;
                case QuestMachineMessages.QuestCounterChangedMessage:
                case QuestMachineMessages.RefreshUIsMessage:
                    RepaintUIs();
                    break;
                case QuestMachineMessages.QuestTrackToggleChangedMessage:
                    var refreshed = CheckTrackingToggles(messageArgs.parameter);
                    if (!refreshed) RepaintUIs();
                    break;
            }
        }

        /// <summary>
        /// If quest was completed and rememberCompletedQuests is false,
        /// remove the quest.
        /// </summary>
        /// <param name="messageArgs"></param>
        protected virtual void CheckQuestState(MessageArgs messageArgs)
        {
            if (!rememberCompletedQuests)
            {
                if (messageArgs.firstValue == null &&
                    (messageArgs.values.Length >= 2 && messageArgs.values[1] != null &&
                    messageArgs.values[1].GetType() == typeof(QuestState)))
                {
                    var state = (QuestState)(messageArgs.values[1]);
                    if (state == QuestState.Successful || state == QuestState.Failed)
                    {
                        DeleteQuest(FindQuest(messageArgs.parameter));
                    }
                }
            }
        }

        /// <summary>
        /// Show the quest journal.
        /// </summary>
        public virtual void ShowJournalUI()
        {
            if (questJournalUI != null) questJournalUI.Show(this);
        }

        /// <summary>
        /// Hide the quest journal.
        /// </summary>
        public virtual void HideJournalUI()
        {
            if (questJournalUI != null) questJournalUI.Hide();
        }

        /// <summary>
        /// Toggle visibility of the journal.
        /// </summary>
        public virtual void ToggleJournalUI()
        {
            if (questJournalUI != null) questJournalUI.Toggle(this);
        }

        /// <summary>
        /// If onlyTrackOneQuestAtATime is true and the specified quest is
        /// now being tracked, disable tracking on other quests.
        /// Returns true if any quest tracking changed.
        /// </summary>
        public virtual bool CheckTrackingToggles(string questID)
        {
            if (!onlyTrackOneQuestAtATime) return false;
            var quest = FindQuest(questID);
            if (quest == null || !quest.showInTrackHUD) return false;
            var changed = false;
            for (int i = 0; i < questList.Count; i++)
            {
                if (questList[i] == null || !questList[i].showInTrackHUD || Equals(questList[i].id, questID)) continue;
                questList[i].showInTrackHUD = false;
                changed = true;
            }
            if (changed) QuestMachineMessages.RefreshUIs(quest);
            return changed;
        }

        public virtual void AbandonQuest(Quest quest)
        {
            if (quest == null || !quest.isAbandonable) return;
            if (quest.rememberIfAbandoned)
            {
                quest.SetState(QuestState.Abandoned);
            }
            else
            {
                quest.ExecuteStateActions(QuestState.Abandoned);
                DeleteQuest(quest.id);
            }
            QuestMachineMessages.QuestAbandoned(this, quest.id);
            if (questJournalUI != null && questJournalUI.isVisible) questJournalUI.SelectQuest(null);
            RepaintUIs();
        }

        public override Quest AddQuest(Quest quest)
        {
            if (quest == null) return null;
            var result = base.AddQuest(quest);
            CheckTrackingToggles(StringField.GetStringValue(quest.id));
            return result;
        }

        public override void ApplyData(string data)
        {
            base.ApplyData(data);
            RepaintUIs();
        }

        public virtual void RepaintUIs()
        {
            if (questJournalUI != null) questJournalUI.Repaint(this);
            if (questHUD != null) questHUD.Repaint(this);
        }

    }
}