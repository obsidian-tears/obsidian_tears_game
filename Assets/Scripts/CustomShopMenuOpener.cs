
/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Menus
{
    using Opsive.UltimateInventorySystem.Core.InventoryCollections;
    using Opsive.UltimateInventorySystem.Exchange.Shops;
    using Opsive.UltimateInventorySystem.UI.Menus.Shop;
    using Opsive.UltimateInventorySystem.UI.Panels;
    using UnityEngine;

    /// <summary>
    /// Shop interactable behavior.
    /// </summary>
    public class CustomShopMenuOpener : InventoryPanelOpener<CustomShopMenu>
    {
        [Tooltip("The shop")]
        [SerializeField] protected ShopBase m_Shop;

        // !!! Start of the code added by Jakub
        protected override void Initialize(bool force) {
            m_Menu = GameManagers.GameUIManager.Instance.ShopMenu;
            if (m_Menu == null)
            {
                Debug.LogError("ERROR! ShopMenu has not been found, please make sure that GameUIManager has been spawned and its Shop Menu property assigned!");
            }

            base.Initialize(force);
        }
        // !!! End of the code added by Jakub

        /// <summary>
        /// Open the menu on for an inventory.
        /// </summary>
        /// <param name="inventory">The inventory.</param>
        public override void Open(Inventory inventory)
        {
            m_Menu.BindInventory(inventory);
            m_Menu.SetShop(m_Shop);
            
            m_Menu.DisplayPanel.SmartOpen();
        }
    }
}