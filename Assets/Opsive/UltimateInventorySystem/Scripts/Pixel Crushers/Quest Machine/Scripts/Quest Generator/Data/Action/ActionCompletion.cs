// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Defines how to know when an action is completed.
    /// </summary>
    [Serializable]
    public class ActionCompletion
    {
        public enum Mode { Message, Counter }

        [Tooltip("How the action is completed.")]
        [SerializeField]
        private Mode m_mode;

        // For Messages:

        [Tooltip("Required message sender ID, or any sender if blank. Can also be {QUESTERID} or {QUESTGIVERID}. Sender must have a Quest Giver or Entity component.")]
        [SerializeField]
        private StringField m_senderID;

        [Tooltip("Required message target ID, or any target if blank. Can also be {QUESTERID} or {QUESTGIVERID}. Target must have a Quest Giver or Entity component.")]
        [SerializeField]
        private StringField m_targetID;

        [Tooltip("Require this message for completion.")]
        [SerializeField]
        private StringField m_message;

        [Tooltip("Required this message parameter for completion.")]
        [SerializeField]
        private StringField m_parameter;

        // For Counters:

        [Tooltip("Counter name. Entity name will be prefixed to counter name.")]
        [SerializeField]
        private StringField m_baseCounterName;

        [Tooltip("Initial value.")]
        [SerializeField]
        private int m_initialValue = 0;

        [Tooltip("Min value.")]
        [SerializeField]
        private int m_minValue = 0;

        [Tooltip("Max value.")]
        [SerializeField]
        private int m_maxValue = 100;

        [Tooltip("Required value. If mode is AtLeast, action will require at least this value. If mode is AtMost, action will require no more than this value.")]
        [SerializeField]
        private int m_requiredValue = 1;

        [Tooltip("Max value.")]
        [SerializeField]
        private CounterValueConditionMode m_counterValueMode = CounterValueConditionMode.AtLeast;

        [Tooltip("How this counter updates it value.")]
        [SerializeField]
        private QuestCounterUpdateMode m_updateMode;

        [Tooltip("When Update Mode is Messages, these messages affect the counter value.")]
        [SerializeField]
        private List<QuestCounterMessageEvent> m_messageEventList = new List<QuestCounterMessageEvent>();

        /// <summary>
        /// Mode by which the action is completed.
        /// </summary>
        public Mode mode
        {
            get { return m_mode; }
            set { m_mode = value; }
        }

        /// <summary>
        /// Required message sender ID, or any sender if blank. Can also be {QUESTERID} or {QUESTGIVERID}. Sender must have a Quest Giver or Entity component.
        /// </summary>
        public StringField senderID
        {
            get { return m_senderID; }
            set { m_senderID = value; }
        }

        /// <summary>
        /// Required message target ID, or any target if blank. Can also be {QUESTERID} or {QUESTGIVERID}. Target must have a Quest Giver or Entity component.
        /// </summary>
        public StringField targetID
        {
            get { return m_targetID; }
            set { m_targetID = value; }
        }

        /// <summary>
        /// Required message.
        /// </summary>
        public StringField message
        {
            get { return m_message; }
            set { m_message = value; }
        }

        /// <summary>
        /// Required message parameter (or blank).
        /// </summary>
        public StringField parameter
        {
            get { return m_parameter; }
            set { m_parameter = value; }
        }

        /// <summary>
        /// Base counter name that will be prefixed by entity type name.
        /// </summary>
        public StringField baseCounterName
        {
            get { return m_baseCounterName; }
            set { m_baseCounterName = value; }
        }

        /// <summary>
        /// Initial value.
        /// </summary>
        public int initialValue
        {
            get { return m_initialValue; }
            set { m_initialValue = value; }
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
        /// The required value for the counter.
        /// </summary>
        public int requiredValue
        {
            get { return m_requiredValue; }
            set { m_requiredValue = value; }
        }

        /// <summary>
        /// How the counter value applies to the condition.
        /// </summary>
        public CounterValueConditionMode counterValueMode
        {
            get { return m_counterValueMode; }
            set { m_counterValueMode = value; }
        }

        /// <summary>
        /// How the counter updates its value.
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
    }

}