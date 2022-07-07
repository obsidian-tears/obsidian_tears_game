// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Unity UI template for an icon.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class UnityUIIconTemplate : UnityUIContentTemplate
    {

        [Tooltip("Image UI element for icon's image.")]
        [SerializeField]
        private UnityEngine.UI.Image m_image;

        [Tooltip("Text UI element for count. Shown if icon's count is greater than 1.")]
        [SerializeField]
        private UITextField m_countText;

        [Tooltip("Text UI element for icon's caption.")]
        [SerializeField]
        private UITextField m_captionText;

        /// <summary>
        /// Image UI element for icon's image.
        /// </summary>
        public UnityEngine.UI.Image image
        {
            get { return m_image; }
            set { m_image = value; }
        }

        /// <summary>
        /// Text UI element for count. Shown if icon's count is greater than 1.
        /// </summary>
        public UITextField countText
        {
            get { return m_countText; }
            set { m_countText = value; }
        }

        /// <summary>
        /// Text UI element for icon's caption.
        /// </summary>
        public UITextField captionText
        {
            get { return m_captionText; }
            set { m_captionText = value; }
        }

        public virtual void Awake()
        {
            //--- No warning. These are optional:
            //if (image == null && Debug.isDebugBuild) Debug.LogError("Quest Machine: UI Image is unassigned.", this);
            //if (countText == null && Debug.isDebugBuild) Debug.LogError("Quest Machine: Count Text is unassigned.", this);
            //if (captionText == null && Debug.isDebugBuild) Debug.LogError("Quest Machine: Caption Text is unassigned.", this);
        }

        public virtual void Assign(Sprite sprite, int count, string caption)
        {
            if (image != null) image.sprite = sprite;
            if (countText != null)
            {
                countText.text = count.ToString();
                countText.enabled = count > 1;
            }
            if (captionText != null) captionText.text = caption;
        }

    }

}
