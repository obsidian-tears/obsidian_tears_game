/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Item
{
    using System;
    using Opsive.UltimateInventorySystem.UI.Item.DragAndDrop;
    using Opsive.UltimateInventorySystem.UI.Panels;
    using Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers;
    using System.Collections.Generic;
    using Opsive.UltimateInventorySystem.Core;
    using Opsive.UltimateInventorySystem.Input;
    using Opsive.UltimateInventorySystem.UI.Item.ItemViewModules;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    /// <summary>
    /// The Item View Slot move cursor, used to Move items without a pointer.
    /// </summary>
    public class ItemViewSlotMoveCursor : ItemViewSlotsContainerBinding
    {
        [Tooltip("Can be a hidden display, it is required to allow cancelling the move action without closing the other panels.")]
        [SerializeField] internal DisplayPanel m_MoveDisplayPanel;
        [Tooltip("Unbind ItemAction while moving.")]
        [SerializeField] internal ItemViewSlotsContainerBinding[] m_UnbindWhileMoving;
        [Tooltip("Unbind ItemAction while moving.")]
        [SerializeField] internal GameObject[] m_UnbindInterfaceWhileMoving;
        [Tooltip("Event on start move.")]
        [SerializeField] protected UnityEvent m_OnMoveStart;
        [Tooltip("Event on start move.")]
        [SerializeField] protected UnityEvent m_OnMoveEnd;

        protected bool m_IsMoving;
        protected ItemViewSlotEventData m_ItemViewSlotEventData;
        protected List<IItemViewSlotContainerBinding> m_UnbindItemViewSlotContainersWhileMoving;

        private ItemViewSlotCursorManager ItemViewSlotCursorManager =>
            m_ItemViewSlotsContainer.ItemViewSlotCursor;
        
        public UnityEvent OnMoveStart {
            get => m_OnMoveStart;
            set => m_OnMoveStart = value;
        }
        
        public UnityEvent OnMoveEnd {
            get => m_OnMoveEnd;
            set => m_OnMoveEnd = value;
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="force">Force the initialize.</param>
        public override void Initialize(bool force)
        {
            if (m_IsInitialized && !force) { return; }
            base.Initialize(force);

            m_UnbindItemViewSlotContainersWhileMoving = new List<IItemViewSlotContainerBinding>();
            m_UnbindItemViewSlotContainersWhileMoving.AddRange(m_UnbindWhileMoving);
            for (int i = 0; i < m_UnbindInterfaceWhileMoving.Length; i++) {
                m_UnbindItemViewSlotContainersWhileMoving.AddRange(m_UnbindInterfaceWhileMoving[i].GetComponents<IItemViewSlotContainerBinding>());
            }
        }

        private void HandleOnMoveEnd()
        {
            if (m_IsMoving == false) { return; }
            
            EndMove();
            
            if (m_MoveDisplayPanel != null) {
                m_MoveDisplayPanel.Close(true);
            }
        }

        private void HandleOnMoveStart()
        {
            //Do nothing
        }

        /// <summary>
        /// A slot container was bound.
        /// </summary>
        protected override void OnBindItemViewSlotContainer()
        {
            if (m_ItemViewSlotEventData == null) {
                m_ItemViewSlotEventData = new ItemViewSlotEventData();
            }
            if (m_MoveDisplayPanel != null) {
                m_MoveDisplayPanel.OnClose += CancelMove;
            }
            
            Shared.Events.EventHandler.RegisterEvent(ItemViewSlotCursorManager.gameObject,
                EventNames.c_ItemViewSlotCursorManagerGameobject_StartMove, HandleOnMoveStart);
            Shared.Events.EventHandler.RegisterEvent(ItemViewSlotCursorManager.gameObject,
                EventNames.c_ItemViewSlotCursorManagerGameobject_EndMove, HandleOnMoveEnd);
        }

        /// <summary>
        /// The slot container was unbound.
        /// </summary>
        protected override void OnUnbindItemViewSlotContainer()
        {
            if (m_MoveDisplayPanel != null) {
                m_MoveDisplayPanel.OnClose -= CancelMove;
            }
            
            Shared.Events.EventHandler.UnregisterEvent(ItemViewSlotCursorManager.gameObject,
                EventNames.c_ItemViewSlotCursorManagerGameobject_StartMove, HandleOnMoveStart);
            Shared.Events.EventHandler.UnregisterEvent(ItemViewSlotCursorManager.gameObject,
                EventNames.c_ItemViewSlotCursorManagerGameobject_EndMove, HandleOnMoveEnd);
        }

        /// <summary>
        /// Cancel the move.
        /// </summary>
        private void CancelMove()
        {
            if (m_IsMoving == false) {
                return;
            }
            ItemViewSlotCursorManager.RemoveItemView();
            //EndMove();
        }

        /// <summary>
        /// The move ended.
        /// </summary>
        protected void EndMove()
        {
            m_IsMoving = false;
            for (int i = 0; i < m_UnbindItemViewSlotContainersWhileMoving.Count; i++) {
                m_UnbindItemViewSlotContainersWhileMoving[i]?.BindItemViewSlotContainer();
            }
            
            OnMoveEnd?.Invoke();
        }

        /// <summary>
        /// Start moving the item view slot.
        /// </summary>
        /// <param name="index">The index of the item view slot.</param>
        public void StartMove(int index)
        {
            if (ItemViewSlotCursorManager.CanMove() == false) { return; }

            var itemViewSlot = m_ItemViewSlotsContainer.GetItemViewSlot(index);

            if (m_MoveDisplayPanel != null) {
                m_MoveDisplayPanel.Open(m_MoveDisplayPanel.Manager.SelectedDisplayPanel,
                    null, false);
            }

            m_IsMoving = true;

            m_ItemViewSlotEventData.SetValues(m_ItemViewSlotsContainer, index);

            var basePosition = itemViewSlot.transform.position;
            var viewPosition = basePosition;
            for (int i = 0; i < itemViewSlot.ItemView.Modules.Count; i++) {
                var module = itemViewSlot.ItemView.Modules[i];
                if (module is ItemShapeItemView itemShapeItemView) {
                    viewPosition = itemShapeItemView.ForegroundItemView.View.transform.position;
                }
            }

            ItemViewSlotCursorManager.StartMove(m_ItemViewSlotEventData, viewPosition, false);

            for (int i = 0; i < m_UnbindItemViewSlotContainersWhileMoving.Count; i++) {
                m_UnbindItemViewSlotContainersWhileMoving[i]?.UnbindItemViewSlotContainer();
            }
            
            OnMoveStart?.Invoke();
        }
    }
}