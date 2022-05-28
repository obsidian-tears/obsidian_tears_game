// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Unity UI implementation of Quest Journal UI.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class UnityUIQuestJournalUI : UnityUIBaseUI, IQuestJournalUI
    {

        #region Serialized Fields

        [Header("Selection Panel")]

        [SerializeField]
        private RectTransform m_questSelectionContentContainer;
        [SerializeField]
        private UnityUIFoldoutTemplate m_questGroupTemplate;
        [SerializeField]
        private UnityUIQuestNameButtonTemplate m_activeQuestNameTemplate;
        [SerializeField]
        private UnityUIQuestNameButtonTemplate m_completedQuestNameTemplate;
        [Tooltip("Show all groups expanded.")]
        [SerializeField]
        private bool m_alwaysExpandAllGroups = false;
        [Tooltip("Show details when pointer hovers quest name.")]
        [SerializeField]
        private bool m_showDetailsOnFocus = false;
        [Tooltip("Include completed quests in selection panel.")]
        [SerializeField]
        private bool m_showCompletedQuests = true;

        [Header("Details Panel")]

        [SerializeField]
        private RectTransform m_questDetailsContentContainer;
        [SerializeField]
        private UnityUITextTemplate m_questHeadingTextTemplate;
        [SerializeField]
        private UnityUITextTemplate[] m_subheadingTemplates;
        [SerializeField]
        private UnityUITextTemplate m_questBodyTextTemplate;
        [SerializeField]
        private UnityUIIconListTemplate m_iconListTemplate;
        [SerializeField]
        private UnityUIButtonListTemplate m_buttonListTemplate;
        [SerializeField]
        private RectTransform m_questDetailsButtonContainer;
        [SerializeField]
        private UnityUIButtonTemplate m_trackButtonTemplate;
        [SerializeField]
        private UnityUIButtonTemplate m_abandonButtonTemplate;

        [Header("Abandon Quest Panel")]

        [SerializeField]
        private UIPanel m_abandonQuestPanel;
        [SerializeField]
        private UITextField m_abandonQuestNameText;

        [Header("Misc")]

        [SerializeField]
        private bool m_sortAlphabetically = true;
        [SerializeField]
        private bool m_showDisplayNameInHeading = false;
        [SerializeField]
        private bool m_showDialogueContentIfNoJournalContent = false;
        [SerializeField]
        private bool m_showQuestsThatHaveNoContent = true;
        [SerializeField]
        private bool m_showFirstQuestDetailsOnOpen = false;
        [Tooltip("Show Track toggle button in quest details panel.")]
        [SerializeField]
        private bool m_showTrackButtonInDetails = false;

        public enum SendMessageOnOpen { Never, Always, NotWhenUsingMouse }
        [SerializeField]
        private SendMessageOnOpen m_sendMessageOnOpen = SendMessageOnOpen.NotWhenUsingMouse;
        [SerializeField]
        private string m_openMessage = "Pause Player";
        [SerializeField]
        private string m_closeMessage = "Unpause Player";

        #endregion

        #region Accessor Properties for Serialized Fields

        public RectTransform questSelectionContentContainer
        {
            get { return m_questSelectionContentContainer; }
            set { m_questSelectionContentContainer = value; }
        }
        public UnityUIFoldoutTemplate questGroupTemplate
        {
            get { return m_questGroupTemplate; }
            set { m_questGroupTemplate = value; }
        }
        public UnityUIQuestNameButtonTemplate activeQuestNameTemplate
        {
            get { return m_activeQuestNameTemplate; }
            set { m_activeQuestNameTemplate = value; }
        }
        public UnityUIQuestNameButtonTemplate completedQuestNameTemplate
        {
            get { return m_completedQuestNameTemplate; }
            set { m_completedQuestNameTemplate = value; }
        }
        public bool alwaysExpandAllGroups
        {
            get { return m_alwaysExpandAllGroups; }
            set { m_alwaysExpandAllGroups = value; }
        }
        public bool showDetailsOnFocus
        {
            get { return m_showDetailsOnFocus; }
            set { m_showDetailsOnFocus = value; }
        }
        public bool showTrackButtonInDetails
        {
            get { return m_showTrackButtonInDetails; }
            set { m_showTrackButtonInDetails = value; }
        }
        public bool showCompletedQuests
        {
            get { return m_showCompletedQuests; }
            set { m_showCompletedQuests = value; }
        }
        public RectTransform questDetailsContentContainer
        {
            get { return m_questDetailsContentContainer; }
            set { m_questDetailsContentContainer = value; }
        }
        public UnityUITextTemplate questHeadingTextTemplate
        {
            get { return m_questHeadingTextTemplate; }
            set { m_questHeadingTextTemplate = value; }
        }
        public UnityUITextTemplate[] subheadingTemplates
        {
            get { return m_subheadingTemplates; }
            set { m_subheadingTemplates = value; }
        }
        public UnityUITextTemplate questBodyTextTemplate
        {
            get { return m_questBodyTextTemplate; }
            set { m_questBodyTextTemplate = value; }
        }
        public UnityUIIconListTemplate iconListTemplate
        {
            get { return m_iconListTemplate; }
            set { m_iconListTemplate = value; }
        }
        public UnityUIButtonListTemplate buttonListTemplate
        {
            get { return m_buttonListTemplate; }
            set { m_buttonListTemplate = value; }
        }
        public UnityUIButtonTemplate trackButtonTemplate
        {
            get { return m_trackButtonTemplate; }
            set { m_trackButtonTemplate = value; }
        }
        public UnityUIButtonTemplate abandonButtonTemplate
        {
            get { return m_abandonButtonTemplate; }
            set { m_abandonButtonTemplate = value; }
        }
        public UIPanel abandonQuestPanel
        {
            get { return m_abandonQuestPanel; }
            set { m_abandonQuestPanel = value; }
        }
        public UITextField abandonQuestNameText
        {
            get { return m_abandonQuestNameText; }
            set { m_abandonQuestNameText = value; }
        }
        public bool showDisplayNameInHeading
        {
            get { return m_showDisplayNameInHeading; }
            set { m_showDisplayNameInHeading = value; }
        }
        public bool showDialogueContentIfNoJournalContent
        {
            get { return m_showDialogueContentIfNoJournalContent; }
            set { m_showDialogueContentIfNoJournalContent = value; }
        }
        public bool showQuestsThatHaveNoContent
        {
            get { return m_showQuestsThatHaveNoContent; }
            set { m_showQuestsThatHaveNoContent = value; }
        }
        public bool showFirstQuestDetailsOnOpen
        {
            get { return m_showFirstQuestDetailsOnOpen; }
            set { m_showFirstQuestDetailsOnOpen = value; }
        }

        #endregion

        #region Runtime Properties

        protected UnityUIInstancedContentManager selectionPanelContentManager { get; set; }
        protected UnityUIInstancedContentManager detailsPanelContentManager { get; set; }
        protected bool isDrawingSelectionPanel { get; set; }

        protected override RectTransform currentContentContainer { get { return isDrawingSelectionPanel ? questSelectionContentContainer : questDetailsContentContainer; } }
        protected override UnityUIInstancedContentManager currentContentManager { get { return isDrawingSelectionPanel ? selectionPanelContentManager : detailsPanelContentManager; } }
        protected override UnityUITextTemplate currentHeadingTemplate { get { return isDrawingSelectionPanel ? null : questHeadingTextTemplate; } }
        protected override UnityUITextTemplate[] currentSubheadingTemplates { get { return subheadingTemplates; } }
        protected override UnityUITextTemplate currentBodyTemplate { get { return isDrawingSelectionPanel ? null : questBodyTextTemplate; } }
        protected override UnityUIIconListTemplate currentIconListTemplate { get { return isDrawingSelectionPanel ? null : iconListTemplate; } }
        protected override UnityUIButtonListTemplate currentButtonListTemplate { get { return isDrawingSelectionPanel ? null : buttonListTemplate; } }

        protected List<string> expandedGroupNames = new List<string>();
        protected Quest selectedQuest { get; set; }
        protected QuestJournal questJournal { get; set; }

        private Coroutine m_refreshCoroutine = null;

        private bool m_mustSendCloseMessage = false;
        private bool m_hasInitialized = false;

        #endregion

        protected override void Awake()
        {
            base.Awake();
            selectionPanelContentManager = new UnityUIInstancedContentManager();
            detailsPanelContentManager = new UnityUIInstancedContentManager();
            m_hasInitialized = true;
        }

        protected virtual void OnDisable()
        {
            m_refreshCoroutine = null;
        }

        protected override void InitializeTemplates()
        {
            if (Debug.isDebugBuild)
            {
                if (mainPanel == null) Debug.LogError("Quest Machine: Main Panel is unassigned.", this);
                if (questSelectionContentContainer == null) Debug.LogError("Quest Machine: Quest Selection Content Container is unassigned.", this);
                if (questGroupTemplate == null) Debug.LogError("Quest Machine: Quest Group Template is unassigned.", this);
                if (activeQuestNameTemplate == null) Debug.LogError("Quest Machine: Active Quest Name Template is unassigned.", this);
                if (completedQuestNameTemplate == null && showCompletedQuests) Debug.LogError("Quest Machine: Completed Quest Name Template is unassigned.", this);
                if (questDetailsContentContainer == null) Debug.LogError("Quest Machine: Quest Details Content Container is unassigned.", this);
                if (questHeadingTextTemplate == null) Debug.LogError("Quest Machine: Quest Heading Text Template is unassigned.", this);
                if (questBodyTextTemplate == null) Debug.LogError("Quest Machine: Quest Body Text Template is unassigned.", this);
                if (iconListTemplate == null) Debug.LogError("Quest Machine: Icon List Template is unassigned.", this);
                if (iconListTemplate != null && iconListTemplate.iconTemplate == null) Debug.LogError("Quest Machine: Icon List Template's Icon Template is unassigned.", this);
                if (buttonListTemplate == null) Debug.LogError("Quest Machine: Button List Template is unassigned.", this);
                if (buttonListTemplate != null && buttonListTemplate.buttonTemplate == null) Debug.LogError("Quest Machine: Button List Template's Button Template is unassigned.", this);
                if (abandonQuestPanel == null) Debug.LogError("Quest Machine: Abandon Quest Panel is unassigned.", this);
                if (abandonQuestNameText == null) Debug.LogError("Quest Machine: Abandon Quest Name Text is unassigned.", this);
            }
            if (questGroupTemplate != null) questGroupTemplate.gameObject.SetActive(false);
            if (activeQuestNameTemplate != null) activeQuestNameTemplate.gameObject.SetActive(false);
            if (completedQuestNameTemplate!= null) completedQuestNameTemplate.gameObject.SetActive(false);
            if (questHeadingTextTemplate != null) questHeadingTextTemplate.gameObject.SetActive(false);
            if (questBodyTextTemplate != null) questBodyTextTemplate.gameObject.SetActive(false);
            if (iconListTemplate != null) iconListTemplate.gameObject.SetActive(false);
            if (buttonListTemplate != null) buttonListTemplate.gameObject.SetActive(false);
            if (abandonButtonTemplate != null) abandonButtonTemplate.gameObject.SetActive(false);
            if (trackButtonTemplate != null) trackButtonTemplate.gameObject.SetActive(false);
        }

        public virtual void Toggle(QuestJournal questJournal)
        {
            if (isVisible)
            {
                Hide();
            }
            else
            {
                Show(questJournal);
            }
        }

        /// <summary>
        /// True if the group is expanded in the UI.
        /// </summary>
        public virtual bool IsGroupExpanded(string groupName)
        {
            return alwaysExpandAllGroups || expandedGroupNames.Contains(groupName);
        }

        /// <summary>
        /// Toggles whether a group is expanded or not.
        /// </summary>
        /// <param name="groupName">Group to toggle.</param>
        public virtual void ToggleGroup(string groupName)
        {
            if (IsGroupExpanded(groupName))
            {
                expandedGroupNames.Remove(groupName);
            }
            else
            {
                expandedGroupNames.Add(groupName);
            }
        }

        /// <summary>
        /// Opens the quest journal UI.
        /// </summary>
        public virtual void Show(QuestJournal questJournal)
        {
            this.questJournal = questJournal;
            Show();
            Repaint();
            m_mustSendCloseMessage = ShouldSendOpenCloseMessage();
            if (ShouldSendOpenCloseMessage()) MessageSystem.SendMessage(this, m_openMessage, string.Empty);
            if (showFirstQuestDetailsOnOpen) selectedQuest = GetFirstQuest();
        }

        /// <summary>
        /// Opens the quest journal UI, showing a specified quest's details.
        /// </summary>
        public virtual void Show(QuestJournal questJournal, Quest quest)
        {
            Show(questJournal);
            SelectQuest(quest);
        }

        /// <summary>
        /// Opens the quest journal UI, showing a specified quest's details.
        /// </summary>
        public virtual void Show(QuestJournal questJournal, StringField questID)
        {
            Show(questJournal);
            if (questJournal != null) SelectQuest(questJournal.FindQuest(questID));
        }

        /// <summary>
        /// Opens the quest journal UI, showing a specified quest's details.
        /// </summary>
        public virtual void Show(QuestJournal questJournal, string questID)
        {
            Show(questJournal);
            if (questJournal != null) SelectQuest(questJournal.FindQuest(questID));
        }

        public override void Hide()
        {
            base.Hide();
            if (m_mustSendCloseMessage) MessageSystem.SendMessage(this, m_closeMessage, string.Empty);
            m_mustSendCloseMessage = false;
        }

        private bool ShouldSendOpenCloseMessage()
        {
            switch (m_sendMessageOnOpen)
            {
                case SendMessageOnOpen.Always:
                    return true;
                case SendMessageOnOpen.Never:
                    return false;
                case SendMessageOnOpen.NotWhenUsingMouse:
                    return InputDeviceManager.currentInputDevice != InputDevice.Mouse;
                default:
                    return false;
            }
        }

        protected virtual Quest GetFirstQuest()
        {
            if (!showCompletedQuests && questJournal.questList != null)
            {
                // If we're omitting completed quests, return the first non-completed quest:
                for (int i = 0; i < questJournal.questList.Count; i++)
                {
                    var quest = questJournal.questList[i];
                    if (quest != null && !IsCompletedQuestState(quest.GetState())) return quest;
                }
                return null;
            }
            else
            {
                // Otherwise we can just return the first quest in the list:
                var hasQuest = (questJournal != null && questJournal.questList != null && questJournal.questList.Count > 0);
                return hasQuest ? questJournal.questList[0] : null;
            }
        }

        public virtual void Repaint(QuestJournal questJournal)
        {
            this.questJournal = questJournal;
            Repaint();
        }

        public virtual void Repaint()
        {
            if (!(isVisible && enabled && gameObject.activeInHierarchy)) return;
            if (m_refreshCoroutine == null) m_refreshCoroutine = StartCoroutine(RefreshAtEndOfFrame());
        }

        private IEnumerator RefreshAtEndOfFrame()
        {
            // Wait until end of frame so we only refresh once in case we receive multiple
            // requests to refresh during the same frame.
            yield return new WaitForEndOfFrame();
            m_refreshCoroutine = null;
            RefreshNow();
        }

        protected virtual void RefreshNow() //[TODO] Consider pooling.
        {
            isDrawingSelectionPanel = true;
            selectionPanelContentManager.Clear();

            RefreshHeading();

            List<string> groupNames;
            int numGroupless;
            GetGroupNames(out groupNames, out numGroupless);

            AddQuestsToUI(groupNames, numGroupless);

            RepaintSelectedQuest();

            RefreshNavigableSelectables();
        }

        protected virtual void RefreshHeading()
        {
            if (entityImage != null) entityImage.sprite = questJournal.image;
            if (showDisplayNameInHeading && entityName != null && !StringField.IsNullOrEmpty(questJournal.displayName)) entityName.text = questJournal.displayName.value;
        }

        protected virtual void GetGroupNames(out List<string> groupNames, out int numGroupless)
        {
            groupNames = new List<string>();
            numGroupless = 0;
            foreach (var quest in questJournal.questList)
            {
                if (quest == null) continue;
                var questState = quest.GetState();
                if (questState == QuestState.WaitingToStart) continue;
                if (!showCompletedQuests && IsCompletedQuestState(questState)) continue;
                var groupName = StringField.GetStringValue(quest.group);
                if (string.IsNullOrEmpty(groupName)) numGroupless++;
                if (string.IsNullOrEmpty(groupName) || groupNames.Contains(groupName)) continue;
                groupNames.Add(groupName);
            }
            SortGroupNames(groupNames);
        }

        /// <summary>
        /// You can override this method to sort differently.
        /// </summary>
        protected virtual void SortGroupNames(List<string> groupNames)
        {
            if (m_sortAlphabetically)
            {
                groupNames.Sort();
            }
        }

        /// <summary>
        /// You can override this method to sort differently.
        /// </summary>
        protected virtual void SortQuests(List<Quest> quests)
        {
            if (m_sortAlphabetically)
            {
                quests.Sort((x, y) => string.Compare(StringField.GetStringValue(x.title), StringField.GetStringValue(y.title)));
            }
        }

        protected virtual bool IsCompletedQuestState(QuestState questState)
        {
            return questState == QuestState.Successful || questState == QuestState.Failed || questState == QuestState.Abandoned;
        }

        protected virtual void AddQuestsToUI(List<string> groupNames, int numGroupless)
        {
            var quests = new List<Quest>(questJournal.questList);
            SortQuests(quests);

            AddGroupedQuestsToUI(quests, groupNames);
            if (numGroupless > 0)
            {
                AddQuestsToUI(quests, string.Empty, questSelectionContentContainer);
            }
        }

        protected virtual void AddGroupedQuestsToUI(List<Quest> quests, List<string> groupNames)
        {
            foreach (var groupName in groupNames)
            {
                AddQuestGroupToUI(quests, groupName);
            }
        }

        protected void AddQuestGroupToUI(List<Quest> quests, string groupName)
        {
            // Add foldout for group:
            var groupFoldout = Instantiate<UnityUIFoldoutTemplate>(questGroupTemplate);
            selectionPanelContentManager.Add(groupFoldout, questSelectionContentContainer);
            groupFoldout.Assign(groupName, IsGroupExpanded(groupName));
            if (alwaysExpandAllGroups)
            {
                groupFoldout.foldoutButton.interactable = false;
            }
            else
            {
                groupFoldout.foldoutButton.interactable = true;
                groupFoldout.foldoutButton.onClick.AddListener(() => { OnClickGroup(groupName, groupFoldout); });
            }

            // Add group's quests:
            AddQuestsToUI(quests, groupName, groupFoldout.interiorPanel);
        }

        protected virtual void AddGrouplessQuestsToUI()
        {
            foreach (var quest in questJournal.questList)
            {
                if (quest.GetState() == QuestState.WaitingToStart) continue;
                var groupName = StringField.GetStringValue(quest.group);
                if (!string.IsNullOrEmpty(groupName)) continue;
                var questName = Instantiate<UnityUIQuestNameButtonTemplate>(GetQuestNameTemplateForState(quest.GetState()));
                questName.Assign(quest, OnToggleTracking);
                selectionPanelContentManager.Add(questName, questSelectionContentContainer);
                var target = quest;
                SetupQuestNameUIEvents(questName.buttonTemplate.button, target);
            }
        }

        protected virtual void AddQuestsToUI(List<Quest> quests, string requiredGroupName, RectTransform container)
        {
            AddQuestsToUI(quests, requiredGroupName, container, true);
            AddQuestsToUI(quests, requiredGroupName, container, false);
        }

        protected virtual void AddQuestsToUI(List<Quest> quests, string requiredGroupName, RectTransform container, bool onlyAddActive)
        { 
            foreach (var quest in quests)
            {
                if (quest == null) continue;
                var questState = quest.GetState();
                if (questState == QuestState.WaitingToStart) continue;
                if (!showCompletedQuests && IsCompletedQuestState(questState)) continue;
                if (onlyAddActive && questState != QuestState.Active) continue;
                if (!onlyAddActive && questState == QuestState.Active) continue;
                var groupName = StringField.GetStringValue(quest.group);
                if (string.Equals(groupName, requiredGroupName))
                {
                    AddQuestToUI(quest, container);
                }
            }
        }

        protected virtual void AddQuestToUI(Quest quest, RectTransform container)
        {
            if (quest == null || container == null) return;
            if (!showQuestsThatHaveNoContent)
            {
                var contents = GetQuestContents(quest);
                if (contents == null || contents.Count <= 1) return;
            }
            var questName = Instantiate<UnityUIQuestNameButtonTemplate>(GetQuestNameTemplateForState(quest.GetState()));
            questName.Assign(quest, OnToggleTracking);
            selectionPanelContentManager.Add(questName, container);
            var target = quest;
            SetupQuestNameUIEvents(questName.buttonTemplate.button, target);
        }

        private void SetupQuestNameUIEvents(UnityEngine.UI.Button button, Quest target)
        {
            if (button == null || target == null) return;
            button.onClick.AddListener(() => { OnClickQuest(target); });
            if (showDetailsOnFocus)
            {
                var trigger = button.GetComponent<UnityEngine.EventSystems.EventTrigger>();
                if (trigger == null) trigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
                // PointerEnter:
                var entry = new UnityEngine.EventSystems.EventTrigger.Entry();
                entry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
                entry.callback.AddListener((eventData) => { OnClickQuest(target); });
                trigger.triggers.Add(entry);
                // Select:
                entry = new UnityEngine.EventSystems.EventTrigger.Entry();
                entry.eventID = UnityEngine.EventSystems.EventTriggerType.Select;
                entry.callback.AddListener((eventData) => { OnClickQuest(target); });
                trigger.triggers.Add(entry);
            }
        }

        private UnityUIQuestNameButtonTemplate GetQuestNameTemplateForState(QuestState state)
        {
            return (state == QuestState.Active) ? activeQuestNameTemplate : completedQuestNameTemplate;
        }

        protected virtual void OnClickGroup(string groupName, UnityUIFoldoutTemplate groupFoldout)
        {
            ToggleGroup(groupName);
            groupFoldout.ToggleInterior();
        }

        protected virtual void OnClickQuest(Quest quest)
        {
            SelectQuest(quest);
        }

        public virtual void SelectQuest(Quest quest)
        { 
            selectedQuest = quest;
            RepaintSelectedQuest();
        }

        protected virtual void RepaintSelectedQuest()
        {
            if (!m_hasInitialized) return;
            isDrawingSelectionPanel = false;
            currentContentManager.Clear();
            currentIconList = null;
            currentButtonList = null;
            if (selectedQuest != null)
            {
                var contents = GetQuestContents(selectedQuest);
                AddContents(contents);
                var isQuestActive = selectedQuest.GetState() == QuestState.Active;
                var showTrack = showTrackButtonInDetails && isQuestActive && selectedQuest.isTrackable;
                var showAbandon = isQuestActive && selectedQuest.isAbandonable;
                if (trackButtonTemplate != null) trackButtonTemplate.gameObject.SetActive(showTrack);
                if (abandonButtonTemplate != null) abandonButtonTemplate.gameObject.SetActive(showAbandon);
                if (m_questDetailsButtonContainer != null) m_questDetailsButtonContainer.transform.SetAsLastSibling();

                if (m_questDetailsButtonContainer == null) m_questDetailsButtonContainer = null; // Silence warning about never assigned.
                
                //{
                //    var instance = Instantiate<UnityUIButtonTemplate>(abandonButtonTemplate);  //[TODO] Pool.
                //    detailsPanelContentManager.Add(instance, questDetailsContentContainer);
                //}
            }
            isDrawingSelectionPanel = true;

            RefreshNavigableSelectables();
        }

        protected virtual List<QuestContent> GetQuestContents(Quest quest)
        {
            var contents = quest.GetContentList(QuestContentCategory.Journal);
            if (contents.Count == 0 && showDialogueContentIfNoJournalContent) contents = quest.GetContentList(QuestContentCategory.Dialogue);
            return contents;
        }

        public void OnToggleTracking(bool value, object data)
        {
            var quest = data as Quest;
            if (quest == null) return;
            quest.showInTrackHUD = value;
            QuestMachineMessages.RefreshUIs(quest);
        }

        public void ToggleTracking()
        {
            if (selectedQuest != null && selectedQuest.isTrackable)
            {
                OnToggleTracking(!selectedQuest.showInTrackHUD, selectedQuest);
            }
        }

        public void OpenAbandonQuestConfirmationDialog()
        {
            if (abandonQuestPanel == null || selectedQuest == null) return;
            if (abandonQuestNameText != null) abandonQuestNameText.text = StringField.GetStringValue(selectedQuest.title);
            abandonQuestPanel.Open();
        }

        public void ConfirmAbandonQuest()
        {
            if (questJournal == null) return;
            questJournal.AbandonQuest(selectedQuest);
            Repaint();
        }

    }
}
