// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    public delegate void ToggleChangedDelegate(bool value, object data);

    /// <summary>
    /// Unity UI template for a toggle.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class UnityUIToggleTemplate : UnityUIContentTemplate
    {

        [Tooltip("Toggle UI element.")]
        [SerializeField]
        private UnityEngine.UI.Toggle m_toggle;

        /// <summary>
        /// Toggle UI element.
        /// </summary>
        public UnityEngine.UI.Toggle toggle
        {
            get { return m_toggle; }
            set { m_toggle = value; }
        }

        protected object m_data;

        public event ToggleChangedDelegate onToggleChanged = delegate { };

        public virtual void Awake()
        {
            if (toggle == null && Debug.isDebugBuild) Debug.LogError("Quest Machine: UI Toggle is unassigned.", this);
        }

        public virtual void Assign(bool isVisible, bool isOn, object data, ToggleChangedDelegate toggleDelegate)
        {
            m_data = data;
            if (toggle != null)
            {
                if (isVisible)
                {
                    toggle.isOn = isOn;
                    toggle.onValueChanged.AddListener(OnToggleChanged);
                    onToggleChanged += toggleDelegate;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }

        protected virtual void OnToggleChanged(bool value)
        {
            try
            {
                onToggleChanged.Invoke(value, m_data);
            }
            catch (Exception e) // Don't let exceptions in user-added events break our code.
            {
                if (Debug.isDebugBuild) Debug.LogException(e);
            }
        }

    }
}
