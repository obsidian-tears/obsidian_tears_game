/// ---------------------------------------------
/// Ultimate Inventory System.
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers
{
    using System;
    using Opsive.Shared.Game;
    using Opsive.UltimateInventorySystem.Core.InventoryCollections;
    using Opsive.UltimateInventorySystem.UI.Grid;
    using Opsive.UltimateInventorySystem.UI.Item;
    using UnityEngine;

    /// <summary>
    /// Tab toggle is used by a TabController to create tabs.
    /// </summary>
    public class InventoryTabData : MonoBehaviour
    {
        public event Action<InventoryGrid> OnSelected;
        public event Action<InventoryGrid> OnUnSelected;
        
        
        [Tooltip("The inventory to use for this tab data (optional).")]
        [SerializeField] protected Inventory m_Inventory;
        [Tooltip("The item info filter.")]
        [SerializeField] protected internal ItemInfoFilterSorterBase m_ItemInfoFilter;
        [Tooltip("Set the max element count on the Grid")]
        [SerializeField] protected bool m_SetMaxElementCount = false;
        [Tooltip("Use the Real Element Count as the Max element count, allowing you to trim the Grid to a dynamic size.")]
        [SerializeField] protected bool m_SetRealElementCountAsMax = true;
        [Tooltip("The max element count used to increase the grid navigation to a specific index (Only used if SetRealElementCountAsMax is false).")]
        [SerializeField] protected int m_MaxElementCount = 100;
        [Tooltip("Set the max element count on the Grid")]
        [SerializeField] protected bool m_SetDisableElementOption = false;
        [Tooltip("Options to disable elements (slots) if they are out of bounds.")]
        [SerializeField] protected GridBase.DisableElementOptions m_DisableElementOption = GridBase.DisableElementOptions.DoNotDisableElements;
        [Tooltip("The index used if the Disable Element Option is set to Custom Disable Index..")]
        [SerializeField] protected int m_CustomDisableElementIndex = -1;
        
        protected InventoryGridIndexer m_Indexer;
        protected bool m_IsInitialized;
        
        public bool SetMaxElementCount { get => m_SetMaxElementCount; set => m_SetMaxElementCount = value; }

        public bool SetRealElementCountAsMax
        {
            get => m_SetRealElementCountAsMax;
            set => m_SetRealElementCountAsMax = value;
        }

        public int MaxElementCount { get => m_MaxElementCount; set => m_MaxElementCount = value; }

        public bool SetDisableElementOption
        {
            get => m_SetDisableElementOption;
            set => m_SetDisableElementOption = value;
        }

        public GridBase.DisableElementOptions DisableElementOption
        {
            get => m_DisableElementOption;
            set => m_DisableElementOption = value;
        }

        public int CustomDisableElementIndex
        {
            get => m_CustomDisableElementIndex;
            set => m_CustomDisableElementIndex = value;
        }

        

        public Inventory Inventory => m_Inventory;
        public ItemInfoFilterSorterBase ItemInfoFilter => m_ItemInfoFilter;

        public InventoryGridIndexer Indexer {
            get => m_Indexer;
            set => m_Indexer = value;
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        private void Awake()
        {
            Initialize(false);
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="force">Force initialize.</param>
        public void Initialize(bool force)
        {
            if (m_IsInitialized && force == false) { return; }

            m_Indexer = new InventoryGridIndexer();

            m_IsInitialized = true;
        }

        public virtual void Selected(InventoryGrid inventoryGrid)
        {
            OnSelected?.Invoke(inventoryGrid);
            
            if (Inventory != null) {
                inventoryGrid.SetInventory(Inventory);
            }

            if (ItemInfoFilter != null) {
                inventoryGrid.BindGridFilterSorter(ItemInfoFilter);
            }

            // Set the indexed items data.
            var inventoryGridIndexData = inventoryGrid.Inventory?.gameObject?.GetCachedComponent<InventoryGridIndexData>();
            if (inventoryGridIndexData != null) {
                inventoryGrid.InventoryGridIndexer.Copy(inventoryGridIndexData.GetGridIndexer(inventoryGrid));
            } else if (Indexer != null) {
                inventoryGrid.InventoryGridIndexer.Copy(Indexer);
            } else {
                inventoryGrid.InventoryGridIndexer.Clear();
            }

            if (m_SetMaxElementCount) {
                inventoryGrid.Grid.SetRealElementCountAsMax = m_SetRealElementCountAsMax;
                inventoryGrid.Grid.MaxElementCount = m_MaxElementCount;
            }
            
            if (m_SetDisableElementOption) {
                inventoryGrid.Grid.DisableElementOption = m_DisableElementOption;
                inventoryGrid.Grid.CustomDisableElementIndex = m_CustomDisableElementIndex;
            }
        }

        public virtual void Unselected(InventoryGrid inventoryGrid)
        {
            OnUnSelected?.Invoke(inventoryGrid);
            
            var previousInventory = inventoryGrid.Inventory;

            var previousInventoryGridIndexData = previousInventory?.gameObject?.GetCachedComponent<InventoryGridIndexData>();
            if (previousInventoryGridIndexData != null) {
                //m_InventoryGrid.InventoryGridIndexer.SetIndexItems(previousInventoryGridIndexData.GetGridIndexData(m_InventoryGrid));
                previousInventoryGridIndexData.SetGridIndexData(inventoryGrid);
            } else {
                Indexer.Copy(inventoryGrid.InventoryGridIndexer);
            }
        }
    }
}