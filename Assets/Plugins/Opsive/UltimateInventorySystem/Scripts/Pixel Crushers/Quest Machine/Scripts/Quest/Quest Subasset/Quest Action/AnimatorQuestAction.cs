// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    public enum AnimatorControlAction { Play, SetTrigger, SetBool, SetFloat }

    /// <summary>
    /// Controls an animator.
    /// </summary>
    public class AnimatorQuestAction : QuestAction
    {

        #region Serialized Fields

        [Tooltip("Name of GameObject that has Animator.")]
        [SerializeField]
        private StringField m_gameObjectName;

        [Tooltip("How to control Animator.")]
        [SerializeField]
        private AnimatorControlAction m_action;

        [Tooltip("State or parameter to control.")]
        [SerializeField]
        private StringField m_target;

        [Tooltip("Boolean value.")]
        [SerializeField]
        private bool m_boolValue;

        [Tooltip("Float value.")]
        [SerializeField]
        private float m_floatValue;

        #endregion

        #region Public Properties

        /// <summary>
        /// Name of GameObject that has Animator.
        /// </summary>
        public StringField gameObjectName
        {
            get { return m_gameObjectName; }
            set { m_gameObjectName = value; }
        }

        /// <summary>
        /// How to control Animator.
        /// </summary>
        public AnimatorControlAction action
        {
            get { return m_action; }
            set { m_action = value; }
        }

        /// <summary>
        /// State or parameter to control.
        /// </summary>
        public StringField target
        {
            get { return m_target; }
            set { m_target = value; }
        }

        /// <summary>
        /// Boolean parameter value.
        /// </summary>
        public bool boolValue
        {
            get { return m_boolValue; }
            set { m_boolValue = value; }
        }

        /// <summary>
        /// Float parameter value.
        /// </summary>
        public float floatValue
        {
            get { return m_floatValue; }
            set { m_floatValue = value; }
        }

        #endregion

        public override string GetEditorName()
        {
            if (StringField.IsNullOrEmpty(gameObjectName)) return "Control Animator";
            switch (action)
            {
                case AnimatorControlAction.Play:
                    return "Animator on " + gameObjectName + ": Play state " + target;
                case AnimatorControlAction.SetTrigger:
                    return "Animator on " + gameObjectName + ": Set " + target;
                case AnimatorControlAction.SetBool:
                    return "Animator on " + gameObjectName + ": Set " + target + " to " + boolValue;
                case AnimatorControlAction.SetFloat:
                    return "Animator on " + gameObjectName + ": Set " + target + " to " + floatValue;
                default:
                    return "Animator on " + gameObjectName + ": Action not set yet";
            }
        }

        public override void Execute()
        {
            if (StringField.IsNullOrEmpty(gameObjectName) || StringField.IsNullOrEmpty(target)) return;
            var gameObject = GameObjectUtility.GameObjectHardFind(StringField.GetStringValue(gameObjectName));
            if (gameObject == null)
            {
                if (QuestMachine.debug) Debug.LogWarning("Quest Machine: AnimatorQuestAction can't find a GameObject named '" + gameObjectName + "'.");
                return;
            }
            var animator = gameObject.GetComponentInChildren<Animator>();
            if (animator == null)
            {
                if (QuestMachine.debug) Debug.LogWarning("Quest Machine: AnimatorQuestAction: " + gameObjectName + " doesn't have an Animator.");
                return;
            }
            switch (action)
            {
                case AnimatorControlAction.Play:
                    if (QuestMachine.debug) Debug.Log("Quest Machine: " + gameObjectName + ": Crossfading to state '" + target + "'.", gameObject);
                    animator.CrossFade(StringField.GetStringValue(target), floatValue);
                    break;
                case AnimatorControlAction.SetTrigger:
                    if (QuestMachine.debug) Debug.Log("Quest Machine: " + gameObjectName + ": Setting trigger '" + target + "'.", gameObject);
                    animator.SetTrigger(StringField.GetStringValue(target));
                    break;
                case AnimatorControlAction.SetBool:
                    if (QuestMachine.debug) Debug.Log("Quest Machine: " + gameObjectName + ": Setting '" + target + "' to " + boolValue, gameObject);
                    animator.SetBool(StringField.GetStringValue(target), boolValue);
                    break;
                case AnimatorControlAction.SetFloat:
                    if (QuestMachine.debug) Debug.Log("Quest Machine: " + gameObjectName + ": Setting '" + target + "' to " + floatValue, gameObject);
                    animator.SetFloat(StringField.GetStringValue(target), floatValue);
                    break;
            }

        }
    }
}
