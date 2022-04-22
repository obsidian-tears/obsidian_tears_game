// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;
using System;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// A GameObject that can offer quests. 
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class QuestGiver : IdentifiableQuestListContainer
    {

        #region Serialized Fields

        [Tooltip("Text table used to look up tags.")]
        [SerializeField]
        private TextTable m_textTable;

        [Tooltip("The UI content to show when the quest giver has no quests to offer.")]
        [SerializeField]
        private BasicUIContent m_noQuestsUIContents = new BasicUIContent();

        [Tooltip("The UI content to show when the quest giver has more than one quest to offer. Shown above the quest list.")]
        [SerializeField]
        private BasicUIContent m_offerableQuestsUIContents = new BasicUIContent();

        [Tooltip("The UI content to show when the quest giver has more than one active quest. Shown above the quest list.")]
        [SerializeField]
        private BasicUIContent m_activeQuestsUIContents = new BasicUIContent();

        [Tooltip("What to show in dialogue when quest giver only has completed quests.")]
        [SerializeField]
        private CompletedQuestDialogueMode m_completedQuestDialogueMode = CompletedQuestDialogueMode.SameAsGlobal;

        [Tooltip("The quest dialogue UI to use when conversing with the player. If unassigned, uses the default dialogue UI.")]
        [SerializeField]
        [IQuestDialogueUIInspectorField]
        private UnityEngine.Object m_questDialogueUI;

        [Tooltip("Frequency in seconds at which to check quest cooldowns in case a quest becomes offerable again and should update the indicator UI. Set to zero to bypass checking.")]
        [SerializeField]
        private float m_cooldownCheckFrequency = 0;

        #endregion

        #region Property Accessors to Serialized Fields

        /// <summary>
        /// The text table to use for tags.
        /// </summary>
        public TextTable textTable
        {
            get { return m_textTable; }
            set { m_textTable = value; }
        }

        /// <summary>
        /// The UI content to show when the quest giver has no quests to offer.
        /// </summary>
        public BasicUIContent noQuestsUIContents
        {
            get { return m_noQuestsUIContents; }
            set { m_noQuestsUIContents = value; }
        }

        /// <summary>
        /// The UI content to show when the quest giver has more than one quest to offer. Shown above the quest list.
        /// </summary>
        public BasicUIContent offerableQuestsUIContents
        {
            get { return m_offerableQuestsUIContents; }
            set { m_offerableQuestsUIContents = value; }
        }

        /// <summary>
        /// The UI content to show when the quest giver has more than one active quest. Shown above the quest list.
        /// </summary>
        public BasicUIContent activeQuestsUIContents
        {
            get { return m_activeQuestsUIContents; }
            set { m_activeQuestsUIContents = value; }
        }

        /// <summary>
        /// What to show in dialogue when quest givers only have completed quests.
        /// </summary>
        public CompletedQuestDialogueMode completedQuestDialogueMode
        {
            get
            {
                switch (m_completedQuestDialogueMode)
                {
                    case CompletedQuestDialogueMode.SameAsGlobal:
                        switch (QuestMachine.completedQuestDialogueMode)
                        {
                            case CompletedQuestGlobalDialogueMode.ShowCompletedQuest:
                                return CompletedQuestDialogueMode.ShowCompletedQuest;
                            default:
                                return CompletedQuestDialogueMode.ShowNoQuests;
                        }
                    default:
                        return m_completedQuestDialogueMode;
                }
            }
            set
            {
                m_completedQuestDialogueMode = value;
            }
        }

        /// <summary>
        /// The QuestDialogueUI to use when conversing with the player.
        /// </summary>
        public IQuestDialogueUI questDialogueUI
        {
            get { return ((m_questDialogueUI as IQuestDialogueUI) != null) ? m_questDialogueUI as IQuestDialogueUI : QuestMachine.defaultQuestDialogueUI; }
            set { m_questDialogueUI = value as UnityEngine.Object; }
        }

        /// <summary>
        /// Frequency in seconds at which to check quest cooldowns in case a quest becomes offerable again and should update the indicator UI.
        /// </summary>
        public float cooldownCheckFrequency
        {
            get { return m_cooldownCheckFrequency; }
            set { m_cooldownCheckFrequency = value; if (Application.isPlaying) RestartCooldownCheckInvokeRepeating(); }
        }

        public static string GetDisplayName(QuestGiver questGiver)
        {
            return (questGiver != null) ? StringField.GetStringValue(questGiver.displayName) : string.Empty;
        }

        #endregion

        #region Runtime Info

        // Runtime info:
        protected List<Quest> nonOfferableQuests { get; set; }
        protected List<Quest> offerableQuests { get; set; }
        protected List<Quest> activeQuests { get; set; }
        protected List<Quest> completedQuests { get; set; }
        protected GameObject player { get; set; }
        protected QuestParticipantTextInfo playerTextInfo { get; set; }
        protected bool allowBackButton { get; set; }

        private QuestListContainer m_playerQuestListContainer = null;
        protected QuestListContainer playerQuestListContainer 
        { 
            get 
            {
                if (m_playerQuestListContainer == null && player != null) m_playerQuestListContainer = player.GetComponent<QuestListContainer>();
                return m_playerQuestListContainer;
            }
            set 
            { 
                m_playerQuestListContainer = value; 
            }
        }

        private QuestParticipantTextInfo m_myQuestGiverTextinfo = null;
        protected QuestParticipantTextInfo myQuestGiverTextInfo
        {
            get
            {
                if (m_myQuestGiverTextinfo == null)  m_myQuestGiverTextinfo = new QuestParticipantTextInfo(id, displayName, image, textTable);
                return m_myQuestGiverTextinfo;
            }
        }

        /// <summary>
        /// When starting dialogue, show this quest instead of the most relevant quest.
        /// </summary>
        protected string overrideQuestIDToShowInDialogue = string.Empty;

        #endregion

        #region Initialization

        public override void Awake()
        {
            base.Awake();
            nonOfferableQuests = new List<Quest>();
            offerableQuests = new List<Quest>();
            activeQuests = new List<Quest>();
            completedQuests = new List<Quest>();
            allowBackButton = false;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            if (cooldownCheckFrequency > 0) InvokeRepeating("UpdateCooldowns", cooldownCheckFrequency, cooldownCheckFrequency);
        }

        public override void OnDisable()
        {
            CancelInvoke("UpdateCooldowns");
            base.OnDisable();
        }

        protected void RestartCooldownCheckInvokeRepeating()
        {
            CancelInvoke("UpdateCooldowns");
            if (cooldownCheckFrequency > 0) InvokeRepeating("UpdateCooldowns", cooldownCheckFrequency, cooldownCheckFrequency);
        }

        private void UpdateCooldowns()
        {
            for (int i = 0; i < questList.Count; i++)
            {
                var quest = questList[i];
                if (quest != null && quest.GetState() == QuestState.WaitingToStart) quest.UpdateCooldown();
            }
        }

        public override void Start()
        {
            base.Start();
            BackfillInfoFromEntityType();
            DeleteUnavailableQuests();
            AssignGiverIDToQuests();
            QuestMachineMessages.RefreshIndicators(this);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            DestroyBasicUIContent();
        }

        private void DestroyBasicUIContent()
        {
            if (noQuestsUIContents != null) noQuestsUIContents.DestroyContentList();
            if (offerableQuestsUIContents != null) offerableQuestsUIContents.DestroyContentList();
            if (activeQuestsUIContents != null) activeQuestsUIContents.DestroyContentList();
        }

        /// <summary>
        /// Resets quest giver's quest list to its original list.
        /// </summary>
        public override void ResetToOriginalState()
        {
            base.ResetToOriginalState();
            BackfillInfoFromEntityType();
            DeleteUnavailableQuests();
            AssignGiverIDToQuests();
        }

        /// <summary>
        /// If UI info is unassigned, get it from the quest giver's QuestEntity if present.
        /// </summary>
        protected void BackfillInfoFromEntityType()
        {
            if (questEntity == null) return;
            if (StringField.IsNullOrEmpty(id))
            {
                id = questEntity.id;
            }
            if (StringField.IsNullOrEmpty(displayName))
            {
                displayName = questEntity.displayName;
            }
            if (image == null)
            {
                image = questEntity.image;
            }
        }

        /// <summary>
        /// Deletes a quest from this quest giver's list.
        /// </summary>
        /// <param name="questID"></param>
        public override void DeleteQuest(StringField questID)
        {
            var quest = FindQuest(questID);
            quest.ClearQuestIndicatorStates();
            base.DeleteQuest(questID);
        }

        /// <summary>
        /// Deletes quests whose maxTimes have been reached.
        /// </summary>
        protected void DeleteUnavailableQuests()
        {
            if (questList == null || questList.Count == 0) return;
            for (int i = questList.Count - 1; i >= 0; i--)
            {
                var quest = questList[i];
                if (quest != null && quest.timesAccepted >= quest.maxTimes)
                {
                    DeleteQuest(quest);
                }
            }
        }

        protected void AssignGiverIDToQuests()
        {
            for (int i = 0; i < questList.Count; i++)
            {
                var quest = questList[i];
                if (quest == null) continue;
                quest.AssignQuestGiver(myQuestGiverTextInfo);
            }
        }

        /// <summary>
        /// Adds a quest to this quest giver's list.
        /// </summary>
        /// <param name="quest"></param>
        public override Quest AddQuest(Quest quest)
        {
            if (quest == null) return null;
            var instance = base.AddQuest(quest);
            instance.AssignQuestGiver(myQuestGiverTextInfo);
            QuestMachineMessages.RefreshUIs(this);
            return instance;
        }

        #endregion

        #region Record Quests By State

        public virtual bool HasOfferableOrActiveQuest()
        {
            return HasOfferableOrActiveQuest(null);
        }

        /// <summary>
        /// Returns the list of quests that this quest giver can currently offer.
        /// </summary>
        public virtual List<Quest> GetOfferableQuests()
        {
            return GetOfferableQuests(null);
        }

        /// <summary>
        /// Returns the list of quests that this quest giver has given player that 
        /// </summary>
        public virtual List<Quest> GetActiveQuests()
        {
            return GetActiveQuests(null);
        }

        public virtual bool HasOfferableOrActiveQuest(GameObject player)
        {
            RecordQuestsByState(player);
            return offerableQuests.Count > 0 || activeQuests.Count > 0;
        }

        /// <summary>
        /// Returns the list of quests that this quest giver can currently offer.
        /// </summary>
        public virtual List<Quest> GetOfferableQuests(GameObject player)
        {
            RecordQuestsByState(player);
            return offerableQuests;
        }

        /// <summary>
        /// Returns the list of quests that this quest giver has given player that 
        /// </summary>
        public virtual List<Quest> GetActiveQuests(GameObject player)
        {
            RecordQuestsByState(player);
            return activeQuests;
        }

        protected virtual void RecordQuestsByState(GameObject player)
        {
            if (player == null) player = FindPlayerGameObject();
            this.player = player;
            RecordQuestsByState();
        }

        /// <summary>
        /// Records the current offerable and player-assigned quests in the runtime lists.
        /// </summary>
        protected virtual void RecordQuestsByState()
        {
            RecordRelevantPlayerQuests();
            RecordOfferableQuests();
        }

        /// <summary>
        /// Records quests in the player's QuestList that were given by this quest giver
        /// or active quests for which this quest giver has dialogue content.
        /// </summary>
        protected virtual void RecordRelevantPlayerQuests()
        {
            activeQuests.Clear();
            completedQuests.Clear();
            if (playerQuestListContainer == null || playerQuestListContainer.questList == null) return;
            for (int i = 0; i < playerQuestListContainer.questList.Count; i++)
            {
                var quest = playerQuestListContainer.questList[i];
                if (quest == null) continue;
                var questState = quest.GetState();
                if (StringField.Equals(quest.questGiverID, id))
                {
                    switch (questState)
                    {
                        case QuestState.Active:
                            activeQuests.Add(quest);
                            break;
                        case QuestState.Successful:
                        case QuestState.Failed:
                            completedQuests.Add(quest);
                            break;
                    }
                }
                else if (questState == QuestState.Active && quest.speakers.Contains(StringField.GetStringValue(id)))
                {
                    activeQuests.Add(quest);
                }
            }
        }

        /// <summary>
        /// Removes completed quests that have no dialogue to offer.
        /// </summary>
        protected virtual void RemoveCompletedQuestsWithNoDialogue()
        {
            if (completedQuests == null || completedQuests.Count == 0) return;
            var info = new QuestParticipantTextInfo(id, displayName, image, textTable);
            for (int i = completedQuests.Count - 1; i >= 0; i--)
            {
                if (!completedQuests[i].HasContent(QuestContentCategory.Dialogue, info))
                {
                    completedQuests.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Records which quests are offerable or not in the runtime lists.
        /// </summary>
        protected virtual void RecordOfferableQuests()
        {
            nonOfferableQuests.Clear();
            offerableQuests.Clear();
            if (questList == null) return;
            if (playerQuestListContainer == null || playerQuestListContainer.questList == null) return;
            for (int i = 0; i < questList.Count; i++)
            {
                var quest = questList[i];
                if (quest == null || quest.GetState() != QuestState.WaitingToStart) continue;

                // Check if the player is already doing a copy of this quest:
                var playerCopy = playerQuestListContainer.FindQuest(quest.id);
                var isPlayerCopyActive = (playerCopy != null && playerCopy.GetState() == QuestState.Active);

                // And check if the player is already doing a similar procedurally-generated quest by this giver:
                if (IsDoingSimilarGeneratedQuest(quest, playerQuestListContainer)) isPlayerCopyActive = true;

                quest.UpdateCooldown();
                if (quest.canOffer && !isPlayerCopyActive && (playerCopy == null || playerCopy.timesAccepted < quest.maxTimes))
                {
                    // Add to offerable list unless player has already succeeded and quest is no repeat if successful:
                    if (!(quest.noRepeatIfSuccessful && PlayerHasSuccessfullyCompleted(quest.id)))
                    {
                        offerableQuests.Add(quest);
                    }
                }
                else if (playerCopy == null)
                {
                    nonOfferableQuests.Add(quest);
                }
            }
        }

        protected virtual bool PlayerHasSuccessfullyCompleted(StringField questID)
        {
            if (StringField.IsNullOrEmpty(questID) || playerQuestListContainer == null) return false;
            var quest = playerQuestListContainer.FindQuest(questID);
            return (quest == null) ? false : (quest.GetState() == QuestState.Successful);

        }

        protected virtual bool IsDoingSimilarGeneratedQuest(Quest quest, QuestListContainer playerQuestListContainer)
        {
            if (quest == null || playerQuestListContainer == null || !quest.isProcedurallyGenerated || activeQuests == null) return false;
            // Check active quests given by this giver:
            for (int i = 0; i < activeQuests.Count; i++)
            {
                var activeQuest = activeQuests[i];
                if (activeQuest == null) continue;
                if (activeQuest.tagDictionary.ContainsTag(QuestMachineTags.ACTION) &&
                    quest.tagDictionary.ContainsTag(QuestMachineTags.ACTION) &&
                    string.Equals(activeQuest.tagDictionary.dict[QuestMachineTags.ACTION], quest.tagDictionary.dict[QuestMachineTags.ACTION]) &&
                    activeQuest.tagDictionary.ContainsTag(QuestMachineTags.TARGET) &&
                    quest.tagDictionary.ContainsTag(QuestMachineTags.TARGET) &&
                    string.Equals(activeQuest.tagDictionary.dict[QuestMachineTags.TARGET], quest.tagDictionary.dict[QuestMachineTags.TARGET]))
                    return true;
            }
            return false;
        }

        #endregion

        #region Dialogue

        /// <summary>
        /// Looks for a GameObject tagged Player that has a QuestListContainer.
        /// Failing that, looks for a QuestJournal on any GameObject.
        /// </summary>
        public virtual GameObject FindPlayerJournalGameObject()
        {
            var players = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].GetComponent<QuestListContainer>() != null) return players[i];
            }
            var journal = FindObjectOfType<QuestJournal>(); // In case designer forgot to tag player as Player.
            return (journal != null) ? journal.gameObject : null;
        }

        /// <summary>
        /// Looks for a GameObject tagged Player that has a QuestListContainer.
        /// Failing that, looks for any GameObject tagged Player.
        /// </summary>
        public virtual GameObject FindPlayerGameObject()
        {
            var players = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].GetComponent<QuestListContainer>() != null) return players[i];
            }
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) return player;
            var journal = FindObjectOfType<QuestJournal>(); // In case designer forgot to tag player as Player, find a QuestJournal.
            return (journal != null) ? journal.gameObject : null;
        }

        /// <summary>
        /// Starts dialogue with the first GameObject in the scene that's tagged as "Player".
        /// </summary>
        public virtual void StartDialogueWithPlayer()
        {
            StartDialogue(null);
        }

        /// <summary>
        /// Starts dialogue with the player. The content of the dialogue will depend on the quest giver's
        /// offerable quests and the player's quests.
        /// </summary>
        /// <param name="player">Player conversing with this QuestGiver. If null, searches the scene for a GameObject tagged Player.</param>
        public virtual void StartDialogue(GameObject player)
        {
            if (questDialogueUI == null && Debug.isDebugBuild)
            {
                Debug.LogWarning("Quest Machine: Can't start dialogue with " + name + ". Quest Giver doesn't have access to a quest dialogue UI.", this);
                return;
            }

            // If player isn't specified, find it in the scene and find relevant quest journal:
            if (player == null) player = FindPlayerGameObject();
            if (player == null && Debug.isDebugBuild)
            {
                Debug.LogWarning("Quest Machine: Can't start dialogue with " + name + ". No quester specified.", this);
                return;
            }
            playerQuestListContainer = player.GetComponent<QuestListContainer>();
            if (playerQuestListContainer == null)
            {
                var playerID = player.GetComponent<QuestMachineID>();
                if (playerID != null && playerID.questListContainer != null)
                {
                    playerQuestListContainer = playerID.questListContainer;
                }
                else
                {
                    var playerJournalObject = FindPlayerJournalGameObject();
                    playerQuestListContainer = (playerJournalObject != null) ? playerJournalObject.GetComponent<QuestListContainer>() : null;
                }
            }
            if (playerQuestListContainer == null && Debug.isDebugBuild)
            {
                Debug.LogWarning("Quest Machine: Can't start dialogue with " + name + ". Quester " + player.name + " doesn't have a Quest Journal and can't find one in scene.", this);
                return;
            }

            QuestMachineTags.fallbackTextTable = textTable;

            // Setup player info:
            this.player = player;
            playerTextInfo = new QuestParticipantTextInfo(QuestMachineMessages.GetID(player), QuestMachineMessages.GetDisplayName(player), null, null);

            // Greet before recording quests in case Greet message changes a quest state:
            QuestMachineMessages.Greet(player, this, id);

            // Record quests related to this player and me:
            RecordQuestsByState();

            StartMostRelevantDialogue();

            // Note: Sends Greeted immediately after opening dialogue UI, not when closing it:
            QuestMachineMessages.Greeted(player, this, id);
        }

        public virtual void StartSpecifiedQuestDialogueWithPlayer(string questID)
        {
            overrideQuestIDToShowInDialogue = questID;
            StartDialogueWithPlayer();
        }

        public virtual void StartSpecifiedQuestDialogue(GameObject player, string questID)
        {
            overrideQuestIDToShowInDialogue = questID;
            StartDialogue(player);
        }

        protected virtual void StartMostRelevantDialogue()
        {
            // If specific quest is specified, start it:
            if (!string.IsNullOrEmpty(overrideQuestIDToShowInDialogue))
            {
                var quest = activeQuests.Find(x => string.Equals(StringField.GetStringValue(x.id), overrideQuestIDToShowInDialogue))
                ?? offerableQuests.Find(x => string.Equals(StringField.GetStringValue(x.id), overrideQuestIDToShowInDialogue))
                ?? nonOfferableQuests.Find(x => string.Equals(StringField.GetStringValue(x.id), overrideQuestIDToShowInDialogue))
                ?? completedQuests.Find(x => string.Equals(StringField.GetStringValue(x.id), overrideQuestIDToShowInDialogue));
                if (quest != null)
                {
                    if (activeQuests.Contains(quest))
                    {
                        ShowActiveQuest(quest);
                    }
                    else if (offerableQuests.Contains(quest))
                    {
                        ShowOfferQuest(quest);
                    }
                    else if (nonOfferableQuests.Contains(quest))
                    {
                        ShowOfferConditionsUnmet(quest);
                    }
                    else
                    {
                        ShowCompletedQuest(quest);
                    }
                    return;
                }
            }

            // Start the most appropriate dialogue based on the recorded quests:
            if (QuestMachine.debug) Debug.Log("Quest Machine: " + name + ".StartDialogue: #offerable=" + offerableQuests.Count + " #active=" + activeQuests.Count + " #completed=" + completedQuests.Count, this);
            if (activeQuests.Count + offerableQuests.Count >= 2)
            {
                ShowQuestList();                
            }
            else if (activeQuests.Count == 1)
            {
                ShowActiveQuest(activeQuests[0]);
            }
            else if (offerableQuests.Count == 1)
            {
                ShowOfferQuest(offerableQuests[0]);
            }
            else if (nonOfferableQuests.Count >= 1)
            {
                ShowOfferConditionsUnmet();                
            }
            else
            {
                RemoveCompletedQuestsWithNoDialogue();
                if (completedQuests.Count > 0 && completedQuestDialogueMode == CompletedQuestDialogueMode.ShowCompletedQuest)
                {
                    ShowCompletedQuest();
                }
                else
                {
                    ShowNoQuestsToDiscuss();
                }
            }
        }

        /// <summary>
        /// Stops dialogue with the player.
        /// </summary>
        public virtual void StopDialogue()
        {
            if (questDialogueUI == null) return;
            questDialogueUI.Hide();
        }

        protected virtual void ShowNoQuestsToDiscuss()
        {
            questDialogueUI.ShowContents(myQuestGiverTextInfo, noQuestsUIContents.contentList); 
        }

        protected virtual void ShowQuestList()
        {
            questDialogueUI.ShowQuestList(myQuestGiverTextInfo, activeQuestsUIContents.contentList, activeQuests, offerableQuestsUIContents.contentList, offerableQuests, OnSelectQuest);
        }

        protected virtual void ShowOfferConditionsUnmet()
        {
            questDialogueUI.ShowOfferConditionsUnmet(myQuestGiverTextInfo, noQuestsUIContents.contentList, nonOfferableQuests); 
        }

        protected virtual void ShowOfferConditionsUnmet(Quest quest)
        {
            questDialogueUI.ShowOfferConditionsUnmet(myQuestGiverTextInfo, noQuestsUIContents.contentList, new List<Quest>() { quest });
        }

        protected virtual void ShowOfferQuest(Quest quest)
        {
            if (questDialogueUI == null)
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: There is no Quest Dialogue UI.", this);
            }
            else if (quest == null)
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: The quest passed to ShowOfferQuest() is null.", this);
            }
            else if (playerQuestListContainer == null)
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: There is no Player Quest List Container. Can't offer quest '" + quest.title + "'.", this);
            }
            else
            {
                quest.greeterID = StringField.GetStringValue(playerTextInfo.id);
                quest.greeter = StringField.GetStringValue(playerTextInfo.displayName);
                QuestMachineMessages.DiscussQuest(player, this, id, quest.id);
                playerQuestListContainer.DeleteQuest(quest.id); // Clear any old instance of repeatable quests first.
                questDialogueUI.ShowOfferQuest(myQuestGiverTextInfo, quest, OnAcceptQuest, OnQuestBackButton);
                QuestMachineMessages.DiscussedQuest(player, this, id, quest.id);
            }
        }

        protected virtual void ShowActiveQuest(Quest quest)
        {
            QuestParameterDelegate backButtonDelegate = null;
            if (activeQuests.Count + offerableQuests.Count >= 2)
            {
                backButtonDelegate = OnQuestBackButton;
                allowBackButton = true; // May turn in quest and be left with only 1. In this case, still allow back button.
            }
            quest.greeterID = StringField.GetStringValue(playerTextInfo.id);
            quest.greeter = StringField.GetStringValue(playerTextInfo.displayName);
            QuestMachineMessages.DiscussQuest(player, this, id, quest.id);
            questDialogueUI.ShowActiveQuest(myQuestGiverTextInfo, quest, OnContinueActiveQuest, backButtonDelegate);
            QuestMachineMessages.DiscussedQuest(player, this, id, quest.id);

            // If this was the only quest, check if we have a new quest after discussing this one. 
            // If so, enable back button:
            if (backButtonDelegate == null)
            {
                RecordQuestsByState();
                var numOtherActive = activeQuests.Count;
                if (quest != null && quest.GetState() == QuestState.Active) numOtherActive--;
                var hasNewQuest = (numOtherActive + offerableQuests.Count) > 0;
                if (hasNewQuest && (questDialogueUI is UnityUIQuestDialogueUI))
                {
                    allowBackButton = true;
                    (questDialogueUI as UnityUIQuestDialogueUI).SetBackHandler(OnQuestBackButton);
                }
            }
        }

        protected virtual void ShowCompletedQuest()
        {
            questDialogueUI.ShowCompletedQuest(myQuestGiverTextInfo, completedQuests);
        }

        protected virtual void ShowCompletedQuest(Quest quest)
        {
            questDialogueUI.ShowCompletedQuest(myQuestGiverTextInfo, new List<Quest>() { quest }); 
        }

        protected virtual void OnSelectQuest(Quest quest)
        {
            switch (quest.GetState())
            {
                case QuestState.WaitingToStart:
                    ShowOfferQuest(quest);
                    break;
                case QuestState.Active:
                    ShowActiveQuest(quest);
                    break;
            }
        }

        protected virtual void OnAcceptQuest(Quest quest)
        {
            GiveQuestToQuester(quest, playerTextInfo, playerQuestListContainer);
            RecordQuestsByState();
            if (offerableQuests.Count >= 1 && (activeQuests.Count + offerableQuests.Count) >= 2)
            {
                questDialogueUI.ShowQuestList(myQuestGiverTextInfo, activeQuestsUIContents.contentList, activeQuests, offerableQuestsUIContents.contentList, offerableQuests, OnSelectQuest);
            }
            else
            {
                questDialogueUI.Hide();
            }
        }

        protected virtual void OnQuestBackButton(Quest quest)
        {
            if (activeQuests.Contains(quest) && quest.GetState() != QuestState.Active)
            {
                activeQuests.Remove(quest);
            }
            if (activeQuests.Count + offerableQuests.Count >= 2 || allowBackButton)
            {
                questDialogueUI.ShowQuestList(myQuestGiverTextInfo, activeQuestsUIContents.contentList, activeQuests, offerableQuestsUIContents.contentList, offerableQuests, OnSelectQuest);
                allowBackButton = false;
            }
            else
            {
                questDialogueUI.Hide();
            }
        }

        protected virtual void OnContinueActiveQuest(Quest quest)
        {
            questDialogueUI.Hide();
        }

        #endregion

        #region Give Quest

        /// <summary>
        /// Adds an instance of a quest to a quester's list. If the quest's maxTimes are reached,
        /// deletes the quest from the giver. Otherwise starts cooldown timer until it can be
        /// given again.
        /// </summary>
        /// <param name="quest">Quest to give to quester.</param>
        /// <param name="questerTextInfo">Quester's text info.</param>
        /// <param name="questerQuestListContainer">Quester's quest list container.</param>
        public virtual void GiveQuestToQuester(Quest quest, QuestParticipantTextInfo questerTextInfo, QuestListContainer questerQuestListContainer)
        {
            if (quest == null)
            {
                Debug.LogWarning("Quest Machine: " + name + ".GiveQuestToQuester - quest is null.", this);
                return;
            }
            if (questerTextInfo == null)
            {
                Debug.LogWarning("Quest Machine: " + name + ".GiveQuestToQuester - questerTextInfo is null.", this);
                return;
            }
            if (questerQuestListContainer == null)
            {
                Debug.LogWarning("Quest Machine: " + name + ".GiveQuestToQuester - questerQuestListContainer is null.", this);
                return;
            }

            // Make a copy of the quest for the quester:
            var questInstance = quest.Clone();

            // Update the version on this QuestGiver:
            quest.timesAccepted++;
            if (quest.timesAccepted >= quest.maxTimes)
            {
                DeleteQuest(quest.id);
            }
            else
            {
                quest.StartCooldown();
            }

            // Add the copy to the quester and activate it:
            questInstance.AssignQuestGiver(myQuestGiverTextInfo);
            questInstance.AssignQuester(questerTextInfo);
            questInstance.timesAccepted = 1;
            if (questerQuestListContainer.questList.Count > 0)
            {
                for (int i = questerQuestListContainer.questList.Count - 1; i >= 0; i--)
                {
                    var inJournal = questerQuestListContainer.questList[i];
                    if (inJournal == null) continue;
                    if (StringField.Equals(inJournal.id, quest.id) && inJournal.GetState() != QuestState.Active)
                    {
                        questInstance.timesAccepted++;
                        questerQuestListContainer.DeleteQuest(inJournal);
                    }
                }
            }
            questerQuestListContainer.deletedStaticQuests.Remove(StringField.GetStringValue(questInstance.id));
            questerQuestListContainer.AddQuest(questInstance);
            questInstance.SetState(QuestState.Active);
            QuestMachineMessages.RefreshIndicators(questInstance);

        }

        /// <summary>
        /// Adds an instance of a quest to a quester's list. If the quest's maxTimes are reached,
        /// deletes the quest from the giver. Otherwise starts cooldown timer until it can be
        /// given again.
        /// </summary>
        /// <param name="quest">Quest to give to quester.</param>
        /// <param name="questerQuestListContainer">Quester's quest list container.</param>
        public virtual void GiveQuestToQuester(Quest quest, QuestListContainer questerQuestListContainer)
        {
            if (quest == null) return;
            if (questerQuestListContainer == null)
            {
                Debug.LogWarning("Quest Machine: " + name + ".GiveQuestToQuester - quester (QuestListContainer) is null.", this);
                return;
            }
            var questerTextInfo = new QuestParticipantTextInfo(QuestMachineMessages.GetID(questerQuestListContainer.gameObject), QuestMachineMessages.GetDisplayName(questerQuestListContainer.gameObject), null, null);
            GiveQuestToQuester(quest, questerTextInfo, questerQuestListContainer);
        }

        /// <summary>
        /// Adds an instance of a quest to a quester's list. If the quest's maxTimes are reached,
        /// deletes the quest from the giver. Otherwise starts cooldown timer until it can be
        /// given again.
        /// </summary>
        /// <param name="quest">Quest to give to quester.</param>
        /// <param name="questerID">Quester's ID.</param>
        public virtual void GiveQuestToQuester(Quest quest, string questerID)
        {
            GiveQuestToQuester(quest, QuestMachine.GetQuestListContainer(questerID));
        }

        /// <summary>
        /// Adds an instance of a quest to a quester's list. If the quest's maxTimes are reached,
        /// deletes the quest from the giver. Otherwise starts cooldown timer until it can be
        /// given again.
        /// </summary>
        /// <param name="quest">Quest to give to quester.</param>
        /// <param name="questerID">Quester's ID.</param>
        public virtual void GiveQuestToQuester(Quest quest, StringField questerID)
        {
            GiveQuestToQuester(quest, StringField.GetStringValue(questerID));
        }

        /// <summary>
        /// Gives all quests to a quester.
        /// </summary>
        /// <param name="questerQuestListContainer">Quester's QuestListContainer.</param>
        public virtual void GiveAllQuestsToQuester(QuestListContainer questerQuestListContainer)
        {
            if (questerQuestListContainer == null || questList.Count == 0) return;
            for (int i = questList.Count - 1; i >= 0; i--)
            {
                var quest = questList[i];
                if (quest != null && !questerQuestListContainer.ContainsQuest(quest.id))
                {
                    GiveQuestToQuester(questList[i], questerQuestListContainer);
                }
            }
        }

        /// <summary>
        /// Gives all quests to a quester.
        /// </summary>
        /// <param name="questerID">ID of quester.</param>
        public virtual void GiveAllQuestsToQuester(string questerID)
        {
            GiveAllQuestsToQuester(QuestMachine.GetQuestListContainer(questerID));
        }

        /// <summary>
        /// Gives all quests to a quester.
        /// </summary>
        /// <param name="questerID">ID of quester.</param>
        public virtual void GiveAllQuestsToQuester(StringField questerID)
        {
            GiveAllQuestsToQuester(StringField.GetStringValue(questerID));
        }

        /// <summary>
        /// Gives all quests to a quester.
        /// </summary>
        /// <param name="quester">Quester.</param>
        public virtual void GiveAllQuestsToQuester(GameObject quester)
        {
            if (quester == null) return;
            GiveAllQuestsToQuester(quester.GetComponent<QuestListContainer>());
        }

        #endregion

    }

}
