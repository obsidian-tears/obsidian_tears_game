// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    public enum UrgentFactionSelectionCriterion
    {
        SameAsGlobalSetting,
        Weighted,
        WeightedSquared
    }

    /// <summary>
    /// Specifies the method to use to select the most urgent fact(s).
    /// The quest generator will generate a quest to address one of these most-urgent facts.
    /// </summary>
    [Serializable]
    public struct UrgentFactSelectionMode
    {
        [Tooltip("How to weight urgencies of facts in world model. Higher urgency facts are more likely to get quests.")]
        [SerializeField]
        private UrgentFactionSelectionCriterion m_criterion;

        [Tooltip("Number of top-urgency facts to consider when choosing a fact to create a quest about.")]
        [SerializeField]
        private int m_max;

        public UrgentFactionSelectionCriterion criterion
        {
            get { return m_criterion; }
            set { m_criterion = value; }
        }

        public int max
        {
            get { return m_max; }
            set { m_max = value; }
        }

        public UrgentFactSelectionMode(UrgentFactionSelectionCriterion criterion, int max)
        {
            m_criterion = criterion;
            m_max = max;
        }

        public float AdjustUrgency(float urgency)
        {
            switch (criterion)
            {
                default:
                case UrgentFactionSelectionCriterion.Weighted:
                    return urgency;
                case UrgentFactionSelectionCriterion.WeightedSquared:
                    return urgency * urgency;
                case UrgentFactionSelectionCriterion.SameAsGlobalSetting:
                    var globalCriterion = (QuestMachineConfiguration.instance != null) ? QuestMachineConfiguration.instance.generatorSettings.goalSelectionCriterion : mostUrgent;
                    return (globalCriterion.criterion == UrgentFactionSelectionCriterion.WeightedSquared) ? (urgency * urgency) : urgency;
            }
        }

        public static UrgentFactSelectionMode mostUrgent = new UrgentFactSelectionMode(UrgentFactionSelectionCriterion.Weighted, 1);
    }

}