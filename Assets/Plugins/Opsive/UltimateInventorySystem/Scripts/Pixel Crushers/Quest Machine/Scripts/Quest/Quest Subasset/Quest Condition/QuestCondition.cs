// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Abstract base class for quest conditions.
    /// </summary>
    public abstract class QuestCondition : QuestSubasset
    {

        /// <summary>
        /// Delegate to call when the condition becomes true.
        /// </summary>
        protected System.Action trueAction = delegate { };

        private bool m_isChecking = false;

        /// <summary>
        /// True if the condition is currently monitoring the requirements that would make it true.
        /// </summary>
        protected virtual bool isChecking
        {
            get { return m_isChecking; }
            set { m_isChecking = value; }
        }

        public override void SetRuntimeReferences(Quest quest, QuestNode questNode)
        {
            base.SetRuntimeReferences(quest, questNode);
            isChecking = false;
        }

        /// <summary>
        /// Tells the condition to start checking; when true, call SetTrue().
        /// </summary>
        /// <param name="trueAction">The method to invoke when the condition becomes true.</param>
        public virtual void StartChecking(System.Action trueAction)
        {
            isChecking = true;
            this.trueAction = trueAction;
        }

        /// <summary>
        /// Tells the condition to stop checking.
        /// </summary>
        public virtual void StopChecking()
        {
            isChecking = false;
        }

        /// <summary>
        /// Sets the condition true, invoking the trueAction.
        /// Also stops checking.
        /// </summary>
        public virtual void SetTrue()
        {
            if (QuestMachine.debug) Debug.Log("Quest Machine: " + GetType().Name + ".SetTrue()", quest);
            StopChecking();
            trueAction();
        }

    }

}
