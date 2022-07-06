// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Holds an integer that a quest can use to track a value. Counters can be updated by
    /// messages (e.g., "Kill"+"Orc") or a DataSynchronizer. When the value changes, the 
    /// counter invokes a changed event.
    /// </summary>
    [Serializable]
    public class QuestCounter : IMessageHandler
    {

        #region Serialized Fields

        [Tooltip("Quest counter name.")]
        [SerializeField]
        private StringField m_name;

        [Tooltip("Current value. This value is always clamped between Min Value and Max Value. Make sure Min and Max Values are set appropriately.")]
        [SerializeField]
        private int m_currentValue = 0;

        [Tooltip("Set the initial value to a random value between Min Value and Max Value.")]
        [SerializeField]
        private bool m_randomizeInitialValue = false;

        [Tooltip("Min value.")]
        [SerializeField]
        private int m_minValue = 0;

        [Tooltip("Max value.")]
        [SerializeField]
        private int m_maxValue = 100;

        [Tooltip("How this counter updates it value.")]
        [SerializeField]
        private QuestCounterUpdateMode m_updateMode;

        [Tooltip("When Update Mode is Messages, these messages affect the counter value.")]
        [SerializeField]
        private List<QuestCounterMessageEvent> m_messageEventList = new List<QuestCounterMessageEvent>();

        #endregion

        #region Accessor Properties for Serialized Fields

        /// <summary>
        /// Quest counter name.
        /// </summary>
        public StringField name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        /// <summary>
        /// Current value. When you set this property, it notifies listeners.
        /// </summary>
        public int currentValue
        {
            get { return m_currentValue; }
            set { SetValue(value); }
        }

        /// <summary>
        /// Initialize the current value to a random value between min and max value.
        /// </summary>
        public bool randomizeInitialValue
        {
            get { return m_randomizeInitialValue; }
            set { m_randomizeInitialValue = value; }
        }

        /// <summary>
        /// Min value.
        /// </summary>
        public int minValue
        {
            get { return m_minValue; }
            set { m_minValue = value; }
        }

        /// <summary>
        /// Max value.
        /// </summary>
        public int maxValue
        {
            get { return m_maxValue; }
            set { m_maxValue = value; }
        }

        /// <summary>
        /// How this counter updates its value.
        /// </summary>
        public QuestCounterUpdateMode updateMode
        {
            get { return m_updateMode; }
            set { m_updateMode = value; }
        }

        /// <summary>
        /// When updateMode is Messages, these message events affect the counter value.
        /// </summary>
        public List<QuestCounterMessageEvent> messageEventList
        {
            get { return m_messageEventList; }
            set { m_messageEventList = value; }
        }

        public event QuestCounterParameterDelegate changed = delegate { };

        #endregion

        #region Private Fields

        private bool m_isListening = false;

        private Quest m_quest = null;

        private StringField questID { get { return (m_quest != null) ? m_quest.id : StringField.empty; } }

        #endregion

        #region Initialization

        public QuestCounter() { }

        public QuestCounter(StringField name, int currentValue, int minValue, int maxValue, QuestCounterUpdateMode updateMode)
        {
            m_name = name;
            m_currentValue = currentValue;
            m_minValue = minValue;
            m_maxValue = maxValue;
            m_updateMode = QuestCounterUpdateMode.Messages;
        }

        public void SetRuntimeReferences(Quest quest)
        {
            this.m_quest = quest;
        }

        public void InitializeToRandomValue()
        {
            if (!randomizeInitialValue) return;
            m_currentValue = UnityEngine.Random.Range(minValue, maxValue + 1);
        }

        public void SetValue(int newValue, QuestCounterSetValueMode setValueMode = QuestCounterSetValueMode.InformListeners)
        {
            var clampedNewValue = Mathf.Clamp(newValue, minValue, maxValue);
            if (clampedNewValue == m_currentValue) return;
            m_currentValue = clampedNewValue;
            if (setValueMode != QuestCounterSetValueMode.DontInformListeners)
            {
                var informDataSync = (updateMode == QuestCounterUpdateMode.DataSync) && (setValueMode != QuestCounterSetValueMode.DontInformDataSync);
                if (informDataSync) MessageSystem.SendMessage(this, DataSynchronizer.RequestDataSourceChangeValueMessage, name, currentValue);
                QuestMachineMessages.QuestCounterChanged(this, questID, name, currentValue);
                try
                {
                    changed(this);
                }
                catch (Exception e) // Don't let exceptions in user-added events break our code.
                {
                    if (Debug.isDebugBuild) Debug.LogException(e);
                }
            }
        }

        #endregion

        #region Messages

        public void SetListeners(bool enable)
        {
            if (!Application.isPlaying || (enable && m_isListening) || (!enable && !m_isListening)) return;
            m_isListening = enable;
            if (enable)
            {
                switch (updateMode)
                {
                    case QuestCounterUpdateMode.DataSync:
                        MessageSystem.AddListener(this, DataSynchronizer.DataSourceValueChangedMessage, name);
                        break;
                    case QuestCounterUpdateMode.Messages:
                        MessageSystem.AddListener(this, QuestMachineMessages.SetQuestCounterMessage, name);
                        MessageSystem.AddListener(this, QuestMachineMessages.IncrementQuestCounterMessage, name);
                        break;
                    default:
                        if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: Internal error. Unrecognized counter update mode '" + updateMode + "'. Please contact the developer.", m_quest);
                        break;
                }
                if (messageEventList != null)
                {
                    for (int i = 0; i < messageEventList.Count; i++)
                    {
                        var messageEvent = messageEventList[i];
                        if (messageEvent != null) MessageSystem.AddListener(this, QuestMachineTags.ReplaceTags(messageEvent.message, m_quest), QuestMachineTags.ReplaceTags(messageEvent.parameter, m_quest));
                    }
                }
            }
            else
            {
                MessageSystem.RemoveListener(this);
            }
        }

        public void OnMessage(MessageArgs messageArgs)
        {
            if (QuestMachine.debug) Debug.Log("Quest Machine: QuestCounter[" + name + "].OnMessage(" + messageArgs.message + ", " + messageArgs.parameter + ")", m_quest);
            var newValue = m_currentValue;
            switch (messageArgs.message)
            {
                case DataSynchronizer.DataSourceValueChangedMessage:
                    newValue = messageArgs.intValue;
                    break;
                case QuestMachineMessages.SetQuestCounterMessage:
                    newValue = messageArgs.intValue;
                    break;
                case QuestMachineMessages.IncrementQuestCounterMessage:
                    newValue += messageArgs.intValue;
                    break;
                default:
                    if (messageEventList == null) break;
                    for (int i = 0; i < messageEventList.Count; i++)
                    {
                        var messageEvent = messageEventList[i];
                        if (messageEvent != null && messageArgs.Matches(messageEvent.message, messageEvent.parameter) && 
                            QuestMachineMessages.IsRequiredID(messageArgs.sender, QuestMachineTags.ReplaceTags(messageEvent.senderID, m_quest)) &&
                            QuestMachineMessages.IsRequiredID(messageArgs.target, QuestMachineTags.ReplaceTags(messageEvent.targetID, m_quest)))
                        {
                            switch (messageEvent.operation)
                            {
                                case QuestCounterMessageEvent.Operation.ModifyByLiteralValue:
                                    newValue += messageEvent.literalValue;
                                    break;
                                case QuestCounterMessageEvent.Operation.ModifyByMessageValue:
                                    newValue += messageArgs.intValue;
                                    break;
                                case QuestCounterMessageEvent.Operation.SetToLiteralValue:
                                    newValue = messageEvent.literalValue;
                                    break;
                                case QuestCounterMessageEvent.Operation.SetToMessageValue:
                                    newValue = messageArgs.intValue;
                                    break;
                            }
                        }
                    }
                    break;
            }
            SetValue(newValue, QuestCounterSetValueMode.DontInformDataSync);
        }

        #endregion

    }
}
