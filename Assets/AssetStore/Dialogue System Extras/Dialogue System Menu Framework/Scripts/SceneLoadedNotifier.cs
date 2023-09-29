// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.Events;
using System;

namespace PixelCrushers.DialogueSystem.MenuSystem
{

    /// <summary>
    /// This script provides an event to which other scripts can subscribe to 
    /// be notified when a scene has been loaded.
    /// </summary>
    public class SceneLoadedNotifier : MonoBehaviour
    {

        [Serializable]
        public class IntUnityEvent : UnityEvent<int> { }

        public IntUnityEvent onSceneLoaded = new IntUnityEvent();

        private bool m_started = false;

        private void Start()
        {
            m_started = true;
        }

#if UNITY_5_4_OR_NEWER
        private void OnEnable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            if (m_started) onSceneLoaded.Invoke(scene.buildIndex);
        }
#else
        private void OnLevelWasLoaded(int index)
        {
            if (m_started) onSceneLoaded.Invoke(index);
        }
#endif

   }
}