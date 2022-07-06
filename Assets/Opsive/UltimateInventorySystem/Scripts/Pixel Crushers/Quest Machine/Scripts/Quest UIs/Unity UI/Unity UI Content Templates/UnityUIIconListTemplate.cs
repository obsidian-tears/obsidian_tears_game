// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Unity UI holder for icons.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class UnityUIIconListTemplate : UnityUIContainerTemplate
    {

        [Tooltip("Template used to instantiate individual icons.")]
        [SerializeField]
        private UnityUIIconTemplate m_iconTemplate;

        /// <summary>
        /// Template used to instantiate individual icons.
        /// </summary>
        public UnityUIIconTemplate iconTemplate
        {
            get { return m_iconTemplate; }
            set { m_iconTemplate = value; }
        }

        public virtual void Awake()
        {

            if (iconTemplate == null && Debug.isDebugBuild) Debug.LogError("Quest Machine: Icon Template is unassigned.", this);
            iconTemplate.gameObject.SetActive(false);
        }

        public UnityUIIconTemplate AddIcon(IconQuestContent icon)
        {
            var instance = Instantiate<UnityUIIconTemplate>(iconTemplate);
            AddInstanceToContainer(instance);
            instance.image.sprite = icon.image;
            instance.countText.text = icon.count.ToString();
            instance.countText.enabled = icon.count > 1;
            instance.captionText.text = icon.runtimeText;
            return instance;
        }

    }
}
