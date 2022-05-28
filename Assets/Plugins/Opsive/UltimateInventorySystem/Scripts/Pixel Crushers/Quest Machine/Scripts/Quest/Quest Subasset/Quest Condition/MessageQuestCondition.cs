// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Quest condition that becomes true on receipt of a message from the MessageSystem.
    /// </summary>
    [Serializable]
    public class MessageQuestCondition : QuestCondition, IMessageHandler
    {

        [Tooltip("Required message sender.")]
        [SerializeField]
        private QuestMessageParticipant m_senderSpecifier = QuestMessageParticipant.Any;

        [Tooltip("Required message sender ID, or any sender if blank. Can also be {QUESTERID} or {QUESTGIVERID}. Sender must have a Quest Giver or Entity component.")]
        [SerializeField]
        private StringField m_senderID;

        [Tooltip("Required message target.")]
        [SerializeField]
        private QuestMessageParticipant m_targetSpecifier = QuestMessageParticipant.Any;

        [Tooltip("Required message target ID, or any target if blank. Can also be {QUESTERID} or {QUESTGIVERID}. Target must have a Quest Giver or Entity component.")]
        [SerializeField]
        private StringField m_targetID;

        [Tooltip("Required message. Condition is true when this message is received with the parameter below.")]
        [SerializeField]
        private StringField m_message;

        [Tooltip("Required parameter for message. Condition is true when the message above is received with this parameter. (Leave blank to accept any parameter.)")]
        [SerializeField]
        private StringField m_parameter;

        [Tooltip("Additional value to expected with the message.")]
        [SerializeField]
        private MessageValue m_value;

        /// <summary>
        /// Required message sender.
        /// </summary>
        public QuestMessageParticipant senderSpecifier
        {
            get { return m_senderSpecifier; }
            set { m_senderSpecifier = value; }
        }

        /// <summary>
        /// Required message sender ID, or any sender if blank. Can also be {QUESTERID} or {QUESTGIVERID}. Sender must have a Quest Giver or Entity component.
        /// </summary>
        public StringField senderID
        {
            get { return QuestMachineTags.GetIDBySpecifier(senderSpecifier, m_senderID); }
            set { m_senderID = value; }
        }

        /// <summary>
        /// Required message target.
        /// </summary>
        public QuestMessageParticipant targetSpecifier
        {
            get { return m_targetSpecifier; }
            set { m_targetSpecifier = value; }
        }

        /// <summary>
        /// Required message target ID, or any target if blank. Can also be {QUESTERID} or {QUESTGIVERID}. Target must have a Quest Giver or Entity component.
        /// </summary>
        public StringField targetID
        {
            get { return QuestMachineTags.GetIDBySpecifier(targetSpecifier, m_targetID); }
            set { m_targetID = value; }
        }

        /// <summary>
        /// Required message. Condition is true when this message is received with the parameter below.
        /// </summary>
        public StringField message
        {
            get { return m_message; }
            set { m_message = value; }
        }

        /// <summary>
        /// Required parameter for message. Condition is true when the message above is received with this parameter. (Leave blank to accept any parameter.)
        /// </summary>
        public StringField parameter
        {
            get { return m_parameter; }
            set { m_parameter = value; }
        }

        /// <summary>
        /// Additional value to expected with the message.
        /// </summary>
        public MessageValue value
        {
            get { return m_value; }
            set { m_value = value; }
        }

        public string runtimeSenderID { get { return QuestMachineTags.ReplaceTags(StringField.GetStringValue(senderID), quest); } }

        public string runtimeTargetID{ get { return QuestMachineTags.ReplaceTags(StringField.GetStringValue(targetID), quest); } }

        public string runtimeMessage { get { return QuestMachineTags.ReplaceTags(StringField.GetStringValue(message), quest); } }

        public string runtimeParameter { get { return QuestMachineTags.ReplaceTags(StringField.GetStringValue(parameter), quest); } }

        public string runtimeStringValue { get { return (value != null) ? QuestMachineTags.ReplaceTags(value.stringValue, quest) : string.Empty; } }

        public override string GetEditorName()
        {
            return StringField.IsNullOrEmpty(message) ? "Message" : "Message: " + message.value + " " + StringField.GetStringValue(parameter) + " " + value.EditorNameValue();
        }

        public override void AddTagsToDictionary()
        {
            AddTagsToDictionary(senderID);
            AddTagsToDictionary(targetID);
            AddTagsToDictionary(message);
            AddTagsToDictionary(parameter);
            if (value != null && value.valueType == MessageValueType.String) AddTagsToDictionary(value.stringValue);
        }

        public override void StartChecking(System.Action trueAction)
        {
            base.StartChecking(trueAction);
            MessageSystem.AddListener(this, runtimeMessage, runtimeParameter);
        }

        public override void StopChecking()
        {
            base.StopChecking();
            MessageSystem.RemoveListener(this);
        }

        void IMessageHandler.OnMessage(MessageArgs messageArgs)
        {
            if (!(QuestMachineMessages.IsRequiredID(messageArgs.sender, runtimeSenderID) &&
                QuestMachineMessages.IsRequiredID(messageArgs.target, runtimeTargetID) && 
                IsRequiredValue(messageArgs))) return;
            if (QuestMachine.debug) Debug.Log("Quest Machine: MessageQuestCondition.OnMessage( " + messageArgs.message + ", " + messageArgs.parameter + ")", quest);
            SetTrue();
        }

        private bool IsRequiredValue(MessageArgs messageArgs)
        {
            if (value == null) return true;
            if (value.valueType == MessageValueType.None) return true;
            if (messageArgs.firstValue == null) return false;
            switch (value.valueType)
            {
                case MessageValueType.String:
                    return QuestMachineMessages.ArgToString(messageArgs.firstValue) == runtimeStringValue;
                case MessageValueType.Int:
                    return QuestMachineMessages.ArgToInt(messageArgs.firstValue) == value.intValue;
                default:
                    Debug.LogError("Quest Machine: Unhandled MessageValueType " + value.valueType + ". Please contact the developer.", quest);
                    return false;
            }
        }

    }

}
