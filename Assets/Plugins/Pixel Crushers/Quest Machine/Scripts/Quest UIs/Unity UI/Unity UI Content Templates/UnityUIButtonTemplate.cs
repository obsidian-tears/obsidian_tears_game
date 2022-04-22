// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Unity UI template for a button.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class UnityUIButtonTemplate : UnityUIIconTemplate
    {

        [Tooltip("Button UI element.")]
        [SerializeField]
        private UnityEngine.UI.Button m_button;

        /// <summary>
        /// Button UI element.
        /// </summary>
        public UnityEngine.UI.Button button
        {
            get { return m_button; }
            set { m_button = value; }
        }

        private List<QuestAction> m_actions;
        protected List<QuestAction> actions
        {
            get { return m_actions; }
            set { m_actions = value; }
        }

        private int m_groupNumber = ButtonQuestContent.NoGroup;
        /// <summary>
        /// Group number this button belongs to, or -1 if none. When one button in the group is clicked, the other buttons become non-interactable.
        /// </summary>
        public int groupNumber
        {
            get { return m_groupNumber; }
            set { m_groupNumber = value; }
        }

        public override void Awake()
        {
            base.Awake();
            if (button == null && Debug.isDebugBuild) Debug.LogError("Quest Machine: UI Button is unassigned.", this);
        }

        public virtual void Assign(Sprite sprite, int count, string caption, List<QuestAction> actions)
        {
            base.Assign(sprite, count, caption);
            this.actions = actions;
            button.onClick.RemoveAllListeners();
            if (actions != null) button.onClick.AddListener(ExecuteActions);
        }

        public virtual void Assign(Sprite sprite, string caption, List<QuestAction> actions)
        {
            Assign(sprite, 1, caption, actions);
        }

        public virtual void Assign(Sprite sprite, int count, string caption, UnityAction unityAction)
        {
            Assign(sprite, count, caption);
            if (unityAction != null)
            {
                button.onClick.AddListener(unityAction);
            }
            else
            {
                button.interactable = false;
            }
        }

        public virtual void Assign(Sprite sprite, string caption, UnityAction unityAction)
        {
            Assign(sprite, 1, caption, unityAction);
        }

        protected virtual void ExecuteActions()
        {
            if (actions == null) return;
            for (int i = 0; i < actions.Count; i++)
            {
                if (actions[i] != null) actions[i].Execute();
            }

            if (groupNumber != ButtonQuestContent.NoGroup)
            {
                MessageSystem.SendMessage(this, QuestMachineMessages.GroupButtonClickedMessage, string.Empty, groupNumber);
            }
        }

    }
}
