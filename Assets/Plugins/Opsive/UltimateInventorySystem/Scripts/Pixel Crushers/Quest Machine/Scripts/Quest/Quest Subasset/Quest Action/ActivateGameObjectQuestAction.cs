// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Activates a GameObject.
    /// </summary>
    public class ActivateGameObjectQuestAction : QuestAction
    {

        [Tooltip("Name of GameObject to activate or deactivate.")]
        [SerializeField]
        private StringField m_gameObjectName;

        [Tooltip("Tick to activate, untick to deactivate.")]
        [SerializeField]
        private bool m_state;

        /// <summary>
        /// Name of GameObject to activate or deactivate.
        /// </summary>
        public StringField gameObjectName
        {
            get { return m_gameObjectName; }
            set { m_gameObjectName = value; }
        }

        /// <summary>
        /// True to activate, false to deactivate.
        /// </summary>
        public bool state
        {
            get { return m_state; }
            set { m_state = value; }
        }

        public override string GetEditorName()
        {
            if (StringField.IsNullOrEmpty(gameObjectName)) return "Activate GameObject";
            return (state ? "Activate" : "Deactivate") + " GameObject: '" + gameObjectName + "'";
        }

        public override void Execute()
        {
            if (StringField.IsNullOrEmpty(gameObjectName)) return;
            var gameObject = GameObjectUtility.GameObjectHardFind(StringField.GetStringValue(gameObjectName));
            if (gameObject == null)
            {
                if (QuestMachine.debug) Debug.LogWarning("Quest Machine: ActivateGameObjectQuestAction can't find a GameObject named '" + gameObjectName + "'.");
                return;
            }
            if (QuestMachine.debug) Debug.Log("Quest Machine: Setting GameObject '" + gameObjectName + "' " + (state ? "active." : "inactive."));
            gameObject.SetActive(state);
        }

    }

}
