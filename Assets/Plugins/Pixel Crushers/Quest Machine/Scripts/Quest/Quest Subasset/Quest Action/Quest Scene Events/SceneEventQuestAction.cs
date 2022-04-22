// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Activates a GameObject.
    /// </summary>
    public class SceneEventQuestAction : QuestAction
    {

        [SerializeField]
        [HideInInspector]
        private string m_guid;

        /// <summary>
        /// GUID in a scene's QuestMachineSceneEvents component.
        /// </summary>
        public string guid
        {
            get { return m_guid; }
            set { m_guid = value; }
        }

        public override string GetEditorName()
        {
            return "Scene Event (" + guid + ")";
        }

        public override void Execute()
        {
            if (string.IsNullOrEmpty(guid)) return;
            var sceneEvent = QuestMachineSceneEvents.GetSceneEvent(guid);
            if (sceneEvent == null)
            {
                if (QuestMachine.debug) Debug.LogWarning("Quest Machine: Can't find scene event with GUID " + guid + ". It may be defined in a different scene's Quest Machine Scene Events component.");
            }
            else
            {
                if (QuestMachine.debug) Debug.LogWarning("Quest Machine: Invoke scene event with GUID " + guid);
                sceneEvent.onExecute.Invoke();
            }
        }

    }

}
