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
using Opsive.UltimateInventorySystem.Exchange;
using Opsive.UltimateInventorySystem.UI.Item.ItemViewModules;
using TMPro;

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
        [Header("Monitor components")]
        public InventoryMonitor InventoryMonitor;
        public List<CurrencyOwnerMonitor> CurrencyMonitors;

        [Header("Menus")]
        public DisplayPanel PlayerUI;
        public DisplayPanel MainMenu;
        public DisplayPanel SpellMenu;
        public CustomShopMenu ShopMenu;
        public GameObject MenuTabs;
        public GameObject LevelUpMenu;
        public GameObject MobileHud;
        public GameObject GameSavedSuccess;
        public ItemViewSlotsContainerPanelBinding InventoryPanel;
        public ItemViewSlotsContainerPanelBinding EquipmentPanel;

        [Header("Graphic Elements")]
        public Image LoadingIndicator;
        public Image Blocker;
        public Slider HealthSlider;
        public Slider MagicSlider;
        public Slider XpSlider;
        public TextMeshProUGUI LevelText;

        public CharacterStatsDisplay StatsDisplay;
        public ActionButton[] ButtonsForBattleHide;
        public MobileInput _mobileInput;

        private UIMode m_currentMode = UIMode.STANDARD;
        public UIMode CurrentMode => m_currentMode;

        private bool m_inventoryShowing = false;
        public bool InventoryShowing => m_inventoryShowing;

        public event Action OnSpellWindowClosed;
        protected override void Init()
        {
            MainMenu.OnClose += OnMainMenuClose;
            Debug.Log("Spawn?");
        }

        protected override void OnDestroy()
        {
            MainMenu.OnClose -= OnMainMenuClose;
        }

        // Set UI mode, use this method for UI adjustments in battle
        public void SetUIMode(UIMode mode)
        {
            m_currentMode = mode;

            bool isStandardMode = m_currentMode == UIMode.STANDARD;
            PlayerUI.gameObject.SetActive(isStandardMode);

            if (CheckMobile.IsMobile)
            {
                if (mode == UIMode.BATTLE)
                {
                    MobileHud.SetActive(false);
                    _mobileInput.enabled = false;
                }
                else
                {
                    MobileHud.SetActive(true);
                    _mobileInput.enabled = true;
                }
            }

            foreach (ActionButton btn in ButtonsForBattleHide)
                btn.gameObject.SetActive(isStandardMode);
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

        public void SetXpSlider(int totalXp, int maxXp)
        {
            if (XpSlider == null)
                return;

            XpSlider.maxValue = maxXp;
            XpSlider.value = totalXp;
        }

        public void SetLevelText(string actualLevel)
        {
            if (LevelText == null)
            {
                return;
            }

            LevelText.text = actualLevel;

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
                    RedrawInventory();
                }
            }
        }

        private void OnMainMenuClose()
        {
            // This is here to make sure that the menu has the right appearence in the battle when closing
            if (m_currentMode == UIMode.BATTLE)
            {
                SetUIMode(UIMode.BATTLE);
            }
        }

        /// <summary>
        /// WORKAROUND FOR A MISSING INVENTORY IN THE BATTLE
        /// Force manual redraw of the inventory, used in battle
        /// </summary>
        public void RedrawInventory()
        {
            StartCoroutine(DoTheInventoryRedraw());
        }

        private IEnumerator DoTheInventoryRedraw()
        {
            yield return null;
            InventoryPanel.OnOpen();
        }

        /// <summary>
        /// Call this from a close spell button when in battle
        /// </summary>
        public void CloseSpellWindow()
        {
            SetUIMode(UIMode.BATTLE);
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

        public void SetInventory(Inventory inventory)
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

        public void SetCurrencyOwner(CurrencyOwner currencyOwner)
        {
            if (CurrencyMonitors != null)
            {
                foreach (CurrencyOwnerMonitor currMonitor in CurrencyMonitors)
                {
                    currMonitor.SetCurrencyOwner(currencyOwner);
                }
            }
            else
            {
                Debug.LogError("No currency monitor component found on the UI! Please assign one!");
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


        public void ShowGameSavedSuccesfull()
        {
            Debug.Log("Entro al shwogamesusccessfull");
            StartCoroutine(WaitSecondsToFade());


        }


        private IEnumerator WaitSecondsToFade()
        {
            Debug.Log("Entro al enumerator");

            GameSavedSuccess.SetActive(true);
            Animator canvasAnimator = GameSavedSuccess.GetComponent<Animator>();

            canvasAnimator.SetTrigger("FadeIn");

            yield return new WaitForSecondsRealtime(canvasAnimator.GetCurrentAnimatorStateInfo(0).length);

            yield return new WaitForSecondsRealtime(2);

            canvasAnimator.SetTrigger("FadeOut");

            yield return new WaitForSecondsRealtime(canvasAnimator.GetCurrentAnimatorStateInfo(0).length);

            GameSavedSuccess.SetActive(false);

        }

    }
}
