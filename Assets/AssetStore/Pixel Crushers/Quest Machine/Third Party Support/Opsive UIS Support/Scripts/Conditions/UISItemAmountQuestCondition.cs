using UnityEngine;
using System;
using Opsive.UltimateInventorySystem.Exchange;
using PixelCrushers.UISSupport;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;

namespace PixelCrushers.QuestMachine.UISSupport
{

    /// <summary>
    /// Becomes true when an inventory's item amount reaches a specified amount.
    /// </summary>
    public class UISItemAmountQuestCondition : QuestCondition
    {

        [HelpBox("Leave Inventory Name blank to use Player's inventory. Leave Item Collection Name blank to use any applicable item collection.", HelpBoxMessageType.None)]
        public StringField itemName = new StringField();
        public StringField inventoryName = new StringField();
        [Tooltip("How the item amount applies to the condition.")]
        public CounterValueConditionMode comparisonMode = CounterValueConditionMode.AtLeast;
        public QuestNumber amount = new QuestNumber(1);
        public int counterIndex = -1; // Optional counter to keep in sync.

        private ItemDefinition m_itemDefinition = null;
        private Inventory m_inventory = null;
        private int m_requiredAmount = 1;

        public override string GetEditorName()
        {
            if (StringField.IsNullOrEmpty(itemName)) return "Check Item Amount";
            return !StringField.IsNullOrEmpty(inventoryName)
                ? (inventoryName + " has " + comparisonMode + " " + amount.GetValue(quest) + itemName)
                : ("Player has " + comparisonMode + " " + amount.GetValue(quest) + itemName);
        }

        public override void StartChecking(System.Action trueAction)
        {
            base.StartChecking(trueAction);
            m_requiredAmount = amount.GetValue(quest);
            m_itemDefinition = UISUtility.GetItemDefinition(StringField.GetStringValue(itemName));
            m_inventory = UISUtility.GetInventory(StringField.GetStringValue(inventoryName));
            if (IsTrue())
            {
                SetTrue();
            }
            else if (m_inventory != null)
            {
                Opsive.Shared.Events.EventHandler.RegisterEvent(m_inventory, EventNames.c_Inventory_OnUpdate, OnItemChange);
            }
            else
            {
                if (QuestMachine.debug) Debug.LogWarning($"Quest Machine: {GetType().Name} can't find Inventory.");
            }
        }

        public override void StopChecking()
        {
            base.StopChecking();
            if (m_inventory != null)
            {
                Opsive.Shared.Events.EventHandler.UnregisterEvent(m_inventory, EventNames.c_Inventory_OnUpdate, OnItemChange);
            }
        }

        private void OnItemChange()
        {
            if (IsTrue())
            {
                SetTrue();
            }
        }

        private bool IsTrue()
        {
            var counter = (counterIndex != -1) ? quest.GetCounter(counterIndex) : null;
            if (m_itemDefinition == null || m_inventory == null)
            {
                if (counter != null) counter.currentValue = 0;
                return false;
            }
            var currentAmount = m_inventory.GetItemAmount(m_itemDefinition);
            if (counter != null) counter.currentValue = currentAmount;
            switch (comparisonMode)
            {
                default:
                case CounterValueConditionMode.AtLeast:
                    return currentAmount >= m_requiredAmount;
                case CounterValueConditionMode.AtMost:
                    return currentAmount <= m_requiredAmount;
            }
        }
    }

}
