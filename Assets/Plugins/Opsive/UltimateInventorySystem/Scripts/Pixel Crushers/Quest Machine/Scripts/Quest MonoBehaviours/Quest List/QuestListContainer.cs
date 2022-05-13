// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Maintains a list of quests on a GameObject.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class QuestListContainer : Saver, IMessageHandler
    {

        #region Serialized Fields

        [Tooltip("Forward quest state events to listeners that have registered for events such as questBecameOfferable, questStateChanged, and questNodeStateChanged.")]
        [SerializeField]
        private bool m_forwardEventsToListeners = false;

        [Tooltip("Include in saved game data.")]
        [SerializeField]
        private bool m_includeInSavedGameData = false;

        [Tooltip("If quests were added after game was saved, add them to quest list when loading saved game.")]
        [SerializeField]
        private bool m_addNewQuestsSinceSavedGame = false;

        [Tooltip("The current quest list. At runtime, these are runtime instances of quests.")]
        [SerializeField]
        private List<Quest> m_questList = new List<Quest>();

        [Tooltip("IDs of static quests that have been deleted and shouldn't be instantiated.")]
        [SerializeField]
        private List<string> m_deletedStaticQuests = new List<string>();

        #endregion

        #region Property Accessors to Serialized Fields

        /// <summary>
        /// Forward quest state events to listeners that have registered to events such as
        /// questBecameOfferable, questStateChanged, and questNodeStateChanged.
        /// </summary>
        public bool forwardEventsToListeners
        {
            get { return m_forwardEventsToListeners; }
            set { m_forwardEventsToListeners = value; }
        }

        /// <summary>
        /// Include in saved game data, which is used for saved games and scene persistence.
        /// </summary>
        public bool includeInSavedGameData
        {
            get { return m_includeInSavedGameData; }
            set { m_includeInSavedGameData = value; }
        }

        /// <summary>
        /// If quests were added after game was saved, add them to quest list when loading saved game.
        /// </summary>
        public bool addNewQuestsSinceSavedGame
        {
            get { return m_addNewQuestsSinceSavedGame; }
            set { m_addNewQuestsSinceSavedGame = value; }
        }

        /// <summary>
        /// Quest assets.
        /// </summary>
        public List<Quest> questList
        {
            get { return m_questList; }
            protected set { m_questList = value; }
        }

        /// <summary>
        /// IDs of static quests that have been deleted and shouldn't be instantiated.
        /// </summary>
        public List<string> deletedStaticQuests
        {
            get { return m_deletedStaticQuests; }
            protected set { m_deletedStaticQuests = value; }
        }

        #endregion

        #region Runtime Properties

        /// <summary>
        /// The original design-time quest list.
        /// </summary>
        protected List<Quest> originalQuestList { get; set; }

        /// <summary>
        /// Raised when a quest is added to the list.
        /// </summary>
        public event QuestParameterDelegate questAdded = delegate { };

        /// <summary>
        /// Raised when a quest is removed from the list.
        /// </summary>
        public event QuestParameterDelegate questRemoved = delegate { };

        /// <summary>
        /// Raised when a quest in the list becomes offerable.
        /// </summary>
        public event QuestParameterDelegate questBecameOfferable = delegate { };

        /// <summary>
        /// Raised when the state of a quest in the list changes.
        /// </summary>
        public event QuestParameterDelegate questStateChanged = delegate { };

        /// <summary>
        /// Raised when the state of a quest node in a quest in the list changes.
        /// </summary>
        public event QuestNodeParameterDelegate questNodeStateChanged = delegate { };

        /// <summary>
        /// This coroutine runs when delaying quest runtime startup until the end of frame.
        /// </summary>
        protected Coroutine m_delayedStartupCoroutine = null;

        /// <summary>
        /// The delayed startup coroutine should do runtime startup for quests in this list.
        /// </summary>
        protected List<Quest> m_runtimeStartupQueue = new List<Quest>();

        #endregion

        #region Initialization

        public override void Reset()
        {
            base.Reset();
            includeInSavedGameData = true;
            saveAcrossSceneChanges = true;
        }

        public override void Awake()
        {
            base.Awake();
            originalQuestList = questList;
            InstantiateQuestAssets();
        }

        public override void OnDestroy()
        {
            DestroyQuestInstances();
            base.OnDestroy();
        }

        /// <summary>
        /// Instantiates copies of quest assets into the runtime
        /// quest list and enables their autostart and offer condition checking.
        /// </summary>
        protected void InstantiateQuestAssets()
        {
            questList = new List<Quest>();
            AddQuests(originalQuestList);
        }

        public void DestroyQuestInstances()
        {
            if (questList == null || questList.Count == 0) return;
            for (int i = questList.Count - 1; i >= 0; i--)
            {
                DeleteQuest(questList[i]);
            }
        }

        /// <summary>
        /// Resets the quest list container to its original list.
        /// </summary>
        public virtual void ResetToOriginalState()
        {
            DestroyQuestInstances();
            deletedStaticQuests.Clear();
            InstantiateQuestAssets();
        }

        #endregion

        #region Add/Remove Quests

        /// <summary>
        /// Adds a list of quests to the quest giver's quest list and runs their runtime startup.
        /// </summary>
        public virtual void AddQuests(List<Quest> listToAdd)
        {
            if (listToAdd == null) return;
            for (int i = 0; i < listToAdd.Count; i++)
            {
                AddQuest(listToAdd[i]);
            }
        }

        /// <summary>
        /// Adds a quest to the quest giver's quest list and runs its runtime startup.
        /// </summary>
        /// <param name="quest"></param>
        public virtual Quest AddQuest(Quest quest)
        {
            return AddQuest(quest, false);
        }

        /// <summary>
        /// Adds a quest to the quest giver's quest list and optionally runs its runtime
        /// startup immediately or waits until the end of frame to allow other quests to
        /// be added to their respective quest lists in case a quest's offer conditions
        /// reference the state of another quest.
        /// </summary>
        /// <param name="delayStartup">If true, delay runtime startup until end of frame; otherwise run runtime startup immediately.</param>
        public virtual Quest AddQuest(Quest quest, bool delayStartup)
        {
            if (quest == null) return null;
            if (deletedStaticQuests.Contains(StringField.GetStringValue(quest.id))) return null;
            var instance = quest.isAsset ? quest.Clone() : quest;
            if (instance == null) return null;
            questList.Add(instance);
            QuestMachine.RegisterQuestInstance(instance);
            RegisterForQuestEvents(instance);
            if (delayStartup)
            {
                m_runtimeStartupQueue.Add(instance);
                if (m_delayedStartupCoroutine == null)
                {
                    m_delayedStartupCoroutine = StartCoroutine(RuntimeStartupAtEndOfFrame());
                }
            }
            else
            {
                instance.RuntimeStartup();
            }
            return instance;
        }

        protected IEnumerator RuntimeStartupAtEndOfFrame()
        {
            var isLoadingGame = QuestMachine.isLoadingGame;
            yield return new WaitForEndOfFrame();
            try
            {
                var prevIsLoadingGame = QuestMachine.isLoadingGame;
                QuestMachine.isLoadingGame = isLoadingGame;
                for (int i = 0; i < m_runtimeStartupQueue.Count; i++)
                {
                    m_runtimeStartupQueue[i].RuntimeStartup();
                }
                QuestMachine.isLoadingGame = prevIsLoadingGame;
            }
            finally
            {
                m_runtimeStartupQueue.Clear();
                m_delayedStartupCoroutine = null;
            }
        }

        public virtual Quest FindQuest(string questID)
        {
            if (string.IsNullOrEmpty(questID)) return null;
            for (int i = 0; i < questList.Count; i++)
            {
                var quest = questList[i];
                if (quest == null) continue;
                if (string.Equals(questID, StringField.GetStringValue(quest.id))) return quest;
            }
            return null;
        }

        public virtual Quest FindQuest(StringField questID)
        {
            return FindQuest(StringField.GetStringValue(questID));
        }

        public virtual bool ContainsQuest(string questID)
        {
            return FindQuest(questID) != null;
        }

        public virtual bool ContainsQuest(StringField questID)
        {
            return FindQuest(questID) != null;
        }

        public virtual void DeleteQuest(StringField questID)
        {
            DeleteQuest(FindQuest(questID));
        }

        public virtual void DeleteQuest(Quest quest)
        {
            if (quest == null) return;
            questList.Remove(quest);
            if (!quest.isProcedurallyGenerated)
            {
                var questID = StringField.GetStringValue(quest.id);
                if (!deletedStaticQuests.Contains(questID)) deletedStaticQuests.Add(questID);
            }
            UnregisterForQuestEvents(quest);
            QuestMachine.UnregisterQuestInstance(quest);
            Quest.DestroyInstance(quest);
        }

        public virtual void RegisterForQuestEvents(Quest quest)
        {
            if (quest == null) return;
            if (quest.hasOfferConditions && quest.GetState() != QuestState.Active) MessageSystem.AddListener(this, "Check Offer Conditions", quest.id);
            if (!forwardEventsToListeners) return;
            questAdded(quest);
            quest.questOfferable += OnQuestBecameOfferable;
            quest.stateChanged += OnQuestStateChanged;
            for (int i = 0; i < quest.nodeList.Count; i++)
            {
                quest.nodeList[i].stateChanged += OnQuestNodeStateChanged;
            }
        }

        public virtual void UnregisterForQuestEvents(Quest quest)
        {
            if (quest == null) return;
            if (quest.hasOfferConditions) MessageSystem.RemoveListener(this, QuestMachineMessages.CheckOfferConditionsMessage, quest.id);
            if (!forwardEventsToListeners) return;
            questRemoved(quest);
            quest.questOfferable -= OnQuestBecameOfferable;
            quest.stateChanged -= OnQuestStateChanged;
            for (int i = 0; i < quest.nodeList.Count; i++)
            {
                quest.nodeList[i].stateChanged -= OnQuestNodeStateChanged;
            }
        }

        public virtual void OnMessage(MessageArgs messageArgs)
        {
            if (String.Equals(messageArgs.message, QuestMachineMessages.CheckOfferConditionsMessage))
            {
                var quest = FindQuest(messageArgs.parameter);
                if (quest != null && quest.hasOfferConditions)
                {
                    var state = quest.GetState();
                    if (state == QuestState.WaitingToStart || state == QuestState.Disabled)
                    {
                        quest.offerConditionSet.StopChecking();
                        quest.offerConditionSet.StartChecking(quest.BecomeOfferable);
                        if (!quest.offerConditionSet.areConditionsMet) quest.BecomeUnofferable();
                    }
                }
            }
        }

        public virtual void OnQuestBecameOfferable(Quest quest)
        {
            questBecameOfferable(quest);
        }

        public virtual void OnQuestStateChanged(Quest quest)
        {
            questStateChanged(quest);
        }

        public virtual void OnQuestNodeStateChanged(QuestNode questNode)
        {
            questNodeStateChanged(questNode);
        }

        #endregion

        #region Save/Load

        [Serializable]
        public class SaveData
        {
            public List<string> staticQuestIds = new List<string>();
            public List<ByteData> staticQuestData = new List<ByteData>();

            public List<string> proceduralQuests = new List<string>();

            public List<string> deletedStaticQuests = new List<string>();
        }

        public override string RecordData()
        {
            if (!includeInSavedGameData) return string.Empty;
            var saveData = new SaveData();
            for (int i = 0; i < questList.Count; i++)
            {
                var quest = questList[i];
                if (quest == null) continue;
                if (quest.isProcedurallyGenerated)
                {
                    saveData.proceduralQuests.Add(JsonUtility.ToJson(new QuestProxy(quest)));
                }
                else
                {
                    saveData.staticQuestIds.Add(StringField.GetStringValue(quest.id));
                    var bytes = QuestStateSerializer.Serialize(quest);
                    saveData.staticQuestData.Add(new ByteData(bytes));
                }
            }
            saveData.deletedStaticQuests.AddRange(deletedStaticQuests);
            return SaveSystem.Serialize(saveData);
        }

        public override void ApplyData(string data)
        {
            if (!includeInSavedGameData) return;
            if (string.IsNullOrEmpty(data)) return;

            try
            {
                QuestMachine.isLoadingGame = true;

                var saveData = SaveSystem.Deserialize<SaveData>(data);
                if (saveData == null) return;

                // Clear current quest list:
                DestroyQuestInstances(); // Adds them to deletedStaticQuests, but that's OK since we're going to restore deletedStaticQuests.

                // Restore dynamic quests:
                for (int i = 0; i < saveData.proceduralQuests.Count; i++)
                {
                    try
                    {
                        var questProxy = JsonUtility.FromJson<QuestProxy>(saveData.proceduralQuests[i]);
                        if (questProxy == null) continue;
                        var quest = ScriptableObject.CreateInstance<Quest>();
                        quest.name = questProxy.displayName;
                        questProxy.CopyTo(quest);
                        AddQuest(quest, true);
                    }
                    catch (Exception e)
                    {
                        if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: Unable to restore quest from serialized quest proxy. Message: " + e.Message + "\nData: " + saveData.proceduralQuests[i], this);
                    }
                }

                // Restore list of deleted static quests:
                deletedStaticQuests.Clear();
                deletedStaticQuests.AddRange(saveData.deletedStaticQuests);

                // Restore static quests from save data:
                // Must add all quests first in case one quest conditionally references another:
                int numStaticQuests = Mathf.Min(saveData.staticQuestIds.Count, saveData.staticQuestData.Count);
                for (int i = 0; i < numStaticQuests; i++)
                {
                    var questID = saveData.staticQuestIds[i];
                    var questData = saveData.staticQuestData[i];
                    if (string.IsNullOrEmpty(questID) || questData == null || questData.bytes == null) continue;
                    if (deletedStaticQuests.Contains(questID)) continue;
                    var quest = QuestMachine.GetQuestAsset(questID);
                    if (quest == null) quest = originalQuestList.Find(x => string.Equals(StringField.GetStringValue(x.id), questID));
                    if (quest == null && Debug.isDebugBuild) Debug.LogError("Quest Machine: " + name + " Can't find quest " + saveData.staticQuestIds[i] + ". Is it registered with Quest Machine?", this);
                    if (quest == null) continue;
                    quest = quest.Clone();
                    AddQuest(quest, true);
                }
                // Then deserialize their state info:
                for (int i = 0; i < numStaticQuests; i++)
                {
                    var questID = saveData.staticQuestIds[i];
                    var questData = saveData.staticQuestData[i];
                    if (string.IsNullOrEmpty(questID) || questData == null || questData.bytes == null) continue;
                    if (deletedStaticQuests.Contains(questID)) continue;
                    var quest = FindQuest(questID);
                    if (quest == null) quest = originalQuestList.Find(x => string.Equals(StringField.GetStringValue(x.id), questID));
                    if (quest == null) continue;
                    try
                    {
                        QuestStateSerializer.DeserializeInto(quest, questData.bytes, true);
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: Unable to restore quest: " + quest.name + ". Message: " + e.Message, this);
                            Destroy(quest);
                        }
                        catch (Exception) { }
                        quest = quest.Clone(); // Add fresh copy if failed to deserialize.
                        AddQuest(quest, true);
                    }                    
                }

                // Add any quests in original list that aren't in saved game:
                if (addNewQuestsSinceSavedGame)
                {
                    for (int i = 0; i < originalQuestList.Count; i++)
                    {
                        var originalQuest = originalQuestList[i];
                        if (originalQuest == null) continue;
                        var quest = FindQuest(originalQuest.id);
                        if (quest == null)
                        {
                            // Quest is not in restored list, so it may be new since the saved game. Add it:
                            quest = originalQuest.Clone();
                            AddQuest(quest, true);
                        }
                    }
                }

                QuestMachineMessages.RefreshIndicator(this, QuestMachineMessages.GetID(this));
            }
            finally
            {
                StartCoroutine(SetIsLoadingGameFalseAfter2Frames());
            }
        }

        protected IEnumerator SetIsLoadingGameFalseAfter2Frames()
        {
            yield return null; // Delayed startup waits one frame.
            yield return null; // Wait another frame to allow delayed startup to complete.
            QuestMachine.isLoadingGame = false;
        }

        #endregion

    }
}