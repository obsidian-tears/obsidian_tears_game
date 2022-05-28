// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Saves the state of the QuestIndicatorManager on the same GameObject.
    /// Generally not needed on QuestGivers, but can be useful for other objects
    /// that have quest indicators.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    [RequireComponent(typeof(QuestIndicatorManager))]
    public class QuestIndicatorSaver : Saver
    {

        [Serializable]
        public class QuestsData
        {
            public string[] quests;
        }

        [Serializable]
        public class Data
        {
            public QuestsData[] states;
        }

        private QuestIndicatorManager m_questIndicatorManager;

        public override void Awake()
        {
            base.Awake();
            m_questIndicatorManager = GetComponent<QuestIndicatorManager>();
        }

        public override string RecordData()
        {
            var data = new Data();
            var numStates = m_questIndicatorManager.states.Length;
            data.states = new QuestsData[numStates];
            for (int i = 0; i < numStates; i++)
            {
                var numQuests = m_questIndicatorManager.states[i].Count;
                data.states[i] = new QuestsData();
                data.states[i].quests = m_questIndicatorManager.states[i].ToArray();
            }
            return SaveSystem.Serialize(data);
        }

        public override void ApplyData(string s)
        {
            if (string.IsNullOrEmpty(s)) return;
            var data = SaveSystem.Deserialize<Data>(s);
            if (data == null || data.states == null) return;
            var numStates = Mathf.Max(data.states.Length, Enum.GetNames(typeof(QuestIndicatorState)).Length);
            var states = new List<string>[numStates];
            for (int i = 0; i < numStates; i++)
            {

                states[i] = (i < data.states.Length) ? new List<string>(data.states[i].quests) : new List<string>();
            }
            m_questIndicatorManager.states = states;
            m_questIndicatorManager.ShowHighestPriorityIndicator();
        }
    }

}
