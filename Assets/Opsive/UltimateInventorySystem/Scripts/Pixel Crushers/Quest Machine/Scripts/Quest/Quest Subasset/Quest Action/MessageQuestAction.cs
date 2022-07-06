// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Sends a message to the MessageSystem.
    /// </summary>
    public class MessageQuestAction : QuestAction
    {

        [Tooltip("Required message sender.")]
        [SerializeField]
        private QuestMessageParticipant m_senderSpecifier = QuestMessageParticipant.QuestGiver;

        [Tooltip("ID of message sender. Can also be {QUESTERID} or {QUESTGIVERID}. If blank, uses quest giver's ID.")]
        [SerializeField]
        private StringField m_senderID = new StringField();

        [Tooltip("Required message target.")]
        [SerializeField]
        private QuestMessageParticipant m_targetSpecifier = QuestMessageParticipant.Any;

        [Tooltip("ID of message target. Can also be {QUESTERID} or {QUESTGIVERID}. Leave blank to broadcast to all listeners.")]
        [SerializeField]
        private StringField m_targetID = new StringField();

        [Tooltip("Message to send.")]
        [SerializeField]
        private StringField m_message;

        [Tooltip("Parameter to send with message.")]
        [SerializeField]
        private StringField m_parameter;

        [Tooltip("Optional value to pass with message.")]
        [SerializeField]
        private MessageValue m_value = new MessageValue();

        /// <summary>
        /// Required message sender.
        /// </summary>
        public QuestMessageParticipant senderSpecifier
        {
            get { return m_senderSpecifier; }
            set { m_senderSpecifier = value; }
        }

        /// <summary>
        /// ID of message sender. If blank, uses quest giver's ID.
        /// </summary>
        public StringField senderID
        {
            get
            {
                if (senderSpecifier == QuestMessageParticipant.QuestGiver ||  StringField.IsNullOrEmpty(m_senderID) || StringField.GetStringValue(m_senderID) == QuestMachineTags.QUESTGIVER)
                {
                    return (quest != null) ? quest.questGiverID : null;
                }
                else
                {
                    return QuestMachineTags.GetIDBySpecifier(senderSpecifier, m_senderID);
                }
            }
            set
            {
                m_senderID = value;
            }
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
        /// ID of message target. Can also be {QUESTERID} or {QUESTGIVERID}. Leave blank to broadcast to all listeners.
        /// </summary>
        public StringField targetID
        {
            get { return QuestMachineTags.GetIDBySpecifier(targetSpecifier, m_targetID); }
            set { m_targetID = value; }
        }

        /// <summary>
        /// Message to send.
        /// </summary>
        public StringField message
        {
            get { return m_message; }
            set { m_message = value; }
        }

        /// <summary>
        /// Parameter to send with message.
        /// </summary>
        public StringField parameter
        {
            get { return m_parameter; }
            set { m_parameter = value; }
        }

        /// <summary>
        /// Optional value to pass with message.
        /// </summary>
        public MessageValue value
        {
            get { return m_value; }
            set { m_value = value; }
        }

        public string runtimeSenderID { get { return QuestMachineTags.ReplaceTags(StringField.GetStringValue(senderID), quest); } }

        public string runtimeTargetID { get { return QuestMachineTags.ReplaceTags(StringField.GetStringValue(targetID), quest); } }

        public string runtimeMessage { get { return QuestMachineTags.ReplaceTags(StringField.GetStringValue(message), quest); } }

        public string runtimeParameter { get { return QuestMachineTags.ReplaceTags(StringField.GetStringValue(parameter), quest); } }

        public string runtimeStringValue { get { return QuestMachineTags.ReplaceTags(value.stringValue, quest); } }

        public override string GetEditorName()
        {
            return StringField.IsNullOrEmpty(m_message) ? "Message" : ("Message: " + m_message.value + " " + StringField.GetStringValue(m_parameter) + " " + m_value.EditorNameValue());
        }

        public override void Execute()
        {
            if (value == null) value = new MessageValue();
            switch (value.valueType)
            {
                case MessageValueType.Int:
                    MessageSystem.SendMessageWithTarget(runtimeSenderID, runtimeTargetID, runtimeMessage, runtimeParameter, value.intValue);
                    break;
                case MessageValueType.String:
                    MessageSystem.SendMessageWithTarget(runtimeSenderID, runtimeTargetID, runtimeMessage, runtimeParameter, runtimeStringValue);
                    break;
                default:
                    MessageSystem.SendMessageWithTarget(runtimeSenderID, runtimeTargetID, runtimeMessage, runtimeParameter);
                    break;
            }
        }

        public override void AddTagsToDictionary()
        {
            AddTagsToDictionary(senderID);
            AddTagsToDictionary(targetID);
            AddTagsToDictionary(message);
            AddTagsToDictionary(parameter);
            if (value != null && value.valueType == MessageValueType.String) AddTagsToDictionary(value.stringValue);
        }

   }

}
