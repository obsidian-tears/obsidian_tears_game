/// ---------------------------------------------
/// Ultimate Inventory System.
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers
{
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Item Category Tab Control is used to organize an InventoryUI panel by ItemCategories.
    /// </summary>
    public class InventoryGridTabControlBinding : ItemViewSlotsContainerInventoryGridBinding
    {
        [Tooltip("The tab control.")]
        [SerializeField] protected internal TabControl m_TabControl;

        [SerializeField] protected bool m_ResetTabIndexOnResetDraw;

        /// <summary>
        /// Handle the Item View Slot Container being bound.
        /// </summary>
        protected override void OnBindItemViewSlotContainer()
        {
            if (m_TabControl == null) {
                Debug.LogError("The tab control is missing from the inventory grid tab control binding", gameObject);
                return;
            }

            m_ItemViewSlotsContainer.OnResetDraw += ResetDraw;
            m_TabControl.OnTabChange += HandleTabChange;
            m_TabControl.Initialize(false);
            m_TabControl.SetTabOn(m_TabControl.TabIndex);

        }

        /// <summary>
        /// Reset the state of the tabs.
        /// </summary>
        protected virtual void ResetDraw()
        {
            if (m_ResetTabIndexOnResetDraw) {
                m_TabControl.SetTabOn(0);
            }
        }

        /// <summary>
        /// Handle the Item view slot container being unbound.
        /// </summary>
        protected override void OnUnbindItemViewSlotContainer()
        {
            m_ItemViewSlotsContainer.OnResetDraw -= ResetDraw;
            m_TabControl.OnTabChange -= HandleTabChange;
        }

        /// <summary>
        /// Handle the tab changing.
        /// </summary>
        /// <param name="previousIndex">The previous tab index.</param>
        /// <param name="newIndex">The new tab index.</param>
        private void HandleTabChange(int previousIndex, int newIndex)
        {
            var inventoryTabData = m_TabControl.CurrentTab?.GetComponent<InventoryTabData>();
            if (inventoryTabData == null) {
                Debug.LogWarning("The selected tab is either null or does not have an InventoryTabData");
                return;
            }

            // Save the indexed items data.
            var previousInventoryGridTabData = m_TabControl.TabToggles[previousIndex]?.GetComponent<InventoryTabData>();
            if (previousInventoryGridTabData != null) {
                previousInventoryGridTabData.Initialize(false);
                previousInventoryGridTabData.Unselected(m_InventoryGrid);
            }

            // Change the tab bindings
            m_InventoryGrid.TabID = m_TabControl.TabIndex;

            inventoryTabData.Initialize(false);
            inventoryTabData.Selected(m_InventoryGrid);

            m_InventoryGrid.Draw();
        }

        /// <summary>
        /// Sort the item indexes.
        /// </summary>
        /// <param name="comparer">The comparer used to sort the item infos.</param>
        public void SortItemIndexes(Comparer<ItemInfo> comparer)
        {
            for (int i = 0; i < m_TabControl.TabCount; i++) {
                var tab = m_TabControl.TabToggles[i];
                var inventoryTab = tab.GetComponent<InventoryTabData>();
                if (inventoryTab != null) {
                    inventoryTab.Indexer.SortItemIndexes(comparer);
                }
            }
        }
    }
}