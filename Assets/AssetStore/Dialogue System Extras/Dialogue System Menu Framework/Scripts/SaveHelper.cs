// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.SceneManagement;

namespace PixelCrushers.DialogueSystem.MenuSystem
{

    /// <summary>
    /// This script provides utility methods to save, load, and restart games.
    /// 
    /// If you want to record a description with saved games, set the Dialogue System 
    /// variable "CurrentStage". The contents of this variable will be recorded as the
    /// saved game's description.
    /// </summary>
    public class SaveHelper : MonoBehaviour
    {

        [Tooltip("When starting a new game, load this scene.")]
        public string firstGameplayScene = "Gameplay";

        [Tooltip("Show this text in empty saved game slots.")]
        public string emptySlotText = "-empty-";

        [Tooltip("Quick save when this button is pressed.")]
        public string quickSaveButton = string.Empty;

        [Tooltip("Quick load when this button is pressed.")]
        public string quickLoadButton = string.Empty;

        [Tooltip("Add this text to the saved game slot summary, where {0} is the regular slot number text.")]
        public string quickSaveSummaryText = "{0} [QUICK SAVE]";

        public enum SlotFormatInSummary { Omit, CountFrom0, CountFrom1 }

        [Tooltip("How to include the slot number in the saved game summary.")]
        public SlotFormatInSummary slotFormatInSummary = SlotFormatInSummary.CountFrom1;

        [Tooltip("Include the current time in the saved game summary.")]
        public bool includeTimeInSummary = true;

        public int currentSlotNum { get; set; }

        public delegate void SaveSlotDelegate(int slotNum, string saveData);
        public delegate string LoadSlotDelegate(int slotNum);
        public delegate void DeleteSlotDelegate(int slotNum);
        public delegate void RecordExtraSlotDetailsDelegate(int slotNum, ref string summaryInfo);

        public SaveSlotDelegate SaveSlotHandler = null;
        public LoadSlotDelegate LoadSlotHandler = null;
        public DeleteSlotDelegate DeleteSlotHandler = null;
        public RecordExtraSlotDetailsDelegate RecordExtraSlotDetailsHandler = null;

        protected int slotToLoad = 0;

        protected InputDeviceManager m_inputDeviceManager = null;

        protected virtual void Start()
        {
            m_inputDeviceManager = InputDeviceManager.instance;
        }

        protected virtual void Update()
        {
            if (m_inputDeviceManager == null) return;
            if (!string.IsNullOrEmpty(quickSaveButton) && m_inputDeviceManager.GetButtonDown(quickSaveButton))
            {
                QuickSave();
            }
            else if (!string.IsNullOrEmpty(quickLoadButton) && m_inputDeviceManager.GetButtonDown(quickLoadButton))
            {
                QuickLoad();
            }
        }

        protected string GetLastSavedGameKey()
        {
            return "savedgame_lastSlotNum";
        }

        public string GetSlotSummaryKey(int slotNum)
        {
            return "savedgame_" + slotNum + "_summary";
        }

        public string GetSlotDetailsKey(int slotNum)
        {
            return "savedgame_" + slotNum + "_details";
        }

        public int GetQuickSaveSlot()
        {
            if (!PlayerPrefs.HasKey("savedgame_quickSaveSlot"))
            {
                int slot = 0;
                while (IsGameSavedInSlot(slot))
                {
                    slot++;
                }
                PlayerPrefs.SetInt("savedgame_quickSaveSlot", slot);
            }
            return PlayerPrefs.GetInt("savedgame_quickSaveSlot", 0);
        }

        public virtual bool IsGameSavedInSlot(int slotNum)
        {
            return SaveSystem.HasSavedGameInSlot(slotNum);
        }

        public virtual string GetSlotSummary(int slotNum)
        {
            return IsGameSavedInSlot(slotNum) ? PlayerPrefs.GetString(GetSlotSummaryKey(slotNum)) : emptySlotText;
        }

        public virtual string GetSlotDetails(int slotNum)
        {
            return IsGameSavedInSlot(slotNum) ? PlayerPrefs.GetString(GetSlotDetailsKey(slotNum)) : string.Empty;
        }

        public virtual string GetCurrentSummary(int slotNum)
        {
            var summary = GetSlotNumText(slotNum);
            if (includeTimeInSummary)
            {
                if (!string.IsNullOrEmpty(summary)) summary += "\n";
                summary += "Time: " + System.DateTime.Now;
            }
            return summary;
        }

        private string GetSlotNumText(int slotNum)
        {
            switch (slotFormatInSummary)
            {
                case SlotFormatInSummary.CountFrom0:
                    return "Slot " + slotNum;
                case SlotFormatInSummary.CountFrom1:
                    return "Slot " + (slotNum + 1);
                default:
                    return string.Empty;
            }
        }

        public virtual string GetCurrentDetails(int slotNum)
        {
            var details = GetCurrentSummary(slotNum);
            if (DialogueLua.DoesVariableExist("CurrentStage"))
            {
                details += "\n" + DialogueLua.GetVariable("CurrentStage").AsString;
            }
            if (RecordExtraSlotDetailsHandler != null)
            {
                RecordExtraSlotDetailsHandler(slotNum, ref details);
            }
            return details;
        }

        public virtual bool HasLastSavedGame()
        {
            return PlayerPrefs.HasKey(GetLastSavedGameKey());
        }

        public virtual bool HasAnySavedGame()
        {
            for (int i = 0; i < 100; i++)
            {
                if (IsGameSavedInSlot(i)) return true;
            }
            return false;
        }

        public virtual void SaveGame(int slotNum)
        {
            SaveGameNow(slotNum);
        }

        public virtual void SaveGameNow(int slotNum)
        {
            SaveSystem.SaveToSlot(slotNum);
            PlayerPrefs.SetString(GetSlotSummaryKey(slotNum), GetCurrentSummary(slotNum));
            PlayerPrefs.SetString(GetSlotDetailsKey(slotNum), GetCurrentDetails(slotNum));
            PlayerPrefs.SetInt(GetLastSavedGameKey(), slotNum);
        }

        public virtual void LoadGame(int slotNum)
        {
            LoadGameNow(slotNum);
        }

        public virtual void LoadGameNow(int slotNum)
        {
            DialogueManager.StopConversation();
            SaveSystem.LoadFromSlot(slotNum);
        }

        public virtual void LoadLastSavedGame()
        {
            if (HasLastSavedGame())
            {
                LoadGame(PlayerPrefs.GetInt(GetLastSavedGameKey()));
            }
        }

        public virtual void LoadCurrentSlot()
        {
            LoadGame(currentSlotNum);
        }

        public virtual void QuickSave()
        {
            SaveGame(GetQuickSaveSlot());
            Invoke("SetQuickSaveSlotSummaryText", 0.5f);

        }

        protected void SetQuickSaveSlotSummaryText()
        {
            var slotNum = PlayerPrefs.GetInt(GetLastSavedGameKey());
            var slotSummaryKey = GetSlotSummaryKey(slotNum);
            var summary = PlayerPrefs.GetString(slotSummaryKey);
            PlayerPrefs.SetString(slotSummaryKey, string.Format(quickSaveSummaryText, summary));
        }

        public virtual void QuickLoad()
        {
            LoadLastSavedGame();
        }

        public virtual void RestartGame()
        {
            DialogueManager.ResetDatabase(DatabaseResetOptions.KeepAllLoaded);
            SaveSystem.RestartGame(firstGameplayScene);
        }

        public virtual void LoadLevel(string levelName, int loadingSceneIndex = -1)
        {
            SaveSystem.LoadScene(levelName);
        }

        public virtual void HandleLevelLoaded(int level)
        {
            DialogueManager.SendUpdateTracker();
        }

        public virtual void DeleteSavedGame(int slotNum)
        {
            SaveSystem.DeleteSavedGameInSlot(slotNum);
            PlayerPrefs.DeleteKey(GetSlotSummaryKey(slotNum));
            PlayerPrefs.DeleteKey(GetSlotDetailsKey(slotNum));
            var lastSlotNum = PlayerPrefs.GetInt(GetLastSavedGameKey());
            if (lastSlotNum == slotNum) PlayerPrefs.DeleteKey(GetLastSavedGameKey());
            if (DeleteSlotHandler != null) DeleteSlotHandler(slotNum);
        }

        public virtual void ReturnToTitleMenu()
        {
            SaveSystem.BeforeSceneChange();
            SaveSystem.LoadScene("index:" + FindObjectOfType<TitleMenu>().titleSceneIndex);
            SaveSystem.sceneLoaded += OpenTitleMenuOnSceneLoaded;
        }

        private void OpenTitleMenuOnSceneLoaded(string sceneName, int sceneIndex)
        {
            SaveSystem.sceneLoaded += OpenTitleMenuOnSceneLoaded;
            FindObjectOfType<TitleMenu>().titleMenuPanel.Open();
        }

        public void HaltProgram()
        {
#if UNITY_STANDALONE
            Application.Quit();
#endif

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

    }

}
