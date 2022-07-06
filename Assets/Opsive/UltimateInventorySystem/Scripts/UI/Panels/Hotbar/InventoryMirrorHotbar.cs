/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Panels.Hotbar
{
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.UI.Item;
    using UnityEngine;

    /// <summary>
    /// A component used to mirror the first few slots of an Inventory Grid.
    /// </summary>
    public class InventoryMirrorHotbar : ItemHotbar
    {
        [Tooltip("The Inventory Grid to Mirror")]
        [SerializeField] protected InventoryGrid m_InventoryGrid;
        [Tooltip("The Inventory Grid index at which to start Mirroring the items")]
        [SerializeField] protected int m_StartIndex = 0;
        [Tooltip("Prevent the items from being moved from the hotbar to the grid")]
        [SerializeField] protected bool m_PreventMoveToGrid;
        [Tooltip("Prevent the items from being moved from the grid to the hotbar")]
        [SerializeField] protected bool m_PreventMoveFromGrid;

        public override void Initialize(bool force)
        {
            base.Initialize(force);

            if (m_InventoryGrid == null) {
                Debug.LogError("The Inventory Mirror Hotbar requires an Inventory Grid to work, please assign one in the Inspector.");
                return;
            }
            
            //Draw the hotbar whenever the Inventory Grid is drawn.
            m_InventoryGrid.OnDraw += Draw;
        }

        /// <summary>
        /// Draw the Item Hotbar and refresh all the views.
        /// </summary>
        protected override void RefreshItemSlotInfos()
        {
            //Draw the Inventory Grid to update the indexes of the item infos
            var itemInfoListSliceInGrid = m_InventoryGrid.FilterAndSortItemInfos();
            
            var slots = m_ItemViewSlots;
            for (int i = 0; i < m_ItemViewSlots.Length; i++) {

                var infoIndex = i + m_StartIndex;
                if (infoIndex < 0 || infoIndex >= itemInfoListSliceInGrid.Count) {
                    slots[i].SetItemInfo(ItemInfo.None);
                    continue;
                }
                
                slots[i].SetItemInfo(itemInfoListSliceInGrid[infoIndex]);
            }
        }

        public override bool CanMoveItem(int sourceIndex, int destinationIndex)
        {
            return m_InventoryGrid.CanMoveItem(sourceIndex-m_StartIndex, SlotIndexToGridIndex(destinationIndex));
        }

        public override void MoveItem(int sourceIndex, int destinationIndex)
        {
            m_InventoryGrid.MoveItem(sourceIndex-m_StartIndex, SlotIndexToGridIndex(destinationIndex));
        }

        public override bool CanGiveItem(ItemInfo itemInfo, int slotIndex)
        {
            if (m_PreventMoveToGrid && ReferenceEquals(itemInfo.Inventory, m_InventoryGrid.Inventory)) {
                return false;
            }
            
            return m_InventoryGrid.CanGiveItem(itemInfo, SlotIndexToGridIndex(slotIndex));
        }

        public override bool CanAddItem(ItemInfo itemInfo, int index)
        {
            if (m_PreventMoveFromGrid && ReferenceEquals(itemInfo.Inventory, m_InventoryGrid.Inventory)) {
                return false;
            }
            return m_InventoryGrid.CanAddItem(itemInfo, SlotIndexToGridIndex(index));
        }

        public override ItemInfo AddItem(ItemInfo itemInfo, int index)
        {
            return m_InventoryGrid.AddItem(itemInfo, index-m_StartIndex);
        }

        public override ItemInfo RemoveItem(ItemInfo itemInfo, int index)
        {
            return m_InventoryGrid.RemoveItem(itemInfo, index-m_StartIndex);
        }

        public override void UnassignItemFromSlots(ItemInfo itemInfo)
        {
            m_InventoryGrid.UnassignItemFromSlots(itemInfo);
        }

        public virtual int SlotIndexToGridIndex(int slotIndex)
        {
            return slotIndex - m_StartIndex;
        }
    }
}