/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Item.DragAndDrop.DropActions
{
    using Opsive.UltimateInventorySystem.UI.Grid;
    using Opsive.UltimateInventorySystem.UI.Panels.Hotbar;
    using System;
    using UnityEngine;

    /// <summary>
    /// The Item View Drop Container Exchange Action.
    /// </summary>
    [Serializable]
    public class ItemViewDropContainerSmartExchangeAction_ClassCheck : ItemViewDropAction
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ItemViewDropContainerSmartExchangeAction_ClassCheck()
        {
        }

        /// <summary>
        /// Drop Action.
        /// </summary>
        /// <param name="itemViewDropHandler">The Item View Drop Handler.</param>
        public override void Drop(ItemViewDropHandler itemViewDropHandler)
        {
            Debug.Log("Dropping");

            var itemSlotSet = itemViewDropHandler.gameObject.GetComponent<ItemSlotCollectionView>().ItemSlotSet;

            var canEquip = false;
            foreach (var item in itemSlotSet.ItemSlots)
            {
                if (!item.Category.InherentlyContains(itemViewDropHandler.SourceItemInfo.Item))
                    continue;
                else
                    canEquip = true;
            }

            if (!canEquip)
            {
                string characterClass;
                itemViewDropHandler.SourceItemInfo.Item.TryGetAttributeValue("Class", out characterClass);
                CantEquipItemNote.Instance.ShowNote(characterClass);
                return;
            }

            var sourceCanGive = itemViewDropHandler.SourceContainer.CanGiveItem(
        itemViewDropHandler.SourceItemInfo,
        itemViewDropHandler.SourceIndex);

            var destinationCanGive = itemViewDropHandler.DestinationContainer.CanGiveItem(
                itemViewDropHandler.DestinationItemInfo,
                itemViewDropHandler.DestinationIndex); ;

            var sourceCanAdd = itemViewDropHandler.SourceContainer.CanAddItem(
                itemViewDropHandler.StreamData.DestinationItemInfo,
                itemViewDropHandler.SourceIndex);

            var destinationCanAdd = itemViewDropHandler.DestinationContainer.CanAddItem(
                itemViewDropHandler.StreamData.SourceItemInfo,
                itemViewDropHandler.DestinationIndex);

            var sourceIsNull = itemViewDropHandler.SourceItemInfo.Item == null;

            var destinationIsNull = itemViewDropHandler.DestinationItemInfo.Item == null;

            var sourceGiveDestinationReceive = sourceIsNull == false && (sourceCanGive && destinationCanAdd);
            var destinationGiveSourceReceive = destinationIsNull == false && (destinationCanGive && sourceCanAdd);

            if (itemViewDropHandler.SourceContainer.Inventory == itemViewDropHandler.DestinationContainer.Inventory)
            {
                //Make an exception for Inventory Mirror Hotbar as they are a direct copy of the Inventory
                if (itemViewDropHandler.DestinationContainer is InventoryMirrorHotbar destinationMirrorHotbar &&
                    itemViewDropHandler.SourceContainer is InventoryGrid sourceInventoryGrid)
                {

                    sourceInventoryGrid.MoveItem(itemViewDropHandler.SourceIndex, destinationMirrorHotbar.SlotIndexToGridIndex(itemViewDropHandler.DestinationIndex));
                    return;
                }
                if (itemViewDropHandler.SourceContainer is InventoryMirrorHotbar sourceMirrorHotbar &&
                    itemViewDropHandler.DestinationContainer is InventoryGrid destinationInventoryGrid)
                {

                    destinationInventoryGrid.MoveItem(sourceMirrorHotbar.SlotIndexToGridIndex(itemViewDropHandler.SourceIndex), itemViewDropHandler.DestinationIndex);
                    return;
                }
            }

            if (sourceGiveDestinationReceive)
            {
                // Make an exception for Item Hotbars as they look for items within the Inventory.
                if (!(itemViewDropHandler.DestinationContainer is ItemHotbar) || itemViewDropHandler.DestinationContainer is InventoryMirrorHotbar)
                {
                    itemViewDropHandler.StreamData.SourceItemInfo = itemViewDropHandler.SourceContainer.RemoveItem(itemViewDropHandler.StreamData.SourceItemInfo, itemViewDropHandler.SourceIndex);
                }
            }

            if (destinationGiveSourceReceive)
            {
                itemViewDropHandler.StreamData.DestinationItemInfo = itemViewDropHandler.DestinationContainer.RemoveItem(itemViewDropHandler.StreamData.DestinationItemInfo, itemViewDropHandler.DestinationIndex);
            }

            if (sourceGiveDestinationReceive)
            {
                var addToDestination = false;

                // Make an exception for Item Hotbars as they look for items within the Inventory.
                Debug.Log("drop handler name: " + itemViewDropHandler.SlotCursorManager.SourceItemViewSlot.gameObject.name);
                if (!(itemViewDropHandler.SourceContainer is ItemHotbar itemHotbar) || itemViewDropHandler.SourceContainer is InventoryMirrorHotbar)
                {
                    addToDestination = true;
                }
                else
                {
                    if (itemHotbar.Inventory != itemViewDropHandler.DestinationContainer.Inventory)
                    {
                        addToDestination = true;
                    }
                }

                if (addToDestination)
                {
                    itemViewDropHandler.DestinationContainer.AddItem(itemViewDropHandler.StreamData.SourceItemInfo, itemViewDropHandler.DestinationIndex);
                }
            }

            if (destinationGiveSourceReceive)
            {
                itemViewDropHandler.SourceContainer.AddItem(itemViewDropHandler.StreamData.DestinationItemInfo, itemViewDropHandler.SourceIndex);
            }
        }
    }
}