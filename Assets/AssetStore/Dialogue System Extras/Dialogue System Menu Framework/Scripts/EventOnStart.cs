// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.Events;

namespace PixelCrushers.DialogueSystem.MenuSystem
{

    /// <summary>
    /// Runs a UnityEvent on start, and opt5ion
    /// </summary>
    public class EventOnStart : MonoBehaviour
    {

        [Tooltip("If not -1, play the Music Manager's music associated with this index.")]
        public int musicIndex = -1;

        public UnityEvent onStart = new UnityEvent();

        void Start()
        {
            if (musicIndex != -1)
            {
                var musicManager = FindObjectOfType<MusicManager>();
                if (musicManager != null) musicManager.PlayGameplayMusic(musicIndex);
            }
            onStart.Invoke();
        }

    }
}