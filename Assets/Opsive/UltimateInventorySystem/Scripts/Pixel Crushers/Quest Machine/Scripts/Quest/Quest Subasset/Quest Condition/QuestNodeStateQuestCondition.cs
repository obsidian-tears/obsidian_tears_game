// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Quest condition that becomes true when another quest's node reaches a specified state.
    /// </summary>
    [Serializable]
    public class QuestNodeStateQuestCondition : QuestCondition, IMessageHandler
    {

        [Tooltip("ID of quest to monitor. Leave blank to monitor this quest.")]
        [SerializeField]
        private StringField m_requiredQuestID;

        [Tooltip("ID of quest node to monitor. Leave blank to monitor this quest node.")]
        [SerializeField]
        private StringField m_requiredQuestNodeID;

        [Tooltip("Required quest node state.")]
        [SerializeField]
        private QuestNodeState m_requiredState;

        /// <summary>
        /// ID of quest to monitor.
        /// </summary>
        public StringField requiredQuestID
        {
            get { return (StringField.IsNullOrEmpty(m_requiredQuestID) && quest != null) ? quest.id : m_requiredQuestID; }
            set { m_requiredQuestID = value; }
        }

        /// <summary>
        /// ID of quest node to monitor. Leave blank for main quest state.
        /// </summary>
        public StringField requiredQuestNodeID
        {
            get { return (StringField.IsNullOrEmpty(m_requiredQuestNodeID) && questNode != null) ? questNode.id : m_requiredQuestNodeID; }
            set { m_requiredQuestNodeID = value; }
        }

        /// <summary>
        /// State that monitored quest node must be in.
        /// </summary>
        public QuestNodeState requiredState
        {
            get { return m_requiredState; }
            set { m_requiredState = value; }
        }

        public override string GetEditorName()
        {
            if (StringField.IsNullOrEmpty(requiredQuestID) && StringField.IsNullOrEmpty(requiredQuestNodeID))
            {
                return "Quest Node State: " + requiredState;
            }
            else if (StringField.IsNullOrEmpty(requiredQuestID))
            {
                return "Quest Node State: '" + requiredQuestNodeID + "' State == " + requiredState;
            }
            else if (StringField.IsNullOrEmpty(requiredQuestNodeID))
            {
                return "Quest Node State: Quest '" + requiredQuestID + "' Node (unspecified) == " + requiredState;
            }
            else
            {
                return "Quest Note State: Quest '" + requiredQuestID + "' Node '" + requiredQuestNodeID + "' == " + requiredState;
            }
       }

        public override void StartChecking(System.Action trueAction)
        {
            base.StartChecking(trueAction);
            if (requiredQuestID == null) return;
            if (QuestMachine.GetQuestNodeState(requiredQuestID, requiredQuestNodeID) == requiredState)
            {
                SetTrue();
            }
            else
            {
                MessageSystem.AddListener(this, QuestMachineMessages.QuestStateChangedMessage, requiredQuestID.value);
            }
        }

        public override void StopChecking()
        {
            base.StopChecking();
            MessageSystem.RemoveListener(this);
        }

        void IMessageHandler.OnMessage(MessageArgs messageArgs)
        {
            if (!isChecking) return;
            if (messageArgs.values == null || messageArgs.values.Length < 2 || requiredQuestID == null) return;
            var questID = messageArgs.parameter;
            if (!string.Equals(questID, StringField.GetStringValue(requiredQuestID))) return;
            var questNodeID = QuestMachineMessages.ArgToString(messageArgs.values[0]);
            if (!string.Equals(questNodeID, requiredQuestNodeID.value)) return;
            var stateValue = messageArgs.values[1];
            var state = (stateValue != null && stateValue.GetType() == typeof(QuestNodeState)) ? (QuestNodeState)stateValue : QuestNodeState.Inactive;
            if (state == requiredState) SetTrue();
        }

    }

}
