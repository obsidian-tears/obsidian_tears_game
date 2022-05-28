// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Central manager for all timers that implement the IQuestTimer interface. 
    /// Typically one instance of this script exists on the QuestMachineConfiguration 
    /// instance. Every second, it invokes the timers' Tick() method.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class QuestTimerManager : MonoBehaviour
    {

        private static QuestTimerManager s_instance = null;

        private static QuestTimerManager instance
        {
            get
            {
                if (s_instance == null) CreateInstance();
                return s_instance;
            }
        }

#if UNITY_2019_3_OR_NEWER && UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitStaticVariables()
        {
            s_instance = null;
        }
#endif
        private static QuestTimerManager CreateInstance()
        {
            if (QuestMachineConfiguration.isQuitting) return null;
            var attachTo = (QuestMachineConfiguration.instance != null) ? QuestMachineConfiguration.instance.gameObject : new GameObject("Quest Timer Manager");
            return attachTo.GetComponent<QuestTimerManager>() ?? attachTo.AddComponent<QuestTimerManager>();
        }

        /// <summary>
        /// Register a timer to receive ticks.
        /// </summary>
        /// <param name="timer"></param>
        public static void RegisterTimer(IQuestTimer timer)
        {
            if (!Application.isPlaying || QuestMachineConfiguration.isQuitting) return;
            instance.Register(timer);
        }

        /// <summary>
        /// Unregister a timer so it no longer receives ticks.
        /// </summary>
        /// <param name="timer"></param>
        public static void UnregisterTimer(IQuestTimer timer)
        {
            if (!Application.isPlaying || QuestMachineConfiguration.isQuitting) return;
            instance.Unregister(timer);
        }

        private List<IQuestTimer> m_timers = new List<IQuestTimer>();

        private void Awake()
        {
            s_instance = this;
        }

        /// <summary>
        /// Registers a timer with this instance of QuestTimerManager.
        /// This method is usually called implicitly by the static method
        /// QuestTimerManager.RegisterTimer.
        /// </summary>
        public void Register(IQuestTimer timer)
        {
            if (timer == null || m_timers.Contains(timer)) return;
            m_timers.Add(timer);
            if (m_timers.Count == 1)
            {
                // First timer, so start coroutine:
                StartCoroutine(TimerCoroutine());
            }
        }

        /// <summary>
        /// Unregisters a timer from this instance of QuestTimerManager.
        /// This method is usually called implicitly by the static method
        /// QuestTimerManager.UnregisterTimer.
        /// </summary>
        public void Unregister(IQuestTimer timer)
        {
            if (timer == null || !m_timers.Contains(timer)) return;
            m_timers.Remove(timer);
            if (m_timers.Count <= 0)
            {
                // No more timers, so stop coroutine:
                StopAllCoroutines();
            }
        }

        private IEnumerator TimerCoroutine()
        {
            var oneSecond = new WaitForSeconds(1);
            while (true)
            {
                yield return oneSecond;
                Tick();
            }
        }

        /// <summary>
        /// Invokes the Tick method on all registered timers.
        /// </summary>
        public void Tick()
        {
            for (int i = 0; i < m_timers.Count; i++)
            {
                if (m_timers[i] != null)
                {
                    try
                    {
                        m_timers[i].Tick();
                    }
                    catch (System.Exception e) // Don't let exceptions stop the other timers.
                    {
                        if (Debug.isDebugBuild) Debug.LogError(e.Message);
                    }
                }
            }
        }

    }
}