// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Unity UI holder for buttons.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class UnityUIButtonListTemplate : UnityUIContainerTemplate
    {

        [Tooltip("Template used to instantiate individual buttons.")]
        [SerializeField]
        private UnityUIButtonTemplate m_buttonTemplate;

        /// <summary>
        /// Template used to instantiate individual buttons.
        /// </summary>
        public UnityUIButtonTemplate buttonTemplate
        {
            get { return m_buttonTemplate; }
            set { m_buttonTemplate = value; }
        }

        public virtual void Awake()
        {
            if (buttonTemplate == null && Debug.isDebugBuild) Debug.LogError("Quest Machine: Button Template is unassigned.", this);
            buttonTemplate.gameObject.SetActive(false);
        }

        protected UnityUIButtonTemplate CreateInstance()
        {
            var instance = Instantiate<UnityUIButtonTemplate>(buttonTemplate);
            AddInstanceToContainer(instance);
            return instance;
        }

        public UnityUIButtonTemplate AddButton(ButtonQuestContent button)
        {
            var instance = CreateInstance();
            instance.Assign(button.image, button.count, button.runtimeText, button.actionList);
            instance.groupNumber = button.groupNumber;
            return instance;
        }

        public UnityUIButtonTemplate AddButton(Sprite image, int count, string caption, UnityAction unityAction)
        {
            var instance = CreateInstance();
            instance.Assign(image, count, caption, unityAction);
            return instance;
        }

    }
}
