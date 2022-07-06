// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Unity UI template for text.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class UnityUITextTemplate : UnityUIContentTemplate
    {

        [Tooltip("Text UI element.")]
        [SerializeField]
        private UITextField m_text;

        /// <summary>
        /// Text UI element.
        /// </summary>
        public UITextField text
        {
            get { return m_text; }
            set { m_text = value; }
        }

        public virtual void Awake()
        {
            if (UITextField.IsNull(text) && Debug.isDebugBuild) Debug.LogError("Quest Machine: UI Text is unassigned.", this);
        }

        /// <summary>
        /// Assigns a text string to the UI element.
        /// </summary>
        /// <param name="text">Text string.</param>
        public void Assign(string text)
        {
            name = text;
            this.text.text = text;
        }

    }
}
