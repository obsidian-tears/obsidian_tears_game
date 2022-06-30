// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.DialogueSystem.MenuSystem
{

    /// <summary>
    /// Handles the title menu.
    /// </summary>
    public class TitleMenu : MonoBehaviour
    {

        [Tooltip("Index of title scene in build settings.")]
        public int titleSceneIndex = 0;

        [Tooltip("Index of credits scene in build settings.")]
        public int creditsSceneIndex = 2;

        public UIPanel titleMenuPanel;
        public UnityEngine.UI.Button startButton;
        public UnityEngine.UI.Button continueButton;
        public UnityEngine.UI.Button restartButton;
        public UnityEngine.UI.Button loadGameButton;

        public bool actAsSingleton = true;

        public bool neverSleep;

        private SaveHelper m_saveHelper;
        private MusicManager m_musicManager;

        private static TitleMenu m_instance = null;

#if UNITY_2019_3_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitStaticVariables()
        {
            m_instance = null;
        }
#endif

        private void Awake()
        {
            if (actAsSingleton)
            {
                if (m_instance != null)
                {
                    Destroy(gameObject);
                    return;
                }
                else
                {
                    m_instance = this;
                    if (transform.root != null) transform.SetParent(null, false);
                    DontDestroyOnLoad(gameObject);
                }
            }
            m_saveHelper = GetComponent<SaveHelper>();
            m_musicManager = GetComponent<MusicManager>();
        }

        private void Start()
        {
            UpdateAvailableButtons();
            if (m_musicManager != null) m_musicManager.PlayTitleMusic();
            if (neverSleep) Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        public void OnSceneLoaded(int index)
        {
            if (index == titleSceneIndex)
            {
                titleMenuPanel.Open();
                if (InputDeviceManager.deviceUsesCursor) Tools.SetCursorActive(true);
            }
            else
            {
                titleMenuPanel.Close();
            }
        }

        public void UpdateAvailableButtons()
        {
            UpdateAvailableButtonsNow();
            Invoke("UpdateAvailableButtonsNow", 0.5f);
        }

        private void UpdateAvailableButtonsNow()
        {
            var hasSavedGame = (m_saveHelper != null) ? m_saveHelper.HasAnySavedGame() : false;
            if (startButton != null) startButton.gameObject.SetActive(!hasSavedGame);
            if (continueButton != null) continueButton.gameObject.SetActive(hasSavedGame);
            if (restartButton != null) restartButton.gameObject.SetActive(hasSavedGame);
            if (loadGameButton != null) loadGameButton.gameObject.SetActive(hasSavedGame);
            var selectableToFocus = hasSavedGame ? ((continueButton != null) ? continueButton.gameObject : null) 
                : ((startButton != null) ? startButton.gameObject : null);
            titleMenuPanel.firstSelected = selectableToFocus;
        }

        public void ShowCreditsScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(creditsSceneIndex);
        }

    }

}