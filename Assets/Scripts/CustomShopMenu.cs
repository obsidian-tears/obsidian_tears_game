/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Menus.Shop
{
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.Exchange;
    using Opsive.UltimateInventorySystem.Exchange.Shops;
    using Opsive.UltimateInventorySystem.Input;
    using Opsive.UltimateInventorySystem.UI.Currency;
    using Opsive.UltimateInventorySystem.UI.Item;
    using Opsive.UltimateInventorySystem.UI.Item.ItemViewModules;
    using Opsive.UltimateInventorySystem.UI.Panels;
    using Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers;
    using Opsive.UltimateInventorySystem.UI.Views;
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;
    using Text = Opsive.Shared.UI.Text;

    /// <summary>
    /// Shop menu, used to display and interact with a shop component
    /// </summary>
    public class CustomShopMenu : ShopMenu
    {
        [Tooltip("The shop component to show in the UI.")]
        [SerializeField] internal ShopBase m_Shop;
        [Tooltip("The inventory grid UI.")]
        [SerializeField] internal InventoryGrid m_InventoryGrid;
        [Tooltip("The currency UI displaying the total price.")]
        [SerializeField] internal MultiCurrencyView m_TotalPrice;
        [Tooltip("The quantity picker panel.")]
        [SerializeField] internal QuantityPickerPanel m_QuantityPickerPanel;
        [Tooltip("The shop id")]
        [SerializeField] protected string m_shopId;

        private ICurrencyOwner<CurrencyCollection> m_ShopperClientCurrencyOwner;
        ReactController reactController;

        /// <summary>
        /// Set up the panel.
        /// </summary>
        public override void Initialize(DisplayPanel display, bool force)
        {
            var wasInitialized = m_IsInitialized;
            if (wasInitialized && !force) { return; }
            base.Initialize(display, force);

            if (wasInitialized == false) {
                //only do it once even if forced.
                if (m_BuyButton != null) {
                    m_BuyButton.onClick.RemoveAllListeners();
                    m_BuyButton.onClick.AddListener(OpenBuy);
                }

                if (m_SellButton != null) {
                    m_SellButton.onClick.RemoveAllListeners();
                    m_SellButton.onClick.AddListener(OpenSell);
                }

                if (m_CloseButton != null) {
                    m_CloseButton.onClick.RemoveAllListeners();
                    m_CloseButton.onClick.AddListener(() => m_DisplayPanel.Close(true));
                }

                m_TempCurrencyCollection = new CurrencyCollection();

                m_InventoryGrid.Initialize(false);

                m_InventoryGrid.Grid.ViewDrawer.AfterDrawing += DrawBuySellPrice;

                m_InventoryGrid.OnItemViewSlotClicked += OnItemClicked;
                m_InventoryGrid.OnItemViewSlotSelected += OnItemSelected;

                m_QuantityPickerPanel.OnAmountChanged += QuantityPickerAmountChanged;
                m_QuantityPickerPanel.ConfirmCancelPanel.OnConfirm += BuySellItem;

                reactController = GameObject.Find("ReactController").GetComponent<ReactController>();
                SetShop(m_Shop);
            }
        }

        /// <summary>
        /// Open the buy sub menu.
        /// </summary>
        private void OpenBuy()
        {
            m_MenuTitle.text = m_BuyTitle;

            m_InventoryGrid.Panel.Open(m_DisplayPanel, m_BuyButton);
            m_IsBuying = true;
            m_InventoryGrid.SetInventory(m_Shop.Inventory);
            m_InventoryGrid.Draw();
            m_InventoryGrid.SelectSlot(0);
        }

        /// <summary>
        /// Open the sell sub menu.
        /// </summary>
        private void OpenSell()
        {
            m_MenuTitle.text = m_SellTitle;

            m_InventoryGrid.Panel.Open(m_DisplayPanel, m_SellButton);
            m_IsBuying = false;
            m_InventoryGrid.SetInventory(m_Inventory);
            m_InventoryGrid.Draw();
            m_InventoryGrid.SelectSlot(0);
        }

        /// <summary>
        /// Buy or sell an item once the player confirmed the quantity. 
        /// </summary>
        protected new virtual void BuySellItem()
        {
            var quantity = m_QuantityPickerPanel.QuantityPicker.Quantity;

            if (quantity < 1) {

                if (m_IsBuying) {
                    m_OnBuyFailed?.Invoke();
                } else {
                    m_OnSellFailed?.Invoke();
                }

                return;
            }

            if (m_IsBuying) {
                var result = m_Shop.BuyItem(m_Inventory, m_ShopperClientCurrencyOwner, (quantity, m_SelectedItemInfo));
                if (result) {
                    reactController.SignalBuyItem(m_Shop.ShopId, m_SelectedItemInfo.Item.ItemDefinitionID.ToString(), quantity);
                    m_OnBuySuccess?.Invoke();
                } else {
                    m_OnBuyFailed?.Invoke();
                }
            } else {
                var result = m_Shop.SellItem(m_Inventory, m_ShopperClientCurrencyOwner, (quantity, m_SelectedItemInfo));
                if (result) {
                    m_OnSellSuccess?.Invoke();
                } else {
                    m_OnSellFailed?.Invoke();
                }
            }

            m_InventoryGrid.Draw();
        }

        /// <summary>
        /// Display the modifier values
        /// </summary>
        private void SetModifierUIs()
        {
            if (m_Shop == null || m_Inventory == null) {
                return;
            }

            var buyModifier = m_Shop.GetBuyModifierForBuyer(m_Inventory);
            var sellModifier = m_Shop.GetSellModifierForSeller(m_Inventory);

            var buyPecDiff = Mathf.RoundToInt((buyModifier - 1) * 100);
            var sellPecDiff = Mathf.RoundToInt((sellModifier - 1) * 100);

            var buySign = buyPecDiff <= 0 ? "" : "+";
            var sellSign = sellPecDiff <= 0 ? "" : "+";

            m_BuyModifierText.text = $"Buy Modifer: {buySign}{buyPecDiff}%";
            m_SellModifierText.text = $"Sell Modifer: {sellSign}{sellPecDiff}%";
        }


    }
}
