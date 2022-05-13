// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Unity UI implementation of QuestDialogueUI.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class UnityUIQuestDialogueUI : UnityUIBaseUI, IQuestDialogueUI, IMessageHandler
    {

        #region Serialized Fields

        [SerializeField]
        private UnityEngine.UI.Button m_cancelButton;
        [SerializeField]
        private UnityEngine.UI.Button m_closeButton;
        [SerializeField]
        private UnityEngine.UI.Button m_backButton;
        [SerializeField]
        private UnityEngine.UI.Button m_acceptButton;
        [SerializeField]
        private UnityEngine.UI.Button m_declineButton;
        [SerializeField]
        private RectTransform m_contentContainer;

        [Header("UI Templates")]

        [SerializeField]
        private UnityUITextTemplate m_headingTemplate;
        [SerializeField]
        private UnityUITextTemplate[] m_subheadingTemplates;
        [SerializeField]
        private UnityUITextTemplate m_bodyTemplate;
        [SerializeField]
        private UnityUIIconListTemplate m_iconListTemplate;
        [SerializeField]
        private UnityUIButtonListTemplate m_buttonListTemplate;

        #endregion

        #region Accessor Properties for Serialized Fields

        public UnityEngine.UI.Button cancelButton
        {
            get { return m_cancelButton; }
            set { m_cancelButton = value; }
        }
        public UnityEngine.UI.Button closeButton
        {
            get { return m_closeButton; }
            set { m_closeButton = value; }
        }
        public UnityEngine.UI.Button backButton
        {
            get { return m_backButton; }
            set { m_backButton = value; }
        }
        public UnityEngine.UI.Button acceptButton
        {
            get { return m_acceptButton; }
            set { m_acceptButton = value; }
        }
        public UnityEngine.UI.Button declineButton
        {
            get { return m_declineButton; }
            set { m_declineButton = value; }
        }
        public RectTransform contentContainer
        {
            get { return m_contentContainer; }
            set { m_contentContainer = value; }
        }

        public UnityUITextTemplate headingTemplate
        {
            get { return m_headingTemplate; }
            set { m_headingTemplate = value; }
        }

        public UnityUITextTemplate[] subheadingTemplates
        {
            get { return m_subheadingTemplates; }
            set { m_subheadingTemplates = value; }
        }
        public UnityUITextTemplate bodyTemplate
        {
            get { return m_bodyTemplate; }
            set { m_bodyTemplate = value; }
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

        #endregion

        #region Runtime Properties

        protected UnityUIInstancedContentManager contentManager { get; set; }
        protected override RectTransform currentContentContainer { get { return contentContainer; } }
        protected override UnityUIInstancedContentManager currentContentManager { get { return contentManager; } }
        protected override UnityUITextTemplate currentHeadingTemplate { get { return headingTemplate; } }
        protected override UnityUITextTemplate[] currentSubheadingTemplates { get { return subheadingTemplates; } }
        protected override UnityUITextTemplate currentBodyTemplate { get { return bodyTemplate; } }
        protected override UnityUIIconListTemplate currentIconListTemplate { get { return iconListTemplate; } }
        protected override UnityUIButtonListTemplate currentButtonListTemplate { get { return buttonListTemplate; } }
        protected UIScrollbarEnabler scrollbarEnabler { get; set; }

        protected Quest selectedQuest { get; set; }
        protected QuestParameterDelegate acceptHandler { get; set; }
        protected QuestParameterDelegate declineHandler { get; set; }
        protected QuestParameterDelegate backHandler { get; set; }
        protected Coroutine selectCoroutine { get; set; }

        #endregion

        protected override void Awake()
        {
            base.Awake();
            contentManager = new UnityUIInstancedContentManager();
            if (contentContainer == null && Debug.isDebugBuild) Debug.LogError("Quest Machine: Content Container is unassigned.", this);
            scrollbarEnabler = GetComponentInChildren<UIScrollbarEnabler>();
        }

        protected virtual void OnEnable()
        {
            MessageSystem.AddListener(this, QuestMachineMessages.GroupButtonClickedMessage, string.Empty);
        }

        protected virtual void OnDisable()
        {
            MessageSystem.RemoveListener(this);
        }

        public virtual void ShowContents(QuestParticipantTextInfo speaker, List<QuestContent> contents)
        {
            Show();
            mainPanel.gameObject.SetActive(true);
            SetContents(speaker, contents);
            SetControlButtons(true, false, false);
            if (scrollbarEnabler != null) scrollbarEnabler.CheckScrollbarWithResetValue(1);
        }

        protected virtual bool ContainsGroupButton(List<QuestContent> contents)
        {
            if (contents == null) return false;
            for (int i = 0; i < contents.Count; i++)
            {
                var buttonContent = contents[i] as ButtonQuestContent;
                if (buttonContent != null && buttonContent.groupNumber != ButtonQuestContent.NoGroup) return true;
            }
            return false;
        }

        protected virtual void SetControlButtons(bool enableClose, bool enableBack, bool enableAcceptDecline)
        {
            SetControlButtonsInteractable(true);            
            closeButton.gameObject.SetActive(enableClose);
            backButton.gameObject.SetActive(enableBack);
            acceptButton.gameObject.SetActive(enableAcceptDecline);
            declineButton.gameObject.SetActive(enableAcceptDecline);
            if (InputDeviceManager.autoFocus)
            {
                var selectable = enableAcceptDecline ? declineButton
                    : (enableBack ? backButton : closeButton);
                if (selectCoroutine != null) StopCoroutine(selectCoroutine);
                selectCoroutine = StartCoroutine(SelectAfterOneFrame(selectable));
            }
            RefreshNavigableSelectables();
        }

        protected virtual void SetControlButtonsInteractable(bool value)
        {
            if (cancelButton != null) cancelButton.interactable = value;
            closeButton.interactable = value;
            backButton.interactable = value;
            acceptButton.interactable = value;
            declineButton.interactable = value;
        }

        protected IEnumerator SelectAfterOneFrame(UnityEngine.UI.Selectable selectable)
        {
            yield return null;
            if (selectable != null)
            {
                selectable.Select();
            }
            selectCoroutine = null;
        }

        public virtual void ShowOfferConditionsUnmet(QuestParticipantTextInfo speaker, List<QuestContent> contents, List<Quest> quests)
        {
            // Show a quest's unofferable contents:
            foreach (var quest in quests)
            {
                if (quest.offerConditionsUnmetContentList != null && quest.offerConditionsUnmetContentList.Count > 0)
                {
                    ShowContents(speaker, quest.offerConditionsUnmetContentList);
                    return;
                }
            }
            // If no quests have unofferable contents, show 'no offerable quests' text:
            ShowContents(speaker, contents);

        }

        public virtual void ShowOfferQuest(QuestParticipantTextInfo speaker, Quest quest, QuestParameterDelegate acceptHandler, QuestParameterDelegate declineHandler)
        {
            selectedQuest = quest;
            this.acceptHandler = acceptHandler;
            this.declineHandler = declineHandler;
            ShowContents(speaker, quest.offerContentList);
            SetControlButtons(false, false, true);
        }

        public void AcceptQuest()
        {
            acceptHandler(selectedQuest);
        }

        public void DeclineQuest()
        {
            declineHandler(selectedQuest);
        }

        public void Back()
        {
            backHandler(selectedQuest);
        }

        public void SetBackHandler(QuestParameterDelegate backHandler)
        {
            this.backHandler = backHandler;
            backButton.gameObject.SetActive(backHandler != null);
        }

        public virtual void ShowActiveQuest(QuestParticipantTextInfo speaker, Quest quest, QuestParameterDelegate continueHandler, QuestParameterDelegate backHandler)
        {
            selectedQuest = quest;
            this.backHandler = backHandler;
            var contents = quest.GetContentList(QuestContentCategory.Dialogue, speaker);
            ShowContents(speaker, contents);
            SetControlButtons(true, backHandler != null, false);
            if (ContainsGroupButton(contents)) SetControlButtonsInteractable(false);
        }

        public virtual void ShowCompletedQuest(QuestParticipantTextInfo speaker, List<Quest> quests)
        {
            if (quests == null || quests.Count == 0) return;
            var quest = quests[0];
            var contents = quest.GetContentList(QuestContentCategory.Dialogue);
            ShowContents(speaker, contents);
            SetControlButtons(true, false, false);
            if (ContainsGroupButton(contents)) SetControlButtonsInteractable(false);
        }

        public virtual void ShowQuestList(QuestParticipantTextInfo speaker, List<QuestContent> activeQuestsContents, List<Quest> activeQuests,
            List<QuestContent> offerableQuestsContents, List<Quest> offerableQuests, QuestParameterDelegate selectHandler)
        {
            ShowContents(speaker, null);
            SetControlButtons(true, false, false);
            if (activeQuests != null && activeQuests.Count > 0)
            {
                currentButtonList = null;
                AddQuestList(activeQuestsContents, activeQuests, selectHandler);
            }
            if (offerableQuests != null && offerableQuests.Count > 0)
            {
                currentButtonList = null;
                AddQuestList(offerableQuestsContents, offerableQuests, selectHandler);
            }
        }

        public void AddQuestList(List<QuestContent> contents, List<Quest> quests, QuestParameterDelegate selectHandler)
        {
            AddContents(contents);
            if (quests == null) return;
            for (int i = 0; i < quests.Count; i++)
            {
                var quest = quests[i];
                if (quest == null) continue;
                PrepareButtonList();
                if (selectHandler != null)
                {
                    currentButtonList.AddButton(quest.icon, 1, StringField.GetStringValue(quest.title), delegate { selectHandler(quest); });
                }
                else
                {
                    currentButtonList.AddButton(quest.icon, 1, StringField.GetStringValue(quest.title), null);
                }
            }
        }

        public void OnMessage(MessageArgs messageArgs)
        {
            if (string.Equals(messageArgs.message, QuestMachineMessages.GroupButtonClickedMessage))
            {
                SetControlButtonsInteractable(true);
                var clickedGroupNumber = messageArgs.intValue;
                for (int i = 0; i < contentManager.instancedContent.Count; i++)
                {
                    var buttonList = contentManager.instancedContent[i] as UnityUIButtonListTemplate;
                    if (buttonList != null && buttonList.instances != null)
                    {
                        for (int j = 0; j < buttonList.instances.Count; j++)
                        {
                            var button = buttonList.instances[j] as UnityUIButtonTemplate;
                            if (button != null && button.groupNumber == clickedGroupNumber && button.button != null)
                            {
                                button.button.interactable = false;
                            }
                        }
                    }
                }
            }
        }
    }

}
