// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Base functionality for Unity UI quest UIs.
    /// </summary>
    public abstract class UnityUIBaseUI : MonoBehaviour
    {

        [Header("UI Elements")]

        [SerializeField]
        private RectTransform m_mainPanel;

        [SerializeField]
        private UITextField m_entityName;

        [SerializeField]
        private UnityEngine.UI.Image m_entityImage;

        public bool isVisible { get { return (uiPanel != null) ? uiPanel.isOpen : ((mainPanel != null) ? mainPanel.gameObject.activeInHierarchy : false); } }

        public RectTransform mainPanel
        {
            get { return m_mainPanel; }
            set { m_mainPanel = value; }
        }

        public UITextField entityName
        {
            get { return m_entityName; }
            set { m_entityName = value; }
        }

        public UnityEngine.UI.Image entityImage
        {
            get { return m_entityImage; }
            set { m_entityImage = value; }
        }

        protected UIPanel uiPanel { get; set; }
        protected UnityUIIconListTemplate currentIconList { get; set; }
        protected UnityUIButtonListTemplate currentButtonList { get; set; }

        protected abstract RectTransform currentContentContainer { get; }
        protected abstract UnityUIInstancedContentManager currentContentManager { get; }
        protected abstract UnityUITextTemplate currentHeadingTemplate { get; }
        protected abstract UnityUITextTemplate[] currentSubheadingTemplates { get; }
        protected abstract UnityUITextTemplate currentBodyTemplate { get; }
        protected abstract UnityUIIconListTemplate currentIconListTemplate { get; }
        protected abstract UnityUIButtonListTemplate currentButtonListTemplate { get; }

        protected virtual void Awake()
        {
            uiPanel = GetComponent<UIPanel>();
            currentIconList = null;
            currentButtonList = null;
            InitializeTemplates();
        }

        protected virtual void InitializeTemplates()
        {
            if (currentHeadingTemplate == null && Debug.isDebugBuild) Debug.LogError("Quest Machine: Heading Template is unassigned.", this);
            if (currentBodyTemplate == null && Debug.isDebugBuild) Debug.LogError("Quest Machine: Body Template is unassigned.", this);
            if (currentIconListTemplate == null && Debug.isDebugBuild) Debug.LogError("Quest Machine: Icon List Template is unassigned.", this);
            if (currentIconListTemplate != null && currentIconListTemplate.iconTemplate == null && Debug.isDebugBuild) Debug.LogError("Quest Machine: Icon List Template's Icon Template is unassigned.", this);
            if (currentButtonListTemplate == null && Debug.isDebugBuild) Debug.LogError("Quest Machine: Button List Template is unassigned.", this);
            if (currentButtonListTemplate != null && currentButtonListTemplate.buttonTemplate == null && Debug.isDebugBuild) Debug.LogError("Quest Machine: Button List Template's Button Template is unassigned.", this);
            if (currentHeadingTemplate != null) currentHeadingTemplate.gameObject.SetActive(false);
            if (currentBodyTemplate != null) currentBodyTemplate.gameObject.SetActive(false);
            if (currentIconListTemplate != null) currentIconListTemplate.gameObject.SetActive(false);
            if (currentButtonListTemplate != null) currentButtonListTemplate.gameObject.SetActive(false);
            if (currentSubheadingTemplates != null)
            {
                for (int i = 0; i < currentSubheadingTemplates.Length; i++)
                {
                    if (currentSubheadingTemplates[i] != null) currentSubheadingTemplates[i].gameObject.SetActive(false);
                }
            }
        }

        public void RefreshNavigableSelectables()
        {
            if (uiPanel != null) uiPanel.RefreshAfterOneFrame();
        }

        public virtual void Show()
        {
            if (uiPanel == null) uiPanel = GetComponent<UIPanel>();
            if (uiPanel != null)
            {
                if (!uiPanel.isOpen) uiPanel.Open();
            }
            mainPanel.gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            if (uiPanel != null)
            {
                uiPanel.Close();
            }
            else
            {
                mainPanel.gameObject.SetActive(false);
            }
        }

        protected virtual UnityUITextTemplate GetHeadingTemplate(int level)
        {
            if (level <= 1) return currentHeadingTemplate;
            var index = level - 2;
            if (currentSubheadingTemplates != null && index < currentSubheadingTemplates.Length && currentSubheadingTemplates[index] != null) return currentSubheadingTemplates[index];
            if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: Quest content wants to use heading level " + level + " but no template is defined for it in the UI. Using the main heading template.", this);
            return currentHeadingTemplate;
        }

        protected virtual void SetContents(QuestParticipantTextInfo speaker, List<QuestContent> contents)
        {
            var displayName = (speaker != null) ? StringField.GetStringValue(speaker.displayName) : null;
            var image = (speaker != null) ? speaker.image : null;
            SetContents(displayName, image, contents);
        }

        protected virtual void SetContents(string displayName, Sprite image, List<QuestContent> contents)
        {
            if (entityName != null) entityName.text = displayName;
            if (entityImage != null) entityImage.sprite = image;
            if (currentContentContainer == null)
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: Current content container is unassigned; not adding UI content.", this);
                return;
            }
            currentContentManager.Clear();
            currentIconList = null;
            currentButtonList = null;
            AddContents(contents);
            RefreshNavigableSelectables();
        }

        protected virtual void AddContents(List<QuestContent> contents)
        {
            if (contents == null) return;
            for (int i = 0; i < contents.Count; i++)
            {
                AddContent(contents[i]);
            }
        }

        protected virtual void AddContent(QuestContent content) //[TODO] Pool template instances for reuse.
        {
            if (content == null) return;
            if (content is HeadingTextQuestContent)
            {
                AddHeadingContent(content as HeadingTextQuestContent);
            }
            else if (content is BodyTextQuestContent)
            {
                AddBodyContent(content as BodyTextQuestContent);
            }
            else if (content is ButtonQuestContent)
            {
                AddButtonContent(content as ButtonQuestContent);
            }
            else if (content is IconQuestContent)
            {
                AddIconContent(content as IconQuestContent);
            }
            else if (content is AudioClipQuestContent)
            {
                AddAudioContent(content as AudioClipQuestContent);
            }
            else
            {
                AddGenericContent(content);
            }
        }

        protected virtual void AddHeadingContent(HeadingTextQuestContent headingContent)
        {
            var instance = Instantiate<UnityUITextTemplate>(GetHeadingTemplate(headingContent.headingLevel));
            currentContentManager.Add(instance, currentContentContainer);
            instance.Assign(headingContent.runtimeText);
            currentIconList = null;
        }

        protected virtual void AddBodyContent(BodyTextQuestContent bodyContent)
        {
            var instance = Instantiate<UnityUITextTemplate>(currentBodyTemplate);
            currentContentManager.Add(instance, currentContentContainer);
            instance.Assign(bodyContent.runtimeText);
            currentIconList = null;
        }

        protected virtual void AddBodyContent(string bodyContentString)
        {
            var instance = Instantiate<UnityUITextTemplate>(currentBodyTemplate);
            currentContentManager.Add(instance, currentContentContainer);
            instance.Assign(bodyContentString);
            currentIconList = null;
        }

        protected virtual void AddIconContent(IconQuestContent iconContent)
        {
            PrepareIconList();
            currentIconList.AddIcon(iconContent);
        }

        protected virtual void PrepareIconList()
        {
            if (currentIconList == null) currentIconList = AddIconList();
        }

        protected virtual UnityUIIconListTemplate AddIconList()
        {
            var iconList = Instantiate<UnityUIIconListTemplate>(currentIconListTemplate);
            currentContentManager.Add(iconList, currentContentContainer);
            return iconList;
        }

        protected virtual void AddButtonContent(ButtonQuestContent buttonContent)
        {
            PrepareButtonList();
            currentButtonList.AddButton(buttonContent);
        }

        protected virtual void PrepareButtonList()
        {
            if (currentButtonList == null) currentButtonList = AddButtonList();
        }

        protected virtual UnityUIButtonListTemplate AddButtonList()
        {
            var buttonList = Instantiate<UnityUIButtonListTemplate>(currentButtonListTemplate);
            currentContentManager.Add(buttonList, currentContentContainer);
            return buttonList;
        }

        protected virtual void AddAudioContent(AudioClipQuestContent audioContent)
        {
            if (audioContent == null || audioContent.audioClip == null || audioContent.useAudioSourceOn == null) return;
            audioContent.useAudioSourceOn.Play(audioContent.audioClip);
        }

        /// <summary>
        /// If you're defining new content types, you may want to override this method
        /// to handle your new content types specially.
        /// </summary>
        protected virtual void AddGenericContent(QuestContent content)
        {
            if (content == null) return;
            var runtimeText = content.runtimeText;
            if (string.IsNullOrEmpty(runtimeText)) return;
            AddBodyContent(runtimeText);
        }

        protected virtual void ClearContent()
        {
            currentContentManager.Clear();
        }

    }
}
