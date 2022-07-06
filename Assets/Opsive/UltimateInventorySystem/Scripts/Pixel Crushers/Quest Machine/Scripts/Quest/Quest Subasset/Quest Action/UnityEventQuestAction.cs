// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.Events;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Invokes a UnityEvent.
    /// </summary>
    public class UnityEventQuestAction : QuestAction
    {

        [SerializeField]
        private UnityEvent m_onExecute = new UnityEvent();

        /// <summary>
        /// UnityEvents to execute.
        /// </summary>
        public UnityEvent onExecute { get { return m_onExecute; } }

        public override string GetEditorName()
        {
            return "UnityEvent";
        }

        public override void Execute()
        {
            if (m_onExecute != null) m_onExecute.Invoke();
        }

    }

}
