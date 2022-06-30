// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.DialogueSystem.MenuSystem
{

    /// <summary>
    /// This script provides utility functions to the PausePanel.
    /// </summary>
    public class Pause : MonoBehaviour
    {

        public UIPanel pausePanel;

        [Tooltip("This button opens the pause menu.")]
        public string pauseButton = "Cancel";

        [Tooltip("Don't allow the player to open the pause menu during conversations.")]
        public bool blockDuringConversations = true;

        [Tooltip("Quest log window to open when the Quests button is clicked.")]
        public QuestLogWindow questLogWindow;

        private TitleMenu m_titleMenu = null;

        private void Awake()
        {
            m_titleMenu = GetComponent<TitleMenu>();
        }

        private void Update()
        {
            if (!pausePanel.isOpen && !m_titleMenu.titleMenuPanel.isOpen && InputDeviceManager.IsButtonDown(pauseButton) && 
                !(blockDuringConversations && DialogueManager.IsConversationActive))
            {
                pausePanel.Open();
            }
        }

        public void SetCursorActive(bool value)
        {
            Tools.SetCursorActive(value);
        }

        public void SetTimeScale(float timeScale)
        {
            Time.timeScale = timeScale;
        }

        public void DelayedPause(float delay)
        {
            Invoke("PauseNow", delay);
        }

        private void PauseNow()
        {
            Time.timeScale = 0;
        }

        public void SetSelectorHUD(bool value)
        {
            var selector = FindObjectOfType<Selector>();
            var proximitySelector = FindObjectOfType<ProximitySelector>();
            var unityUISelectorDisplay = (selector != null) ? selector.GetComponent<UnityUISelectorDisplay>() : null;
            if (unityUISelectorDisplay == null && proximitySelector != null) unityUISelectorDisplay = proximitySelector.GetComponent<UnityUISelectorDisplay>();
            if (selector != null) selector.enabled = value;
            if (proximitySelector != null) proximitySelector.enabled = value;
            if (unityUISelectorDisplay != null)
            {
                if (unityUISelectorDisplay.mainGraphic != null) unityUISelectorDisplay.mainGraphic.gameObject.SetActive(value);
                if (unityUISelectorDisplay.reticleInRange != null) unityUISelectorDisplay.reticleInRange.gameObject.SetActive(value);
                if (unityUISelectorDisplay.reticleOutOfRange != null) unityUISelectorDisplay.reticleOutOfRange.gameObject.SetActive(value);
            }
        }

        public void OpenQuestLogWindow()
        {
            if (questLogWindow == null) questLogWindow = FindObjectOfType<QuestLogWindow>();
            if (questLogWindow == null) return;
            questLogWindow.Open();
        }

    }
}