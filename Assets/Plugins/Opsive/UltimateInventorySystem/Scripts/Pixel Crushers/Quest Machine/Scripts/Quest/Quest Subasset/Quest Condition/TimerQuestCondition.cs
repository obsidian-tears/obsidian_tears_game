// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Quest condition that becomes true after a specified duration.
    /// </summary>
    [Serializable]
    public class TimerQuestCondition : QuestCondition, IQuestTimer
    {

        [Tooltip("Counter to track time left.")]
        [SerializeField]
        private int m_counterIndex;

        /// <summary>
        /// Index of a counter defined in the quest. Inspect the quest's main info to view/edit counters.
        /// </summary>
        public int counterIndex
        {
            get { return m_counterIndex; }
            set { m_counterIndex = value; }
        }

        public override string GetEditorName()
        {
            var counter = (quest != null) ? quest.GetCounter(counterIndex) : null;
            return (counter != null) ? ("Timer: " + counter.name) : "Timer";
        }

        private QuestCounter m_counter = null;

        public override void StartChecking(System.Action trueAction)
        {
            base.StartChecking(trueAction);
            m_counter = (quest != null) ? quest.GetCounter(counterIndex) : null;
            QuestTimerManager.RegisterTimer(this);
        }

        public override void StopChecking()
        {
            base.StopChecking();
            QuestTimerManager.UnregisterTimer(this);
        }

        public void Tick()
        {
            if (m_counter == null) return;
            m_counter.currentValue--;
            if (m_counter.currentValue <= 0)
            {
                if (QuestMachine.debug) Debug.Log("Quest Machine: TimerQuestCondition '" + m_counter.name + "' timer ran out. Setting condition true.", quest);
                SetTrue();
            }
        }

    }

}
