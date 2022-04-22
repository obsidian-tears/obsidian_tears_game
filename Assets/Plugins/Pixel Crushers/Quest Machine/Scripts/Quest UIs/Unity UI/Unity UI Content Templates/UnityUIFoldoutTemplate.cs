// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Unity UI template for foldouts.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class UnityUIFoldoutTemplate : UnityUIContentTemplate
    {

        [SerializeField]
        private UnityEngine.UI.Button m_foldoutButton;

        [SerializeField]
        private UITextField m_foldoutText;

        [SerializeField]
        private RectTransform m_interiorPanel;

        public UnityEngine.UI.Button foldoutButton
        {
            get { return m_foldoutButton; }
            set { m_foldoutButton = value; }
        }

        public UITextField foldoutText
        {
            get { return m_foldoutText; }
            set { m_foldoutText = value; }
        }

        public RectTransform interiorPanel
        {
            get { return m_interiorPanel; }
            set { m_interiorPanel = value; }
        }

        protected UnityUIInstancedContentManager contentManager { get; set; }

        public virtual void Awake()
        {
            if (foldoutButton == null && Debug.isDebugBuild) Debug.LogError("Quest Machine: Foldout Button is unassigned.", this);
            if (UITextField.IsNull(foldoutText) && Debug.isDebugBuild) Debug.LogError("Quest Machine: Foldout Text is unassigned.", this);
            if (interiorPanel == null && Debug.isDebugBuild) Debug.LogError("Quest Machine: Interior Panel is unassigned.", this);
        }

        public void Assign(string text, bool expanded)
        {
            if (contentManager == null) contentManager = new UnityUIInstancedContentManager();
            contentManager.Clear();
            name = text;
            foldoutText.text = text;
            interiorPanel.gameObject.SetActive(expanded);
        }

        public void ToggleInterior()
        {
            interiorPanel.gameObject.SetActive(!interiorPanel.gameObject.activeSelf);
        }

    }
}
