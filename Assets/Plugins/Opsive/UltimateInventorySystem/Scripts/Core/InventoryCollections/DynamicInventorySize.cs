/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Core.InventoryCollections
{
    using System;
    using Opsive.Shared.Utility;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using UnityEngine;

    /// <summary>
    /// This component is used to prevent items from being added to the Inventory when there is no space.
    /// </summary>
    public class DynamicInventorySize : MonoBehaviour, IItemRestriction
    {
        public event Action<ItemInfo> OnSizeChangeOverflow;

        [Serializable]
        public struct SizeChangeOverflowOptions
        {
            public enum RemoveOptions
            {
                RemoveLastItems,
                RemoveFirstItems,
                DoNotRemove
            }

            [Tooltip("Send event")]
            [SerializeField] private RemoveOptions m_RemoveOption;
            [Tooltip("Send event")]
            [SerializeField] private bool m_InvokeRejectedEvent;
            [Tooltip("Send event")]
            [SerializeField] private ItemOverflowAction m_OverflowAction;

            public RemoveOptions RemoveOption { get => m_RemoveOption; set => m_RemoveOption = value; }
            public bool InvokeRejectedEvent { get => m_InvokeRejectedEvent; set => m_InvokeRejectedEvent = value; }
            public ItemOverflowAction OverflowAction { get => m_OverflowAction; set => m_OverflowAction = value; }
        }


        [Tooltip("An ID to differentiate from other DynamicInventorySize components set on the same gameobject.")]
        [SerializeField] protected int m_ID;
        [Tooltip("The Inventory to monitor and restrict")]
        [SerializeField] protected Inventory m_Inventory;
        [Tooltip("The max stack amount.")]
        [SerializeField] protected int m_BaseMaxStackAmount = 99;
        [Tooltip("The itemCollections affected by this restriction.")]
        [SerializeField] protected string[] m_RestrictedItemCollectionNames;
        [Tooltip("Use bage items to increase the size of the item collection group stack size.")]
        [SerializeField] protected bool m_UseBagItems = true;
        [Tooltip("Use bage items to increase the size of the item collection group stack size.")]
        [SerializeField] protected DynamicItemCategory m_BagItemCategory = new DynamicItemCategory("Bag Item");
        [Tooltip("The attribute name for an Integer specifying a size Increase.")]
        [SerializeField] protected string m_BagSizeItemAttributeName = "BagSize";
        [Tooltip("The itemCollections affected by this restriction.")]
        [SerializeField] protected string[] m_BagsItemCollectionNames;
        [Tooltip("Prevent bag item from being removed if it would cause the inventory to overflow?")]
        [SerializeField] protected bool m_PreventRemoveBagItemIfWouldOverflow;
        [Tooltip("What should happen when the dynamic size becomes less then the amount of existing stacks?")]
        [SerializeField] protected SizeChangeOverflowOptions m_SizeChangeOverflowOptions;
       

        [System.NonSerialized] protected ItemCollectionGroup m_RestrictedItemCollections;
        [System.NonSerialized] protected ItemCollectionGroup m_BagsItemCollections;
        [System.NonSerialized] protected bool m_Initialized;

        protected int m_BagsStackAmount;

        public int ID { get => m_ID; set => m_ID = value; }

        public string[] RestrictedItemCollectionNames
        {
            get => m_RestrictedItemCollectionNames;
            set
            {
                m_RestrictedItemCollectionNames = value;
                if (m_RestrictedItemCollections == null) { m_RestrictedItemCollections = new ItemCollectionGroup(); }

                m_RestrictedItemCollections.SetItemCollections(m_Inventory,
                    m_RestrictedItemCollectionNames, true, true);
            }
        }
        
        public bool UseBagItems { get => m_UseBagItems; set => m_UseBagItems = value; }

        public string[] BagsItemCollectionNames
        {
            get => m_BagsItemCollectionNames;
            set
            {
                m_BagsItemCollectionNames = value;
                if (m_BagsItemCollections == null) { m_BagsItemCollections = new ItemCollectionGroup(); }

                m_BagsItemCollections.SetItemCollections(m_Inventory,
                    m_BagsItemCollectionNames, true, true);
            }
        }

        public ItemCategory BagItemCategory { get => m_BagItemCategory; set => m_BagItemCategory = value; }

        public virtual int BaseMaxStackAmount
        {
            get => m_BaseMaxStackAmount;
            set
            {
                var previous = m_BaseMaxStackAmount;
                m_BaseMaxStackAmount = value;
                OnRestrictedSizeAmountChange(previous, m_BaseMaxStackAmount);
            }
        }

        public virtual int BagsStackSize
        {
            get => m_BagsStackAmount;
            set
            {
                var previous = m_BagsStackAmount;
                m_BagsStackAmount = value;
                OnRestrictedSizeAmountChange(previous,m_BagsStackAmount);
            }
        }

        public string BagSizeItemAttributeName
        {
            get => m_BagSizeItemAttributeName;
            set => m_BagSizeItemAttributeName = value;
        }

        public SizeChangeOverflowOptions OverflowOptions
        {
            get => m_SizeChangeOverflowOptions;
            set => m_SizeChangeOverflowOptions = value;
        }

        public virtual int MaxStackAmount => Mathf.Max(0, BaseMaxStackAmount + BagsStackSize);

        public ItemCollectionGroup BagsItemCollections => m_BagsItemCollections;
        public ItemCollectionGroup RestrictedItemCollections => m_RestrictedItemCollections;

        private void Start()
        {
            if (m_Inventory == null) { m_Inventory = GetComponent<Inventory>(); }
            Initialize(m_Inventory, false);
        }

        /// <summary>
        /// Initialize with the Inventory.
        /// </summary>
        /// <param name="inventory">The inventory.</param>
        /// <param name="force">Force Initialization.</param>
        public void Initialize(IInventory inventory, bool force)
        {
            if (m_Initialized && !force && ReferenceEquals(inventory, m_Inventory)) { return; }
            
            if (m_RestrictedItemCollections == null) { m_RestrictedItemCollections = new ItemCollectionGroup(); }

            if (m_BagsItemCollections == null) {
                m_BagsItemCollections = new ItemCollectionGroup();
                m_BagsItemCollections.OnItemAdded += HandleItemInBagCollectionAdded;
                m_BagsItemCollections.OnItemRemoved += HandleItemInBagCollectionRemoved;
            }
            
            m_Inventory = inventory as Inventory;
            
            m_RestrictedItemCollections.SetItemCollections(m_Inventory,
                m_RestrictedItemCollectionNames, true, true);
            m_BagsItemCollections.SetItemCollections(m_Inventory,
                m_BagsItemCollectionNames, true, true);

            RecomputeBagSize();

            m_Initialized = true;
        }

        /// <summary>
        /// An Item was added to the inventory.
        /// </summary>
        /// <param name="originalItemInfo">The original Item Info that was added.</param>
        /// <param name="addedItemStack">The item Stack where the item was added to.</param>
        protected virtual void HandleItemInBagCollectionAdded(ItemInfo originalItemInfo, ItemStack addedItemStack)
        {
            if (m_UseBagItems == false) { return; }
            if (addedItemStack?.Item == null) { return; }
            if(BagItemCategory != null && BagItemCategory.InherentlyContains(addedItemStack.Item) == false){ return; }
            
            if (addedItemStack.Item.TryGetAttributeValue<int>(m_BagSizeItemAttributeName, out var bagSize) == false) {
                return;
            }

            var previousMaxAmount = MaxStackAmount;
            m_BagsStackAmount += bagSize;
            m_BagsStackAmount = Mathf.Max(0, m_BagsStackAmount);
            OnRestrictedSizeAmountChange(previousMaxAmount, MaxStackAmount);
        }
        
        /// <summary>
        /// An item was removed from the bag collection (it is not necessarily a bag item).
        /// </summary>
        /// <param name="itemRemoved">The item that was removed.</param>
        /// <param name="itemStack">The item Stack where the item was added to.</param>
        protected virtual void HandleItemInBagCollectionRemoved(ItemInfo itemRemoved)
        {
            if (m_UseBagItems == false) { return; }
            if (itemRemoved.Item == null) { return; }
            if(BagItemCategory != null && BagItemCategory.InherentlyContains(itemRemoved.Item) == false){ return; }

            if (itemRemoved.Item.TryGetAttributeValue<int>(m_BagSizeItemAttributeName, out var bagSize) == false) {
                return;
            }

            var previousMaxAmount = MaxStackAmount;
            m_BagsStackAmount -= bagSize;
            m_BagsStackAmount = Mathf.Max(0, m_BagsStackAmount);
            OnRestrictedSizeAmountChange(previousMaxAmount, MaxStackAmount);
        }

        public virtual void RecomputeBagSize()
        {
            var previousMaxAmount = MaxStackAmount;
            m_BagsStackAmount = 0;

            for (int i = 0; i < m_BagsItemCollections.ItemCollections.Count; i++) {
                var itemCollection = m_BagsItemCollections.ItemCollections[i];
                if(itemCollection == null){ continue; }

                var itemStacks = itemCollection.GetAllItemStacks();
                for (int j = 0; j < itemStacks.Count; j++) {
                    var itemStack = itemStacks[j];
                    if(itemStack?.Item == null){ continue; }
                    
                    if(BagItemCategory != null && BagItemCategory.InherentlyContains(itemStack.Item) == false){ return; }

                    if (itemStack.Item.TryGetAttributeValue<int>(m_BagSizeItemAttributeName, out var bagSize) == false) {
                        return;
                    }

                    m_BagsStackAmount += bagSize;
                }
            }

            OnRestrictedSizeAmountChange(previousMaxAmount, MaxStackAmount);
        }

        /// <summary>
        /// Get current Item Stack Amount.
        /// </summary>
        /// <returns>The number of item stacks.</returns>
        public int GetCurrentStackAmount()
        {
            var count = 0;
            for (int i = 0; i < m_RestrictedItemCollections.ItemCollections.Count; i++) {
                count += m_RestrictedItemCollections.ItemCollections[i].ItemStacks.Count;
            }

            return count;
        }

        /// <summary>
        /// Can the Item be added to the item collection?
        /// </summary>
        /// <param name="itemInfo">The item to add.</param>
        /// <param name="receivingCollection">The item collection the item is added to.</param>
        /// <returns>The item that can be added.</returns>
        public ItemInfo? CanAddItem(ItemInfo itemInfo, ItemCollection receivingCollection)
        {
            if (m_RestrictedItemCollections.Contains(receivingCollection) == false) { return itemInfo; }

            var count = GetCurrentStackAmount();

            var availableAdditionalStacks = MaxStackAmount - count;

            var itemAmountsThatFit = receivingCollection.GetItemAmountFittingInLimitedAdditionalStacks(itemInfo,
                availableAdditionalStacks);

            if (itemAmountsThatFit == 0) { return null; }

            return (itemAmountsThatFit, itemInfo);
        }

        /// <summary>
        /// Can the Item be removed.
        /// </summary>
        /// <param name="itemInfo">The item info.</param>
        /// <returns>The item Info.</returns>
        public ItemInfo? CanRemoveItem(ItemInfo itemInfo)
        {
            if (m_PreventRemoveBagItemIfWouldOverflow == false) {
                return itemInfo;
            }

            if (itemInfo.Item == null) {
                return itemInfo;
            }

            // Check if the item is a bag item.
            if (m_BagsItemCollections.Contains(itemInfo.ItemCollection) == false) {
                return itemInfo;
            }

            if (BagItemCategory != null && BagItemCategory.InherentlyContains(itemInfo.Item) == false) {
                return itemInfo;
            }

            if (itemInfo.Item.TryGetAttributeValue<int>(m_BagSizeItemAttributeName, out var bagSize) == false) {
                return itemInfo;
            }
            
            // The item is a bag item.
            var currentStackAmount = GetCurrentStackAmount();

            // The item should not be removed.
            if (MaxStackAmount - currentStackAmount - bagSize < 0) {
                return ItemInfo.None;
            }
            
            return itemInfo;
        }

        /// <summary>
        /// The restricted size amount changed. What to do if the collection has more items than it should?
        /// </summary>
        /// <param name="previousAmount">The previous restriction amount.</param>
        /// <param name="newAmount">The new restriction amount</param>
        public virtual void OnRestrictedSizeAmountChange(int previousAmount, int newAmount)
        {
            Opsive.Shared.Events.EventHandler.ExecuteEvent<int, int, int>(m_Inventory,
                EventNames.c_Inventory_OnDynamicInventorySizeChange_ID_PreviousSizeInt_NewSizeInt,
                m_ID, previousAmount, newAmount);
            
            if (previousAmount <= newAmount) {
                //Do nothing.
                return;
            }

            var currentStackAmount = GetCurrentStackAmount();

            if (currentStackAmount <= newAmount) {
                //Do nothing.
                return;
            }

            var stackAmountToRemove = currentStackAmount - newAmount;

            var stackRemovedCount = 0;
            switch (m_SizeChangeOverflowOptions.RemoveOption) {
                case SizeChangeOverflowOptions.RemoveOptions.DoNotRemove:
                    //Do nothing.
                    return;
                case SizeChangeOverflowOptions.RemoveOptions.RemoveLastItems:
                {
                    var itemInfos = m_RestrictedItemCollections.GetAllItemInfos();
                    for (int i = itemInfos.Count - 1; i >= 0; i--) {
                        var removedItem = m_RestrictedItemCollections.RemoveItem(itemInfos[i]);
                        if (removedItem.Amount == 0) { continue; }

                        stackRemovedCount++;
                        HandleItemRemovedDueToSizeChangeOverflow(removedItem);

                        if (stackRemovedCount >= stackAmountToRemove) {
                            break;
                        }
                    }
                    return;
                }

                case SizeChangeOverflowOptions.RemoveOptions.RemoveFirstItems:
                {
                    var itemInfos = m_RestrictedItemCollections.GetAllItemInfos();
                    for (int i = 0; i >= itemInfos.Count; i++) {
                        var removedItem = m_RestrictedItemCollections.RemoveItem(itemInfos[i]);
                        if (removedItem.Amount == 0) { continue; }

                        stackRemovedCount++;
                        HandleItemRemovedDueToSizeChangeOverflow(removedItem);
                        
                        if (stackRemovedCount >= stackAmountToRemove) {
                            break;
                        }
                    }

                    return;
                }
                default:
                    //Do nothing.
                    return;
            }
        }

        /// <summary>
        /// The item was removed because it was overflowing when the max stack amount was changed.
        /// </summary>
        /// <param name="removedItem">The removedItem.</param>
        private void HandleItemRemovedDueToSizeChangeOverflow(ItemInfo removedItem)
        {
            if (m_SizeChangeOverflowOptions.OverflowAction != null) {
                m_SizeChangeOverflowOptions.OverflowAction.HandleItemOverflow(m_Inventory, removedItem, ItemInfo.None, removedItem);
            }

            if (m_SizeChangeOverflowOptions.InvokeRejectedEvent) {
                Opsive.Shared.Events.EventHandler.ExecuteEvent<ItemInfo>(m_Inventory,
                    EventNames.c_Inventory_OnDynamicInventorySizeChangeOverflow_ItemInfoRemoved,
                    removedItem);
                
                OnSizeChangeOverflow?.Invoke(removedItem);
            }
        }
    }
}