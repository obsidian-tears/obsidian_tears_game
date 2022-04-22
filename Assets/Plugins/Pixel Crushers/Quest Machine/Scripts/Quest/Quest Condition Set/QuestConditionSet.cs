// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Manages a set of conditions, invoking a delegate when true.
    /// </summary>
    [Serializable]
    public class QuestConditionSet
    {

        #region Serialized Fields

        [Tooltip("Conditions in this condition set.")]
        [SerializeField]
        private List<QuestCondition> m_conditionList = new List<QuestCondition>();

        [Tooltip("How many conditions need to be true.")]
        [SerializeField]
        private ConditionCountMode m_conditionCountMode;

        [Tooltip("If the Condition Count Mode is Min, at least this many conditions must be true.")]
        [SerializeField]
        private int m_minConditionCount;

        [HideInInspector]
        [SerializeField]
        private int m_numTrueConditions = 0;

        #endregion

        #region Accessors Properties for Serialized Fields

        /// <summary>
        /// Conditions in this condition set.
        /// </summary>
        public List<QuestCondition> conditionList
        {
            get { return m_conditionList; }
            set { m_conditionList = value; }
        }

        /// <summary>
        /// How many conditions need to be true for the connection to be true.
        /// </summary>
        public ConditionCountMode conditionCountMode
        {
            get { return m_conditionCountMode; }
            set { m_conditionCountMode = value; }
        }

        /// <summary>
        /// If the Condition Count Mode is Min, at least this many conditions must be true.
        /// </summary>
        public int minConditionCount
        {
            get { return m_minConditionCount; }
            set { m_minConditionCount = value; }
        }

        /// <summary>
        /// The number of conditions that have reported true.
        /// </summary>
        public int numTrueConditions
        {
            get { return m_numTrueConditions; }
            set { m_numTrueConditions = value; }
        }

        #endregion

        #region Private Fields

        private System.Action m_trueAction = delegate { };

        private bool m_isChecking = false;

        #endregion

        #region Initialization and Destruction

        public void SetRuntimeReferences(Quest quest, QuestNode questNode)
        {
            if (conditionList == null) return;
            for (int i = 0; i < conditionList.Count; i++)
            {
                if (conditionList[i] != null) conditionList[i].SetRuntimeReferences(quest, questNode);
            }
        }

        public void CloneSubassetsInto(QuestConditionSet copy)
        {
            if (copy == null)
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: QuestConditionSet.CloneSubassetsInto() failed because copy is invalid.");
                return;
            }
            copy.conditionList = QuestSubasset.CloneList(conditionList);
        }

        public void DestroySubassets()
        {
            QuestSubasset.DestroyList(conditionList);
        }

        #endregion

        #region Condition Checking

        /// <summary>
        /// Starts checking conditions. Does not reset true condition count
        /// because it may have been restored from a saved game.
        /// </summary>
        /// <param name="trueAction"></param>
        public void StartChecking(System.Action trueAction)
        {
            if (m_isChecking || conditionList == null) return;
            m_isChecking = true;
            m_trueAction = trueAction;
            for (int i = 0; i < conditionList.Count; i++)
            {
                //---Suppress warning: UnityEngine.Assertions.Assert.IsNotNull(conditionList[i], "Quest Machine: conditionList element " + i + " is null. Does your Conditions list have an invalid entry?");
                if (conditionList[i] != null) conditionList[i].StartChecking(OnTrueCondition);
            }
        }

        /// <summary>
        /// Stops checking conditions.
        /// </summary>
        public void StopChecking()
        {
            if (!m_isChecking || conditionList == null) return;
            for (int i = 0; i < conditionList.Count; i++)
            {
                //--- Don't assert; may be null because application is quitting:
                // UnityEngine.Assertions.Assert.IsNotNull(conditionList[i], "Quest Machine: conditionList element " + i + " is null. Does your Conditions list have an invalid entry?");
                if (conditionList[i] != null) conditionList[i].StopChecking();
            }
            m_isChecking = false;
        }

        /// <summary>
        /// True if the conditions are met.
        /// </summary>
        public bool areConditionsMet
        {
            get
            {
                if (conditionList == null || conditionList.Count == 0) return true;
                switch (conditionCountMode)
                {
                    case ConditionCountMode.All:
                        return (numTrueConditions >= conditionList.Count);
                    case ConditionCountMode.Any:
                        return (numTrueConditions > 0);
                    case ConditionCountMode.Min:
                        return (numTrueConditions >= minConditionCount);
                    default:
                        if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: Internal error. Unrecognized condition count mode '" + conditionCountMode + "'. Please contact the developer.");
                        return false;
                }
            }
        }

        private void OnTrueCondition()
        {
            numTrueConditions++;
            if (areConditionsMet) SetTrue();
        }

        private void SetTrue()
        {
            m_trueAction();
        }

        public static int ConditionCount(QuestConditionSet conditionSet)
        {
            return (conditionSet != null && conditionSet.conditionList != null) ? conditionSet.conditionList.Count : 0;
        }

        #endregion

    }

}
