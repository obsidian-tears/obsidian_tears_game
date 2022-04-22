// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Unity UI implementation of Quest HUD.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class UnityUIQuestHUD : UnityUIBaseUI, IQuestHUD
    {

        #region Serialized Fields

        [SerializeField]
        private RectTransform m_contentContainer;

        [Header("UI Templates")]

        [SerializeField]
        private UnityUITextTemplate m_questGroupTemplate;
        [SerializeField]
        private UnityUITextTemplate m_activeQuestHeadingTemplate;
        [SerializeField]
        private UnityUITextTemplate m_activeQuestBodyTemplate;
        [SerializeField]
        private UnityUITextTemplate m_completedQuestHeadingTemplate;
        [SerializeField]
        private UnityUITextTemplate m_completedQuestBodyTemplate;
        [SerializeField]
        private UnityUIIconListTemplate m_iconListTemplate;
        [SerializeField]
        private UnityUIButtonListTemplate m_buttonListTemplate;
        [SerializeField]
        private UnityUITextTemplate[] m_subheadingTemplates;

        [Header("Visibility")]

        [SerializeField]
        private bool m_showHUD = true;
        [SerializeField]
        [Tooltip("Organize quests by group.")]
        private bool m_useGroups = false;
        [SerializeField]
        private bool m_showActiveQuests = true;
        [SerializeField]
        private bool m_showSuccessfulQuests = false;
        [SerializeField]
        private bool m_showFailedQuests = false;
        [Tooltip("If no quests are being tracked, hide HUD.")]
        [SerializeField]
        private bool m_hideIfNoTrackedQuests = false;

        #endregion

        #region Accessor Properties for Serialized Fields

        public RectTransform contentContainer
        {
            get { return m_contentContainer; }
            set { m_contentContainer = value; }
        }

        public UnityUITextTemplate questGroupTemplate
        {
            get { return m_questGroupTemplate; }
            set { m_questGroupTemplate = value; }
        }

        public UnityUITextTemplate activeQuestHeadingTemplate
        {
            get { return m_activeQuestHeadingTemplate; }
            set { m_activeQuestHeadingTemplate = value; }
        }

        public UnityUITextTemplate activeQuestBodyTemplate
        {
            get { return m_activeQuestBodyTemplate; }
            set { m_activeQuestBodyTemplate = value; }
        }

        public UnityUITextTemplate completedQuestHeadingTemplate
        {
            get { return m_completedQuestHeadingTemplate; }
            set { m_completedQuestHeadingTemplate = value; }
        }

        public UnityUITextTemplate completedQuestBodyTemplate
        {
            get { return m_completedQuestBodyTemplate; }
            set { m_completedQuestBodyTemplate = value; }
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

        public UnityUITextTemplate[] subheadingTemplates
        {
            get { return m_subheadingTemplates; }
            set { m_subheadingTemplates = value; }
        }

        public bool showHUD
        {
            get { return m_showHUD; }
            set { m_showHUD = value; }
        }

        public bool useGroups
        {
            get { return m_useGroups; }
            set { m_useGroups = value; }
        }
        
        public bool showActiveQuests
        {
            get { return m_showActiveQuests; }
            set { m_showActiveQuests = value; }
        }

        public bool showSuccessfulQuests
        {
            get { return m_showSuccessfulQuests; }
            set { m_showSuccessfulQuests = value; }
        }

        public bool showFailedQuests
        {
            get { return m_showFailedQuests; }
            set { m_showFailedQuests = value; }
        }

        public bool hideIfNoTrackedQuests
        {
            get { return m_hideIfNoTrackedQuests; }
            set { m_hideIfNoTrackedQuests = value; }
        }

        #endregion

        #region Runtime Properties

        protected bool isQuestActive { get; set; }
        protected UnityUIInstancedContentManager contentManager { get; set; }
        protected override RectTransform currentContentContainer { get { return contentContainer; } }
        protected override UnityUIInstancedContentManager currentContentManager { get { return contentManager; } }
        protected override UnityUITextTemplate currentHeadingTemplate { get { return isQuestActive ? activeQuestHeadingTemplate : completedQuestHeadingTemplate; } }
        protected override UnityUITextTemplate[] currentSubheadingTemplates { get { return subheadingTemplates; } }
        protected override UnityUITextTemplate currentBodyTemplate { get { return isQuestActive ? activeQuestBodyTemplate : completedQuestBodyTemplate; } }
        protected override UnityUIIconListTemplate currentIconListTemplate { get { return iconListTemplate; } }
        protected override UnityUIButtonListTemplate currentButtonListTemplate { get { return buttonListTemplate; } }

        protected List<string> expandedGroupNames = new List<string>();
        protected QuestListContainer questListContainer { get; set; }

        protected CanvasGroup m_canvasGroup = null;
        private Coroutine m_refreshCoroutine = null;

        #endregion

        protected override void Awake()
        {
            base.Awake();
            contentManager = new UnityUIInstancedContentManager();
            if (contentContainer == null && Debug.isDebugBuild) Debug.LogError("Quest Machine: Content Container is unassigned.", this);
            if (questGroupTemplate != null) questGroupTemplate.gameObject.SetActive(false);
            if (activeQuestHeadingTemplate != null) activeQuestHeadingTemplate.gameObject.SetActive(false);
            if (activeQuestBodyTemplate != null) activeQuestBodyTemplate.gameObject.SetActive(false);
            if (completedQuestHeadingTemplate != null) completedQuestHeadingTemplate.gameObject.SetActive(false);
            if (completedQuestBodyTemplate != null) completedQuestBodyTemplate.gameObject.SetActive(false);
        }

        protected virtual void OnDisable()
        {
            m_refreshCoroutine = null;
        }

        public virtual void SetVisibility(QuestListContainer questListContainer, bool value)
        {
            showHUD = !showHUD;
            if ((isVisible && !showHUD) || (!isVisible && showHUD)) Toggle(questListContainer);
        }

        public virtual void Show(QuestListContainer questListContainer)
        {
            if (ShouldBeVisible())
            {
                Show();
                Repaint(questListContainer);
            }
        }

        public virtual void Toggle(QuestListContainer questListContainer)
        {
            if (isVisible)
            {
                Hide();
            }
            else
            {
                Show(questListContainer);
            }
        }

        public virtual void Repaint(QuestListContainer questListContainer)
        {
            if (!isVisible) return;
            this.questListContainer = questListContainer;
            if (!(enabled && gameObject.activeInHierarchy)) return;
            if (ShouldBeVisible())
            {
                if (m_refreshCoroutine == null) m_refreshCoroutine = StartCoroutine(RefreshAtEndOfFrame());
            }
            else
            { 
                MakeInvisible();
            }
        }

        private IEnumerator RefreshAtEndOfFrame()
        {
            // Wait until end of frame so we only refresh once in case we receive multiple
            // requests to refresh during the same frame.
            yield return new WaitForEndOfFrame();
            m_refreshCoroutine = null;
            RefreshNow();
        }

        protected virtual void RefreshNow() //[TODO] 1. Optimize. 2. Toggle groups.
        {
            MakeVisible();
            currentContentManager.Clear();

            if (useGroups && questGroupTemplate != null)
            {
                RefreshByGroup();
            }
            else
            {
                AddQuestsToContent(null, true, false);
            }
        }

        protected virtual void RefreshByGroup()
        {
            var groups = new List<StringField>();
            foreach (var quest in questListContainer.questList)
            {
                if (quest == null || StringField.IsNullOrEmpty(quest.group)) continue;
                if (groups.Contains(quest.group)) continue;
                var state = quest.GetState();
                var showThisQuest = quest.showInTrackHUD &&
                    ((state == QuestState.Active && showActiveQuests) ||
                    (state == QuestState.Successful && showSuccessfulQuests) ||
                    (state == QuestState.Failed && showFailedQuests));
                if (showThisQuest) groups.Add(quest.group);
            }
            SortGroupNames(groups);
            foreach (var group in groups)
            {
                var instance = Instantiate<UnityUITextTemplate>(questGroupTemplate);
                currentContentManager.Add(instance, currentContentContainer);
                instance.Assign(StringField.GetStringValue(group));
                AddQuestsToContent(group, false, false);
            }
            AddQuestsToContent(null, false, true);
        }

        /// <summary>
        /// You can override this method to sort differently.
        /// </summary>
        protected virtual void SortGroupNames(List<StringField> groups)
        {
            groups.Sort((x, y) => string.Compare(StringField.GetStringValue(x), StringField.GetStringValue(y)));
        }

        protected virtual void AddQuestsToContent(StringField group, bool showAll, bool showUngrouped)
        {
            foreach (var quest in questListContainer.questList)
            {
                if (quest == null) continue;

                var showInThisGroup = showAll ||
                    (showUngrouped && StringField.IsNullOrEmpty(quest.group)) ||
                    (StringField.GetStringValue(quest.group) == StringField.GetStringValue(group));
                if (!showInThisGroup) continue;

                var state = quest.GetState();
                var showThisQuest = quest.showInTrackHUD &&
                    ((state == QuestState.Active && showActiveQuests) ||
                    (state == QuestState.Successful && showSuccessfulQuests) ||
                    (state == QuestState.Failed && showFailedQuests));
                if (!showThisQuest) continue;

                var stateInfo = quest.GetStateInfo(state);
                var contentList = new List<QuestContent>();
                contentList.AddRange(stateInfo.GetContentList(QuestContentCategory.HUD));
                isQuestActive = quest.GetState() == QuestState.Active;
                AddContents(contentList);

                for (int i = 0; i < quest.nodeList.Count; i++)
                {
                    contentList.Clear();
                    var nodeUIContents = quest.nodeList[i].GetContentList(QuestContentCategory.HUD);
                    if (nodeUIContents != null) contentList.AddRange(nodeUIContents);
                    isQuestActive = quest.nodeList[i].GetState() == QuestNodeState.Active;
                    AddContents(contentList);
                }
            }
        }

        private UnityUITextTemplate GetQuestNameTemplateForState(QuestState state)
        {
            return (state == QuestState.Active) ? activeQuestHeadingTemplate : completedQuestHeadingTemplate;
        }

        /// <summary>
        /// True if the group is expanded in the UI.
        /// </summary>
        public virtual bool IsGroupExpanded(string groupName)
        {
            return expandedGroupNames.Contains(groupName);
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

        protected virtual bool ShouldBeVisible()
        {
            if (!showHUD) return false;
            if (!hideIfNoTrackedQuests) return true;
            foreach (var quest in questListContainer.questList)
            {
                if (quest == null) continue;
                var state = quest.GetState();
                var showThisQuest = quest.showInTrackHUD &&
                    ((state == QuestState.Active && showActiveQuests) ||
                    (state == QuestState.Successful && showSuccessfulQuests) ||
                    (state == QuestState.Failed && showFailedQuests));
                if (showThisQuest) return true;
            }
            return false;
        }

        protected virtual void MakeInvisible()
        {
            if (m_canvasGroup == null)
            {
                m_canvasGroup = GetComponent<CanvasGroup>();
                if (m_canvasGroup == null) m_canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            m_canvasGroup.alpha = 0;
            if (m_refreshCoroutine != null)
            {
                StopCoroutine(m_refreshCoroutine);
                m_refreshCoroutine = null;
            }
        }

        protected virtual void MakeVisible()
        {
            if (m_canvasGroup == null) return;
            m_canvasGroup.alpha = 1;
        }

    }
}
