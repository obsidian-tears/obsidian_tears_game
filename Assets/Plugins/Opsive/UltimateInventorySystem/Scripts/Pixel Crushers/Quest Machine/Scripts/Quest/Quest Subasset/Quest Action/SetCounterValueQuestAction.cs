// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Sets a counter value.
    /// </summary>
    public class SetCounterValueQuestAction : QuestAction
    {

        [Tooltip("Index of counter to set.")]
        [SerializeField]
        private int m_counterIndex;

        public enum Operation { SetToValue, ModifyByValue, Randomize }

        [Tooltip("Set an absolute value or modify the current value?")]
        [SerializeField]
        private Operation m_operation;

        [Tooltip("Value to set the counter to, modify the counter by, or min value for Randomize.")]
        [SerializeField]
        private int m_operationValue;

        [Tooltip("Max value for Randomize.")]
        [SerializeField]
        private int m_maxValue;

        public int counterIndex
        {
            get { return m_counterIndex; }
            set { m_counterIndex = value; }
        }

        public Operation operation
        {
            get { return m_operation; }
            set { m_operation = value; }
        }

        public int operationValue
        {
            get { return m_operationValue; }
            set { m_operationValue = value; }
        }

        public int maxValue
        {
            get { return m_maxValue; }
            set { m_maxValue = value; }
        }

        public override string GetEditorName()
        {
            if (quest == null || quest.counterList == null || !(0 <= counterIndex && counterIndex < quest.counterList.Count)) return "Set Counter";
            var counterName = quest.counterList[counterIndex].name;
            switch (m_operation)
            {
                case Operation.SetToValue:
                    return "Set Counter: " + counterName + " = " + operationValue;
                case Operation.ModifyByValue:
                    return "Set Counter: " + counterName + " += " + operationValue;
                case Operation.Randomize:
                    return "Set Counter: " + counterName + " to random in [" + operationValue + "," + maxValue + "]";
                default:
                    return "Set Counter x";
            }
        }

        public override void Execute()
        {
            if (quest == null)
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: SetCounterQuestAction was passed a null quest.");
                return;
            }
            var counter = quest.GetCounter(counterIndex);
            if (counter == null)
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: SetCounterQuestAction can't find a counter at index " + counterIndex + " in quest '" + quest.GetEditorName() + "'.");
                return;
            }
            switch (m_operation)
            {
                case Operation.SetToValue:
                    counter.SetValue(operationValue);
                    break;
                case Operation.ModifyByValue:
                    counter.SetValue(counter.currentValue + operationValue);
                    break;
                case Operation.Randomize:
                    counter.SetValue(Random.Range(operationValue, maxValue + 1));
                    break;
            }
        }

    }

}
