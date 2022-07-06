// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using UnityEngine.Events;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Provides methods to control parts of a quest.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class QuestControl : MonoBehaviour
    {

        #region Serialized Private Fields

        [Header("Quest")]

        [Tooltip("ID of quest to use in methods such as SetQuestState.")]
        [SerializeField]
        private StringField m_questID;

        [Tooltip("ID of quest node to use in methods such as SetQuestNodeState.")]
        [SerializeField]
        private StringField m_questNodeID;

        [Header("Quest Counter")]

        [Tooltip("Name of quest counter to use in methods such as IncrementQuestCounter.")]
        [SerializeField]
        private StringField m_counterName;

        [Serializable]
        public class ConditionalEvent
        {
            [HelpBox("This component's TryConditionalEvent method checks a quest and required state. If Quest Node ID is non-blank, it also checks a specific node and state. If conditions are met, executes On Condition Met () event.", HelpBoxMessageType.None)]
            public StringField questID;

            public QuestState requiredQuestState;

            public StringField questNodeID;

            public QuestNodeState requiredQuestNodeState;

            public UnityEvent onConditionMet = new UnityEvent();

        }

        [SerializeField]
        private ConditionalEvent m_conditionalEvent;

        #endregion

        #region Public Properties

        public StringField questID
        {
            get { return m_questID; }
            set { m_questID = value; }
        }

        public StringField questNodeID
        {
            get { return m_questNodeID; }
            set { m_questNodeID = value; }
        }

        public StringField counterName
        {
            get { return m_counterName; }
            set { m_counterName = value; }
        }

        public ConditionalEvent conditionalEvent
        {
            get { return m_conditionalEvent; }
            set { m_conditionalEvent = value; }
        }

        #endregion

        /// <summary>
        /// Sets the quest to a state. The quest is specified in the 
        /// questID property.
        /// </summary>
        /// <param name="state">New state.</param>
        public void SetQuestState(QuestState state)
        {
            var quest = QuestMachine.GetQuestInstance(questID);
            if (quest == null)
            {
                if (QuestMachine.debug) Debug.LogWarning("Quest Machine: Can't find quest with ID '" + questID + "' to set its state.", this);
                return;
            }
            quest.SetState(state);
        }

        /// <summary>
        /// Sets the quest node to a state. The quest and quest node are specified
        /// in the questID and questNodeID properties.
        /// </summary>
        /// <param name="state">New state.</param>
        public void SetQuestNodeState(QuestNodeState state)
        {
            var quest = QuestMachine.GetQuestInstance(questID);
            if (quest == null)
            {
                if (QuestMachine.debug) Debug.LogWarning("Quest Machine: Can't find quest with ID '" + questID + "' to set the state of node with ID '" + questNodeID + "'.", this);
                return;
            }
            var questNode = quest.GetNode(questNodeID);
            if (questNode == null)
            {
                if (QuestMachine.debug) Debug.LogWarning("Quest Machine: Can't find node with ID '" + questNodeID + "' in quest '" + questID + "' to set its state.", this);
                return;
            }
            questNode.SetState(state);
        }

        /// <summary>
        /// Sets a quest counter value. The quest and counter are specified in the
        /// questID and counterName properties.
        /// </summary>
        /// <param name="value">New value.</param>
        public void SetQuestCounter(int value)
        {
            AdjustCounter(value, true);
        }

        /// <summary>
        /// Increments a quest counter value. The quest and counter are specified in the
        /// questID and counterName properties.
        /// </summary>
        /// <param name="value">Value to increment/decrement by.</param>
        public void IncrementQuestCounter(int value)
        {
            AdjustCounter(value, false);
        }

        private void AdjustCounter(int value, bool set)
        { 
            var quest = QuestMachine.GetQuestInstance(questID);
            if (quest == null)
            {
                if (QuestMachine.debug) Debug.LogWarning("Quest Machine: Can't find quest with ID '" + questID + "' to adjust counter " + counterName + ".", this);
                return;
            }
            var counter = quest.GetCounter(counterName);
            if (counter == null)
            {
                if (QuestMachine.debug) Debug.LogWarning("Quest Machine: Can't find counter " + counterName + " in quest '" + questID + "' to adjust its value.", this);
                return;
            }
            if (set)
            {
                counter.currentValue = value;
            }
            else
            {
                counter.currentValue += value; ;
            }
        }

        /// <summary>
        /// Sends a message to the Message System. If the message contains a 
        /// colon (:), the part after the colon is sent as the parameter. If
        /// it contains a second colon, the part after the second colon is sent
        /// as a value.
        /// </summary>
        /// <param name="message"></param>
        public void SendToMessageSystem(string message)
        {
            QuestMachineMessages.SendCompositeMessage(this, message);
        }

        /// <summary>
        /// Shows an alert.
        /// </summary>
        /// <param name="text">Text to show.</param>
        public void ShowAlert(string text)
        {
            if (QuestMachine.defaultQuestAlertUI == null || string.IsNullOrEmpty(text)) return;
            QuestMachine.defaultQuestAlertUI.ShowAlert(text);
        }

        /// <summary>
        /// Checks the conditions of the assigned conditional event
        /// </summary>
        public void TryConditionalEvent()
        {
            if (IsConditionMet())
            {
                conditionalEvent.onConditionMet.Invoke();
            }
        }

        /// <summary>
        /// Checks if the condition event's conditions are met.
        /// </summary>
        /// <returns></returns>
        public bool IsConditionMet()
        {
            if (StringField.IsNullOrEmpty(conditionalEvent.questID)) return true;
            if (QuestMachine.GetQuestState(conditionalEvent.questID) != conditionalEvent.requiredQuestState) return false;
            if (StringField.IsNullOrEmpty(conditionalEvent.questNodeID)) return true;
            return QuestMachine.GetQuestNodeState(conditionalEvent.questID, conditionalEvent.questNodeID) == conditionalEvent.requiredQuestNodeState;
        }

    }
}
