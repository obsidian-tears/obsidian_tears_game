/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers
{
    using System;
    using Opsive.Shared.Game;
    using Opsive.UltimateInventorySystem.UI.Grid;
    using Opsive.UltimateInventorySystem.UI.Item;
    using Opsive.UltimateInventorySystem.UI.Item.ItemViewModules;
    using UnityEngine;
    using UnityEngine.Serialization;

    /// <summary>
    /// Set the panel position depending on the selected item view slot. 
    /// </summary>
    public class ItemViewSlotPanelToTooltip : DisplayPanelBinding, IItemViewSlotContainerBinding
    {
        [Serializable]
        public struct OutOfBoundsOffset
        {
            [Tooltip("The anchor position of the panel to place (Only used if the SetAnchorPosition is true).")]
            [SerializeField] private float m_AnchorPosition;
            [Tooltip("The offset compared to the Item View anchor, (0|0) is the view center. (0.5|0.5) is top right.")]
            [SerializeField] private float m_AnchorRelativeOffset;
            [Tooltip("A fixed offset added at to the relative offset, scales with the canvas scaler.")]
            [SerializeField] private float m_PixelFixedOffset;
            [Tooltip("Add to the fixed offset, this multiplier scales the amount by which the panel goes out of bounds.")]
            [SerializeField] private float m_OutOfBoundOffsetMultiplier;

            public OutOfBoundsOffset(float anchorPosition, float anchorRelativeOffset, float pixelFixedOffset, float outOfBoundOffsetMultiplier)
            {
                m_AnchorPosition = anchorPosition;
                m_AnchorRelativeOffset = anchorRelativeOffset;
                m_PixelFixedOffset = pixelFixedOffset;
                m_OutOfBoundOffsetMultiplier = outOfBoundOffsetMultiplier;
            }

            public float AnchorPosition => m_AnchorPosition;
            public float AnchorRelativeOffset => m_AnchorRelativeOffset;
            public float PixelFixedOffset => m_PixelFixedOffset;
            public float OutOfBoundOffsetMultiplier => m_OutOfBoundOffsetMultiplier;
        }
        
        [Tooltip("The transform to place next to the Item View selected/clicked.")]
        [SerializeField] protected RectTransform m_PanelToPlace;
        [Tooltip("The inventory grid to monitor.")]
        [SerializeField] internal ItemViewSlotsContainerBase m_ItemViewSlotContainer;
        [Tooltip("Set the anchor position of the panel each time it is placed.")]
        [SerializeField] protected bool m_SetAnchorPosition = true;
        [Tooltip("The anchor position of the panel to place (Only used if the SetAnchorPosition is true).")]
        [SerializeField] protected Vector2 m_AnchorPosition = new Vector2(0, 0.5f);
        [FormerlySerializedAs("m_AnchorOffset")]
        [Tooltip("The offset compared to the Item View anchor, (0|0) is the view center. (0.5|0.5) is top right.")]
        [SerializeField] protected Vector2 m_AnchorRelativeOffset = new Vector2(0.5f, 0);
        [Tooltip("A fixed offset added at to the relative offset, scales with the canvas scaler.")]
        [SerializeField] protected Vector2 m_PixelFixedOffset = new Vector2(0, 0);
        [Tooltip("move the panel so that it fits inside the panel bounds (keep null if the panel is unbound).")]
        [SerializeField] protected RectTransform m_PanelBounds;
        [Tooltip("Visualize how much the panel is out of bounds")]
        [SerializeField] protected Vector2 m_OutOfBounds;
        [Tooltip("Override the X values when the panel is out of bounds.")]
        [SerializeField] protected OutOfBoundsOffset m_LeftOutOfBoundsOffset;
        [Tooltip("Override the X values when the panel is out of bounds.")]
        [SerializeField] protected OutOfBoundsOffset m_RightOutOfBoundsOffset;
        [Tooltip("Override the X values when the panel is out of bounds.")]
        [SerializeField] protected OutOfBoundsOffset m_TopOutOfBoundsOffset;
        [Tooltip("Override the X values when the panel is out of bounds.")]
        [SerializeField] protected OutOfBoundsOffset m_BottomOutOfBoundsOffset;
        [Tooltip("Place the panel next to the box when clicked.")]
        [SerializeField] internal bool m_PlaceOnClick;
        [Tooltip("Activate/Deactivate the panel when clicked.")]
        [SerializeField] internal bool m_ShowOnClick;
        [Tooltip("Place the panel next to the box when selected.")]
        [SerializeField] internal bool m_PlaceOnSelect = true;
        [Tooltip("Activate/Deactivate the panel when selected.")]
        [SerializeField] internal bool m_ShowOnSelect = true;
        [Tooltip("Hide on deselect.")]
        [SerializeField] internal bool m_HideShowOnDeselect = true;

        protected Vector3[] m_BoundCorners;
        protected Vector3[] m_PanelCorners;
        protected Canvas m_Canvas;
        protected bool m_TooltipInitialize;
        protected bool m_IsItemViewSlotContainerBound;

        public bool IsItemViewSlotContainerBound => m_IsItemViewSlotContainerBound;
        public ItemViewSlotsContainerBase ItemViewSlotsContainer => m_ItemViewSlotContainer;

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="display">The display panel.</param>
        /// <param name="force">Force Initialize.</param>
        public override void Initialize(DisplayPanel display, bool force)
        {
            var wasInitialized = m_IsInitialized;
            if (wasInitialized && !force) { return; }
            base.Initialize(display, force);

            Initialize(force);
        }

        /// <summary>
        /// Listen to the grid events.
        /// </summary>
        private void Awake()
        {
            Initialize(false);
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="force">Force initialize.</param>
        private void Initialize(bool force)
        {
            if (m_TooltipInitialize && !force) { return; }

            if (m_PanelToPlace == null) {
                m_PanelToPlace = transform as RectTransform;
            }
            
            m_BoundCorners = new Vector3[4];
            m_PanelCorners = new Vector3[4];

            if (m_Canvas == null) {
                m_Canvas = GetComponentInParent<Canvas>();
                // The canvas can still be null if the component is disabled.
                if (m_Canvas == null) {
                    Transform ancestorSearch = transform;
                    while (m_Canvas == null && ancestorSearch.parent != null)
                    {
                        m_Canvas = ancestorSearch.parent.GetComponent<Canvas>();
                        if (m_Canvas != null) break;
                        ancestorSearch = ancestorSearch.parent;
                    }
                }
            }

            if (m_ItemViewSlotContainer == null) {
                Debug.LogError("An Item View Slot Container is missing on the panel placer.", gameObject);
            }

            BindItemViewSlotContainer();

            m_TooltipInitialize = true;
        }

        /// <summary>
        /// Bind the item view slot container.
        /// </summary>
        public virtual void BindItemViewSlotContainer()
        {
            BindItemViewSlotContainer(m_ItemViewSlotContainer);
        }

        /// <summary>
        /// Bind the item view slot container.
        /// </summary>
        /// <param name="itemViewSlotsContainer">The item view slot container to bind.</param>
        public virtual void BindItemViewSlotContainer(ItemViewSlotsContainerBase itemViewSlotsContainer)
        {
            UnbindItemViewSlotContainer();

            m_ItemViewSlotContainer = itemViewSlotsContainer;

            if (m_ItemViewSlotContainer == null) {
                m_IsItemViewSlotContainerBound = false;
                return;
            }

            m_ItemViewSlotContainer.OnItemViewSlotClicked += OnItemClicked;
            m_ItemViewSlotContainer.OnItemViewSlotDeselected += OnItemDeselected;
            m_ItemViewSlotContainer.OnItemViewSlotSelected += OnItemSelected;

            m_IsItemViewSlotContainerBound = true;
        }

        /// <summary>
        /// Unbind the item view slot container.
        /// </summary>
        public virtual void UnbindItemViewSlotContainer()
        {
            if (!m_IsItemViewSlotContainerBound) { return; }

            m_ItemViewSlotContainer.OnItemViewSlotClicked -= OnItemClicked;
            m_ItemViewSlotContainer.OnItemViewSlotDeselected -= OnItemDeselected;
            m_ItemViewSlotContainer.OnItemViewSlotSelected -= OnItemSelected;

            m_IsItemViewSlotContainerBound = false;
        }

        /// <summary>
        /// Handle the item deselected event.
        /// </summary>
        /// <param name="slotEventData">The slot event data.</param>
        private void OnItemDeselected(ItemViewSlotEventData slotEventData)
        {
            if (m_HideShowOnDeselect) { m_PanelToPlace.gameObject.SetActive(false); }
        }

        /// <summary>
        /// Get the rect transform that will be used as a relative anchor for that Item View Slot.
        /// </summary>
        /// <param name="slotEventData">The item view slot data.</param>
        /// <returns>The relative Item View Slot rect transform.</returns>
        protected virtual RectTransform GetItemViewSlotRelativeRectTransform(ItemViewSlotEventData slotEventData)
        {
            if (!(slotEventData.ItemViewSlotsContainer is ItemShapeGrid)) {
                return slotEventData.ItemViewSlot.transform as RectTransform;
            }
            
            ItemShapeItemView backgroundItemShapeView = slotEventData.ItemView.gameObject.GetCachedComponent<ItemShapeItemView>();
            if (backgroundItemShapeView.ForegroundItemView == null) {
                return backgroundItemShapeView.ShapeResizableContent;
            }
            return backgroundItemShapeView.ForegroundItemView.ShapeResizableContent;
        }

        /// <summary>
        /// An item was clicked.
        /// </summary>
        /// <param name="itemInfo">The item info.</param>
        /// <param name="boxIndex">The box index.</param>
        private void OnItemClicked(ItemViewSlotEventData slotEventData)
        {
            if (m_ShowOnClick == false) { return; }

            var itemInfo = slotEventData.ItemViewSlot.ItemInfo;
            var show = m_ShowOnClick && itemInfo.Item != null;

            m_PanelToPlace.gameObject.SetActive(show);

            if (show == false) { return; }

            if (m_PlaceOnClick) {
                var rectTransform = GetItemViewSlotRelativeRectTransform(slotEventData);
                PlacePanel(rectTransform);
            }
        }

        /// <summary>
        /// The item was selected.
        /// </summary>
        /// <param name="itemInfo">The item info.</param>
        /// <param name="boxIndex">The box index.</param>
        private void OnItemSelected(ItemViewSlotEventData slotEventData)
        {
            if (m_ShowOnSelect == false) { return; }

            var itemInfo = slotEventData.ItemViewSlot.ItemInfo;
            var show = m_ShowOnSelect && itemInfo.Item != null;

            m_PanelToPlace.gameObject.SetActive(show);

            if (show == false) { return; }

            if (m_PlaceOnSelect) {
                var rectTransform = GetItemViewSlotRelativeRectTransform(slotEventData);
                PlacePanel(rectTransform);
            }
        }

        /// <summary>
        /// Place the panel next to an Item View.
        /// </summary>
        /// <param name="slotRectTransform">The rect transform.</param>
        private void PlacePanel(RectTransform slotRectTransform)
        {
            var newAnchor = m_SetAnchorPosition ? m_AnchorPosition : m_PanelToPlace.pivot;
            var newPosition = slotRectTransform.position;

            var positionOffset = new Vector2(
                slotRectTransform.sizeDelta.x * m_AnchorRelativeOffset.x,
                slotRectTransform.sizeDelta.y * m_AnchorRelativeOffset.y);

            positionOffset = (positionOffset + m_PixelFixedOffset) * m_Canvas.scaleFactor;
            
            if (m_SetAnchorPosition) {
                m_PanelToPlace.anchorMax = newAnchor;
                m_PanelToPlace.anchorMin = newAnchor;
                m_PanelToPlace.pivot = newAnchor;
            }
            
            // Set the new position
            newPosition = new Vector3(
                newPosition.x + positionOffset.x,
                newPosition.y + positionOffset.y,
                newPosition.z);

            m_PanelToPlace.position = newPosition;

            AdjustPositionIfOutOfBounds(slotRectTransform);
        }

        protected virtual void AdjustPositionIfOutOfBounds(RectTransform slotRectTransform)
        {
            if (m_PanelBounds == null) { return; }
            
            if (m_BoundCorners == null || m_BoundCorners.Length != 4) { m_BoundCorners = new Vector3[4]; }

            if (m_PanelCorners == null || m_PanelCorners.Length != 4) { m_PanelCorners = new Vector3[4]; }

            m_PanelBounds.ForceUpdateRectTransforms();
            m_PanelToPlace.ForceUpdateRectTransforms();
            
            //Get the world corners bottom-left -> top-left -> top-right -> bottom-right.
            m_PanelBounds.GetWorldCorners(m_BoundCorners);
            m_PanelToPlace.GetWorldCorners(m_PanelCorners);

            var posRight = m_PanelCorners[2].x;
            var posLeft = m_PanelCorners[0].x;
            var posTop = m_PanelCorners[2].y;
            var postBot = m_PanelCorners[0].y;

            var anchorPosition = m_AnchorPosition;
            var anchorRelativeOffset = m_AnchorRelativeOffset;
            var pixelFixedOffset = m_PixelFixedOffset;

            m_OutOfBounds = Vector2.zero;

            if (posRight > m_BoundCorners[2].x) {
                //Out of bounds Right
                m_OutOfBounds.x = posRight - m_BoundCorners[2].x;

                anchorPosition.x = m_RightOutOfBoundsOffset.AnchorPosition;
                anchorRelativeOffset.x = m_RightOutOfBoundsOffset.AnchorRelativeOffset;
                pixelFixedOffset.x = m_RightOutOfBoundsOffset.PixelFixedOffset;
                pixelFixedOffset.x += m_RightOutOfBoundsOffset.OutOfBoundOffsetMultiplier * m_OutOfBounds.x;

            } else if (posLeft < m_BoundCorners[0].x) {
                //Out of bounds Left
                m_OutOfBounds.x = posLeft - m_BoundCorners[0].x;
                
                anchorPosition.x = m_LeftOutOfBoundsOffset.AnchorPosition;
                anchorRelativeOffset.x = m_LeftOutOfBoundsOffset.AnchorRelativeOffset;
                pixelFixedOffset.x = m_LeftOutOfBoundsOffset.PixelFixedOffset;
                pixelFixedOffset.x += m_LeftOutOfBoundsOffset.OutOfBoundOffsetMultiplier * m_OutOfBounds.x;
            }

            if (posTop > m_BoundCorners[2].y) {
                //Out of bounds Top
                m_OutOfBounds.y = posTop - m_BoundCorners[2].y;
                
                anchorPosition.y = m_TopOutOfBoundsOffset.AnchorPosition;
                anchorRelativeOffset.y = m_TopOutOfBoundsOffset.AnchorRelativeOffset;
                pixelFixedOffset.y = m_TopOutOfBoundsOffset.PixelFixedOffset;
                pixelFixedOffset.y += m_TopOutOfBoundsOffset.OutOfBoundOffsetMultiplier * m_OutOfBounds.y;
                
            } else if (postBot < m_BoundCorners[0].y) {
                //Out of bounds Bot
                m_OutOfBounds.y = postBot - m_BoundCorners[0].y;
                
                anchorPosition.y = m_BottomOutOfBoundsOffset.AnchorPosition;
                anchorRelativeOffset.y = m_BottomOutOfBoundsOffset.AnchorRelativeOffset;
                pixelFixedOffset.y = m_BottomOutOfBoundsOffset.PixelFixedOffset;
                pixelFixedOffset.y += m_BottomOutOfBoundsOffset.OutOfBoundOffsetMultiplier * m_OutOfBounds.y;
            }

            if (m_OutOfBounds == Vector2.zero) {
                return;
            }
            
            var newAnchor = m_SetAnchorPosition ? anchorPosition : m_PanelToPlace.pivot;
            var newPosition = slotRectTransform.position;

            var positionOffset = new Vector2(
                slotRectTransform.sizeDelta.x * anchorRelativeOffset.x,
                slotRectTransform.sizeDelta.y * anchorRelativeOffset.y);

            positionOffset = (positionOffset + pixelFixedOffset) * m_Canvas.scaleFactor;
            
            if (m_SetAnchorPosition) {
                m_PanelToPlace.anchorMax = newAnchor;
                m_PanelToPlace.anchorMin = newAnchor;
                m_PanelToPlace.pivot = newAnchor;
            }
            
            // Set the new position
            newPosition = new Vector3(
                newPosition.x + positionOffset.x,
                newPosition.y + positionOffset.y,
                newPosition.z);

            m_PanelToPlace.position = newPosition;
        }
    }
}