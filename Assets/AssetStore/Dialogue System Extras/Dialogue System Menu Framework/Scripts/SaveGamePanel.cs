// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.DialogueSystem.MenuSystem
{

    /// <summary>
    /// This script handles the SaveGamePanel.
    /// </summary>
    public class SaveGamePanel : MonoBehaviour
    {

        [Tooltip("Game slots.")]
        public UnityEngine.UI.Button[] slots;

        [Tooltip("Panel to show to confirm player wants to overwrite a saved game.")]
        public UIPanel confirmOverwritePanel;

        private SaveHelper m_saveHelper = null;
        private int m_currentSlotNum = -1;

        private void Awake()
        {
            m_saveHelper = FindObjectOfType<SaveHelper>();
        }

        public void SetupPanel()
        {
            for (int slotNum = 0; slotNum < slots.Length; slotNum++)
            {
                var slot = slots[slotNum];
                var slotLabel = slot.GetComponentInChildren<UnityEngine.UI.Text>();
                if (slotLabel != null) slotLabel.text = m_saveHelper.GetSlotSummary(slotNum);
#if TMP_PRESENT
                var tmpLabel = slot.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (tmpLabel != null) tmpLabel.text = m_saveHelper.GetSlotSummary(slotNum);
#endif
            }
        }

        public void SelectSlot(int slotNum)
        {
            m_currentSlotNum = slotNum;
            if (m_saveHelper.IsGameSavedInSlot(slotNum))
            {
                confirmOverwritePanel.Open();
            }
            else
            {
                ConfirmSave();
            }
        }

        public void ConfirmSave()
        {
            m_saveHelper.SaveGame(m_currentSlotNum);
            GetComponent<UIPanel>().Close();
        }

    }

}