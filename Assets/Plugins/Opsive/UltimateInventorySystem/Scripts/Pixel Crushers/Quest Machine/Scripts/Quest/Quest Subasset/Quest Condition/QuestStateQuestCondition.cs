// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Quest condition that becomes true when another quest reaches a specified state.
    /// </summary>
    [Serializable]
    public class QuestStateQuestCondition : QuestCondition, IMessageHandler
    {

        [Tooltip("ID of quest to monitor.")]
        [SerializeField]
        private StringField m_requiredQuestID;

        [Tooltip("Required quest state.")]
        [SerializeField]
        private QuestState m_requiredState;

        /// <summary>
        /// ID of quest to monitor.
        /// </summary>
        public StringField requiredQuestID
        {
            get { return (StringField.IsNullOrEmpty(m_requiredQuestID) && quest != null) ? quest.id : m_requiredQuestID; }
            set { m_requiredQuestID = value; }
        }

        /// <summary>
        /// State that monitored quest must be in.
        /// </summary>
        public QuestState requiredState
        {
            get { return m_requiredState; }
            set { m_requiredState = value; }
        }

        public override string GetEditorName()
        {
            return StringField.IsNullOrEmpty(m_requiredQuestID) ? ("Quest State: " + requiredState) : ("Quest State: " + requiredQuestID.value + " == " + requiredState);
        }

        public override void StartChecking(System.Action trueAction)
        {
            base.StartChecking(trueAction);
            if (requiredQuestID == null) return;
            if (QuestMachine.GetQuestState(requiredQuestID) == requiredState)
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
            if (!isChecking || messageArgs.values == null || messageArgs.values.Length < 2 || requiredQuestID == null) return;
            var questID = messageArgs.parameter;
            if (!StringField.Equals(requiredQuestID, questID)) return;
            var questNodeID = QuestMachineMessages.ArgToString(messageArgs.values[0]);
            if (!string.IsNullOrEmpty(questNodeID)) return;
            var stateValue = messageArgs.values[1];
            var state = (stateValue != null && stateValue.GetType() == typeof(QuestState)) ? (QuestState)stateValue : QuestState.WaitingToStart;
            if (state == requiredState) SetTrue();
        }

    }

}
