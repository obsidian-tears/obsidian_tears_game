// Copyright (c) Pixel Crushers. All rights reserved.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PixelCrushers.QuestMachine
{

    [Serializable]
    public class QuestMachineSceneEvent
    {
        public string guid = string.Empty;
        public UnityEvent onExecute = new UnityEvent();
    }

    /// <summary>
    /// Holds scene-specific UnityEvents referenced by a dialogue database's dialogue entries.
    /// </summary>
    [AddComponentMenu("")]
    public class QuestMachineSceneEvents : MonoBehaviour
    {
        [HelpBox("Do not remove this GameObject. It contains UnityEvents referenced by Quest Machine quest actions. This GameObject should not be a child of the Dialogue Manager or marked as Don't Destroy On Load.", HelpBoxMessageType.Info)]
        public List<QuestMachineSceneEvent> sceneEvents = new List<QuestMachineSceneEvent>();

        private static QuestMachineSceneEvents m_sceneInstance = null;
        public static QuestMachineSceneEvents sceneInstance
        {
            get
            {
                if (m_sceneInstance == null)
                {
                    m_sceneInstance = FindObjectOfType<QuestMachineSceneEvents>();
                }
                return m_sceneInstance;
            }
            set
            {
                m_sceneInstance = value;
            }
        }

#if UNITY_2019_3_OR_NEWER && UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitStaticVariables()
        {
            m_sceneInstance = null;
        }
#endif

        private void Awake()
        {
            m_sceneInstance = this;
        }

        public static int AddNewSceneEvent(out string guid)
        {
            guid = string.Empty;
            if (sceneInstance == null) return -1;
            guid = Guid.NewGuid().ToString();
            var x = new QuestMachineSceneEvent();
            x.guid = guid;
            sceneInstance.sceneEvents.Add(x);
            return sceneInstance.sceneEvents.Count - 1;
        }

        public static void RemoveSceneEvent(string guid)
        {
            if (Application.isPlaying || sceneInstance == null) return;
            sceneInstance.sceneEvents.RemoveAll(x => x.guid == guid);
        }

        public static QuestMachineSceneEvent GetSceneEvent(string guid)
        {
            if (sceneInstance == null) return null;
            return sceneInstance.sceneEvents.Find(x => x.guid == guid);
        }

        public static int GetSceneEventIndex(string guid)
        {
            if (sceneInstance == null) return -1;
            return sceneInstance.sceneEvents.FindIndex(x => x.guid == guid);
        }

    }
}
