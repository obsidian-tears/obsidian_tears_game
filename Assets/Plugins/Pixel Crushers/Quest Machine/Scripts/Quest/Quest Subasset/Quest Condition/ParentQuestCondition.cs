// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Quest condition that becomes true when a specified number of parent nodes are true.
    /// </summary>
    [Serializable]
    public class ParentQuestCondition : QuestCondition
    {

        [Tooltip("How many parents must be true.")]
        [SerializeField]
        private ConditionCountMode m_parentCountMode = ConditionCountMode.All;

        [Tooltip("If Parent Count Mode is Min, at least this many parents must be true.")]
        [SerializeField]
        private int m_minParentCount;

        /// <summary>
        /// How many parents must be true.
        /// </summary>
        public ConditionCountMode parentCountMode
        {
            get { return m_parentCountMode; }
            set { m_parentCountMode = value; }
        }

        /// <summary>
        /// If parentCountMode is ConditionCountMode.Min, at least this many parents must be true.
        /// </summary>
        public int minParentCount
        {
            get { return m_minParentCount; }
            set { m_minParentCount = value; }
        }

        public override string GetEditorName()
        {
            switch (parentCountMode)
            {
                case ConditionCountMode.All:
                    return "Parents: All True";
                case ConditionCountMode.Any:
                    return "Parents: Any True";
                case ConditionCountMode.Min:
                    return "Parents: At Least " + minParentCount + " True";
            }
            return base.GetEditorName();
        }

        public override void StartChecking(System.Action trueAction)
        {
            base.StartChecking(trueAction);
            ConnectToParentNodes(true);
            CheckTrueParentCount();
        }

        public override void StopChecking()
        {
            base.StopChecking();
            ConnectToParentNodes(false);
        }

        protected void ConnectToParentNodes(bool add)
        {
            if (QuestMachine.debug) Debug.Log("Quest Machine: ParentCountQuestCondition.ConnectToParentNodes(" + (add ? "listen for parent changes" : "stop listening for parent changes") + ")", quest);
            if (quest == null || quest.nodeList == null || questNode == null || questNode.parentList == null) return;
            for (int i = 0; i < questNode.parentList.Count; i++)
            {
                var parentNode = questNode.parentList[i];
                if (parentNode == null) continue;
                parentNode.stateChanged -= OnParentStateChange;
                if (add) parentNode.stateChanged += OnParentStateChange;
            }
        }

        protected void OnParentStateChange(QuestNode parentNode)
        {
            if (QuestMachine.debug) Debug.Log("Quest Machine: ParentCountQuestCondition.OnParentStateChange(" + ((parentNode != null) ? parentNode.GetEditorName() : "null") + ")", quest);
            var parentIsTrue = (parentNode != null && parentNode.GetState() == QuestNodeState.True);
            if (parentIsTrue) CheckTrueParentCount();
        }

        protected void CheckTrueParentCount()
        {
            // Count every time this method is called instead of maintaining a counter that we'd have to include in saved games.
            int nonoptionalCount;
            int optionalCount;
            int totalCount;
            CountTrueParents(QuestNodeState.True, out nonoptionalCount, out optionalCount, out totalCount);
            switch (parentCountMode)
            {
                case ConditionCountMode.Any:
                    if (totalCount >= 1) SetTrue();
                    break;
                case ConditionCountMode.All:
                    if (questNode == null || questNode.nonoptionalParentList == null) break;
                    if (nonoptionalCount >= questNode.nonoptionalParentList.Count) SetTrue();
                    break;
                case ConditionCountMode.Min:
                    if (totalCount >= minParentCount) SetTrue();
                    break;
                default:
                    if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: Unrecognized ConditionCountMode '" + parentCountMode + "'. Please contact the developer.", quest);
                    break;
            }
        }

        protected void CountTrueParents(QuestNodeState requiredState, out int nonoptionalCount, out int optionalCount, out int totalCount)
        {
            nonoptionalCount = 0;
            optionalCount = 0;
            if (questNode != null && questNode.parentList != null)
            {
                for (int i = 0; i < questNode.parentList.Count; i++)
                {
                    var parentNode = questNode.parentList[i];
                    if (parentNode == null) continue;
                    if (parentNode.GetState() != requiredState) continue;
                    if (parentNode.isOptional) optionalCount++; else nonoptionalCount++;
                }
            }
            totalCount = nonoptionalCount + optionalCount;
        }

    }

}
