// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine.Demo
{

    /// <summary>
    /// Simple menu methods for the Quest Machine demo. Saving and loading
    /// use the optional Save System included with Quest Machine.
    /// </summary>
    public class DemoMenu : MonoBehaviour
    {

        public void Awake()
        {
            SaveSystem.sceneLoaded += OnSceneLoaded;
        }

        public void ToggleMenu()
        {
            if (!gameObject.activeInHierarchy && (QuestMachine.defaultQuestJournalUI.isVisible || QuestMachine.defaultQuestDialogueUI.isVisible)) return;
            GetComponent<UIPanel>().Toggle();
        }

        public void SaveGame()
        {
            SaveSystem.SaveToSlot(1);
            QuestMachine.defaultQuestAlertUI.ShowAlert("Game saved.");
        }

        public void LoadGame()
        {
            QuestMachine.defaultQuestAlertUI.ShowAlert("Loading game.");
            SaveSystem.LoadFromSlot(1);
        }

        private void OnSceneLoaded(string sceneName, int sceneIndex)
        {
            // After loading a scene (including loading saved games), refresh the UIs.
            QuestMachineMessages.RefreshUIs(this);
        }

        public void QuitGame()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

    }
}
