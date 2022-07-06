// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// References a literal value or quest counter value.
    /// </summary>
    [Serializable]
    public class QuestNumber
    {

        public enum ValueType { Literal, CounterValue, CounterMinValue, CounterMaxValue }

        [Tooltip("The source of this number's value.")]
        [SerializeField]
        private ValueType m_valueType = ValueType.Literal;

        [Tooltip("Literal value.")]
        [SerializeField]
        private int m_literalValue;

        [Tooltip("A counter defined in the quest. Inspect the quest's main info to view/edit counters.")]
        [SerializeField]
        private int m_counterIndex;

        /// <summary>
        /// The source of this number's value.
        /// </summary>
        public ValueType valueType
        {
            get { return m_valueType; }
            set { m_valueType = value; }
        }

        /// <summary>
        /// Literal value.
        /// </summary>
        public int literalValue
        {
            get { return m_literalValue; }
            set { m_literalValue = value; }
        }

        /// <summary>
        /// A counter defined in the quest.
        /// </summary>
        public int counterIndex
        {
            get { return m_counterIndex; }
            set { m_counterIndex = value; }
        }

        /// <summary>
        /// Returns the current value of this quest number.
        /// </summary>
        /// <param name="quest">The quest to which this quest number pertains.</param>
        public int GetValue(Quest quest)
        {
            switch (valueType)
            {
                default:
                    return literalValue;
                case ValueType.CounterValue:
                case ValueType.CounterMinValue:
                case ValueType.CounterMaxValue:
                    if (quest == null)
                    {
                        if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: Want to get value of counter at index " + counterIndex + " but quest is null.");
                        return 0;
                    }
                    var counter = quest.GetCounter(counterIndex);
                    if (counter == null)
                    {
                        if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: There is no counter at index " + counterIndex + "' in quest '" + quest.GetEditorName() + ".", quest);
                        return 0;
                    }
                    switch (valueType)
                    {
                        default:
                        case ValueType.CounterValue:
                            return counter.currentValue;
                        case ValueType.CounterMinValue:
                            return counter.minValue;
                        case ValueType.CounterMaxValue:
                            return counter.maxValue;
                    }
            }
        }

        public QuestNumber() { }

        /// <summary>
        /// Creates a quest number corresponding to a literal integer value.
        /// </summary>
        /// <param name="literalValue"></param>
        public QuestNumber(int literalValue)
        {
            m_valueType = ValueType.Literal;
            m_literalValue = literalValue;
        }

        /// <summary>
        /// Creates a quest number corresponding to the current counter value of a quest counter.
        /// </summary>
        /// <param name="quest">Quest containing the counter.</param>
        /// <param name="counterName">Name of the counter.</param>
        public QuestNumber(Quest quest, string counterName)
        {
            m_valueType = ValueType.CounterValue;
            m_counterIndex = (quest != null) ? quest.GetCounterIndex(counterName) : -1;
        }

        /// <summary>
        /// Creates a quest number corresponding to a specified type of value of a quest counter.
        /// </summary>
        /// <param name="quest">Quest containing the counter.</param>
        /// <param name="counterName">Name of the counter.</param>
        /// <param name="valueType">The value type (current, min, or max).</param>
        public QuestNumber(Quest quest, StringField counterName, ValueType valueType)
        {
            m_valueType = valueType;
            m_counterIndex = (quest != null) ? quest.GetCounterIndex(counterName) : -1;
        }

        public string GetEditorName(Quest quest)
        {
            if (valueType == ValueType.Literal) return literalValue.ToString();
            var counter = (quest != null) ? quest.GetCounter(counterIndex) : null;
            var counterName = (counter != null) ? StringField.GetStringValue(counter.name) : ("counter #" + counterIndex);
            switch (valueType)
            {
                default:
                case ValueType.CounterValue:
                    return counterName;
                case ValueType.CounterMinValue:
                    return counterName + " Min Value";
                case ValueType.CounterMaxValue:
                    return counterName + " Max Value";
            }
        }

    }
}
