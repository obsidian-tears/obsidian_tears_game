namespace Opsive.UltimateInventorySystem.UI.Item
{
    using System;
    using System.Collections.Generic;
    using Opsive.UltimateInventorySystem.Core.InventoryCollections;
    using Opsive.UltimateInventorySystem.UI.Grid;
    using Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers;
    using UnityEngine;

    /// <summary>
    /// This component is used next to the Inventory Grid component.
    /// It allows you to change the grid max element count matching it to a DynamicInventorySize max stack amount.
    /// </summary>
    public class DynamicInventorySizeInventoryGridBinding : ItemViewSlotsContainerInventoryGridBinding
    {
        [Tooltip("The Dynamic Inventory Size ID used to identify how to limit the Inventory Grid.")]
        [SerializeField] protected int m_DynamicInventorySizeID;
        [Tooltip("Use the Tab Index as the Dyncamic Inventory Size ID? Allowing each tab to have a different limit.")]
        [SerializeField] protected bool m_UseTabIDAsSizeID;
        [Tooltip("A text component displaying the formatted string.")]
        [SerializeField] protected Shared.UI.Text m_DisplayAmount;
        [Tooltip("The format in which to display the restriction. {0}: CurrentAmount, {1}: MaxAmount.")]
        [SerializeField] protected string m_DisplayAmountFormat = "{0}/{1}";
        [Tooltip("Set the max element count on the Grid")]
        [SerializeField] protected bool m_SetMaxElementCount = true;
        [Tooltip("Set the max element count on the Grid")]
        [SerializeField] protected bool m_SetDisableElementOption;
        [Tooltip("Options to disable elements (slots) if they are out of bounds.")]
        [SerializeField] protected GridBase.DisableElementOptions m_DisableElementOption = GridBase.DisableElementOptions.DoNotDisableElements;
        [Tooltip("The index used if the Disable Element Option is set to Custom Disable Index..")]
        [SerializeField] protected int m_CustomDisableElementIndex = -1;
        
        public int DynamicInventorySizeID { get => m_DynamicInventorySizeID; set => m_DynamicInventorySizeID = value; }
        public bool UseTabIDAsSizeID { get => m_UseTabIDAsSizeID; set => m_UseTabIDAsSizeID = value; }

        protected Dictionary<int, DynamicInventorySize> m_InventorySizes;

        protected bool m_PreviousSetRealElementCountAsMax;
        protected int m_PreviousMaxElementCount;
        protected GridBase.DisableElementOptions m_PreviousDisableElementOptions;
        protected int m_PreviousCustomDisableElementIndex;

        protected bool m_PreviouslyDrewWithDynamicSizeInventory;

        protected override void OnBindItemViewSlotContainer()
        {
            var grid = m_InventoryGrid.Grid;
            grid.OnBeforeDraw += HandleBeforeDraw;
            m_InventoryGrid.OnBindInventory += OnBindInventory;
            m_InventoryGrid.OnUnBindInventory += OnUnBindInventory;

            m_PreviousSetRealElementCountAsMax = grid.SetRealElementCountAsMax;
            m_PreviousMaxElementCount = grid.MaxElementCount;
            m_PreviousDisableElementOptions = grid.DisableElementOption;
            m_PreviousCustomDisableElementIndex = grid.CustomDisableElementIndex;

            OnBindInventory(m_InventoryGrid.Inventory);
        }

        protected override void OnUnbindItemViewSlotContainer()
        {
            m_InventoryGrid.Grid.OnBeforeDraw -= HandleBeforeDraw;
            m_InventoryGrid.OnBindInventory -= OnBindInventory;
            m_InventoryGrid.OnUnBindInventory -= OnUnBindInventory;
            
            OnUnBindInventory(m_InventoryGrid.Inventory);
        }

        private void OnBindInventory(Inventory inventory)
        {
            if (m_InventorySizes == null) {
                m_InventorySizes = new Dictionary<int, DynamicInventorySize>();
            } else {
                m_InventorySizes.Clear();
            }

            if (inventory == null) {
                return;
            }

            var dynamicInventorySizes = inventory.GetComponents<DynamicInventorySize>();
            for (int i = 0; i < dynamicInventorySizes.Length; i++) {
                var dynamicInventorySize = dynamicInventorySizes[i];

                if (m_InventorySizes.ContainsKey(dynamicInventorySize.ID)) {
                    Debug.LogWarning($"There are multiple DynamicInventorySize components on the Inventory '{inventory}' with the same ID '{dynamicInventorySize.ID}'",inventory);
                }
                
                m_InventorySizes[dynamicInventorySize.ID] = dynamicInventorySize;
            }

        }

        private void OnUnBindInventory(Inventory inventory)
        {
            Debug.Log("UnBind Inventory "+inventory);
            m_InventorySizes.Clear();
        }

        /// <summary>
        /// Set the limits using the DynamicInventorySize components on the Inventory
        /// </summary>
        protected virtual void HandleBeforeDraw()
        {
            var id = UseTabIDAsSizeID ? m_InventoryGrid.TabID : m_DynamicInventorySizeID;
            if (m_InventorySizes.TryGetValue(id, out var dynamicInventorySize) == false) {
                // Dynamic Inventory Size not found.
                BeforeDrawWithNoDynamicInventorySize();
                return;
            }
            
            BeforeDrawWithDynamicInventorySize(dynamicInventorySize);
        }

        protected virtual void BeforeDrawWithNoDynamicInventorySize()
        {
            if (m_PreviouslyDrewWithDynamicSizeInventory == false) {
                return;
            }
            
            var grid = m_InventoryGrid.Grid;
            if (m_SetMaxElementCount) {
                grid.SetRealElementCountAsMax = m_PreviousSetRealElementCountAsMax;
                grid.MaxElementCount = m_PreviousMaxElementCount;
            }

            if (m_SetDisableElementOption) {
                grid.DisableElementOption = m_PreviousDisableElementOptions;
                grid.CustomDisableElementIndex = m_PreviousCustomDisableElementIndex;
            }
            
            m_DisplayAmount.text = "";

            m_PreviouslyDrewWithDynamicSizeInventory = false;
        }
        
        protected virtual void BeforeDrawWithDynamicInventorySize(DynamicInventorySize dynamicInventorySize)
        {
            var grid = m_InventoryGrid.Grid;
            
            // Keep track of the previous values.
            if (m_PreviouslyDrewWithDynamicSizeInventory == false) {
                m_PreviousSetRealElementCountAsMax = grid.SetRealElementCountAsMax;
                m_PreviousMaxElementCount = grid.MaxElementCount;
                m_PreviousDisableElementOptions = grid.DisableElementOption;
                m_PreviousCustomDisableElementIndex = grid.CustomDisableElementIndex;
            }
            
            
            if (m_SetMaxElementCount) {
                grid.SetRealElementCountAsMax = false;
                grid.MaxElementCount = dynamicInventorySize.MaxStackAmount;
            }
            
            if (m_SetDisableElementOption) {
                grid.DisableElementOption = m_DisableElementOption;
                grid.CustomDisableElementIndex = m_CustomDisableElementIndex;
            }

            var maxStackAmount = dynamicInventorySize.MaxStackAmount;
            var currentStackAMount = dynamicInventorySize.GetCurrentStackAmount();
            
            m_DisplayAmount.text = string.Format(m_DisplayAmountFormat, currentStackAMount, maxStackAmount);


            m_PreviouslyDrewWithDynamicSizeInventory = true;
        }
        
    }
}