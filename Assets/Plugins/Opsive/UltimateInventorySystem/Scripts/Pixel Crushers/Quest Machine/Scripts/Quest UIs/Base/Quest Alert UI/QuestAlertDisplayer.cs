// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Listens for quest alerts and displays them through a QuestAlertUI.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class QuestAlertDisplayer : MonoBehaviour, IMessageHandler
    {

        [Tooltip("The Quest Alert UI to use. If unassigned, use the QuestMachine's default UI.")]
        [SerializeField]
        [IQuestAlertUIInspectorField]
        private UnityEngine.Object m_questAlertUI = null;

        /// <summary>
        /// The QuestAlertUI to use. If unassigned, defaults to the QuestMachine's default UI.
        /// </summary>
        public IQuestAlertUI questAlertUI
        {
            get { return (m_questAlertUI != null) ? m_questAlertUI as IQuestAlertUI : QuestMachine.defaultQuestAlertUI; }
            set { m_questAlertUI = value as UnityEngine.Object; }
        }

        private void OnEnable()
        {
            MessageSystem.AddListener(this, QuestMachineMessages.QuestAlertMessage, string.Empty);
        }

        private void OnDisable()
        {
            MessageSystem.RemoveListener(this);
        }

        public void OnMessage(MessageArgs messageArgs)
        {
            List<QuestContent> uiContent = (messageArgs.values != null && messageArgs.values.Length > 0) ? (messageArgs.values[0] as List<QuestContent>) : null;
            questAlertUI.ShowAlert(messageArgs.parameter, uiContent);
        }
    }
}