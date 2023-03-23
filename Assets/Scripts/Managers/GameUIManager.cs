using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Opsive.UltimateInventorySystem.Demo.UI.Menus.Main.Inventory;
using Opsive.UltimateInventorySystem.UI.CompoundElements;
using Opsive.UltimateInventorySystem.UI.Menus.Shop;
using Opsive.UltimateInventorySystem.UI.Panels;
using Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers;
using Opsive.UltimateInventorySystem.UI.Monitors;
using UnityEngine;
using UnityEngine.UI;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;

namespace GameManagers
{
    public enum UIMode
    {
        STANDARD,
        BATTLE
    }

    /// <summary>
    /// Use this class as a main access point to the game UI
    /// </summary>
    public class GameUIManager : MonoSingleton<GameUIManager>
    {
        [Header("Important components")]
        public InventoryMonitor InventoryMonitor;

        [Header("Menus")]
        public DisplayPanel PlayerUI;
        public DisplayPanel MainMenu;
        public DisplayPanel SpellMenu;
        public CustomShopMenu ShopMenu;
        public GameObject MenuTabs;
        public ItemViewSlotsContainerPanelBinding InventoryPanel;
        public ItemViewSlotsContainerPanelBinding EquipmentPanel;

        [Header("Graphic Elements")]
        public Image LoadingIndicator;
        public Image Blocker;
        public Slider HealthSlider;
        public Slider MagicSlider;
        public CharacterStatsDisplay StatsDisplay;
        public ActionButton[] ButtonsForBattleHide;

        private UIMode m_currentMode = UIMode.STANDARD;
        public UIMode CurrentMode => m_currentMode;

        private bool m_inventoryShowing = false;
        public bool InventoryShowing => m_inventoryShowing;

        public event Action OnSpellWindowClosed;
        protected override void Init()
        {
        }

        // Set UI mode, use this method for UI adjustments in battle
        public void SetUIMode(UIMode mode)
        {
            m_currentMode = mode;
            Debug.Log("Setting UI mode to: "+ mode);

            bool isStandardMode = m_currentMode == UIMode.STANDARD;
            PlayerUI.gameObject.SetActive(isStandardMode);
            foreach (ActionButton btn in ButtonsForBattleHide)
            {
                btn.gameObject.SetActive(isStandardMode);
            }
        }

        public void SetHealthSlider(int healthTotal, int healthMax)
        {
            if (HealthSlider == null)
                return;
            
            HealthSlider.maxValue = healthMax;
            HealthSlider.value = healthTotal;
        }

        public void SetMagicSlider(int magicTotal, int magicMax)
        {
            if (MagicSlider == null)
                return;
            
            MagicSlider.maxValue = magicMax;
            MagicSlider.value = magicTotal;
        }

        public void ToggleBattleInventoryPanel(bool? forceState = null)
        {
            if (InventoryPanel != null)
            {
                m_inventoryShowing = forceState == null ? !m_inventoryShowing : forceState.Value;
                MainMenu.gameObject.SetActive(m_inventoryShowing);
                InventoryPanel.gameObject.SetActive(m_inventoryShowing);
                MenuTabs.gameObject.SetActive(!m_inventoryShowing);
                if (m_inventoryShowing)
                {
                    StartCoroutine(RedrawInventory());
                }
            }
        }

        /// <summary>
        /// WORKAROUND FOR A MISSING INVENTORY IN THE BATTLE
        /// Force manual redraw of the inventory, used in battle
        /// </summary>
        private IEnumerator RedrawInventory()
        {
            yield return null;
            InventoryPanel.OnOpen();
        }

        /// <summary>
        /// Call this from a close spell button when in battle
        /// </summary>
        public void CloseSpellWindow()
        {
            OnSpellWindowClosed?.Invoke();
        }

        /// <summary>
        /// Call this from a close inventory button
        /// </summary>
        public void CloseInventoryWindow()
        {
            if (m_currentMode == UIMode.STANDARD)
            {
                InventoryPanel.GetComponent<DisplayPanel>().SmartClose();
            }
            else
            {
                ToggleBattleInventoryPanel(false);
            }
        }

        public void SetInventoryMonitor(Inventory inventory)
        {
            if (InventoryMonitor != null)
            {
                InventoryMonitor.SetMonitoredInventory(inventory);
            }
            else
            {
                Debug.LogError("No inventory monitor component found on the UI! Please assign one!");
            }
        }

        //TODO this definitely needs some refactor regarding its behaviour (see ReactController for context)
        public void ShowLoadingIndicator(bool show, bool involveBlocker)
        {
            if (LoadingIndicator != null)
            {
                LoadingIndicator.gameObject.SetActive(show);
            }

            if (involveBlocker && Blocker != null)
            {
                Blocker.gameObject.SetActive(show);
            }
        }
    }
}
