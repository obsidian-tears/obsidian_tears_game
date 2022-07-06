// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    public enum QuestAudioSourceIdentifierType
    {
        /// <summary>
        /// Use the audio source on the current main camera.
        /// </summary>
        MainCamera,

        /// <summary>
        /// Use the audio source on the current Quest Machine Configuration instance.
        /// </summary>
        QuestMachine,

        /// <summary>
        /// Use the audio source on the GameObject with the specified tag.
        /// </summary>
        GameObjectWithTag,

        /// <summary>
        /// Use the audio source on the GameObject with the specified name.
        /// </summary>
        GameObjectWithName
    }

    /// <summary>
    /// Specifies which audio source to use to play audio.
    /// </summary>
    [Serializable]
    public class QuestAudioSourceIdentifier
    {

        [Tooltip("How to identify the audio source.")]
        [SerializeField]
        private QuestAudioSourceIdentifierType m_type = QuestAudioSourceIdentifierType.MainCamera;

        [Tooltip("Tag or GameObject name.")]
        [SerializeField]
        private string m_id = string.Empty;

        /// <summary>
        /// How to identify the audio source.
        /// </summary>
        public QuestAudioSourceIdentifierType type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        /// <summary>
        /// Tag or GameObject name.
        /// </summary>
        public string id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        /// <summary>
        /// Play a one shot audio clip through the specified audio source.
        /// </summary>
        /// <param name="audioClip"></param>
        public void Play(AudioClip audioClip)
        {
            var audioSource = FindAudioSource();
            if (audioSource != null) audioSource.PlayOneShot(audioClip);
        }

        private AudioSource FindAudioSource()
        {
            var go = FindAudioSourceGameObject();
            if (go == null) return null;
            var audioSource = go.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = go.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.loop = false;
            }
            return audioSource;
        }

        private GameObject FindAudioSourceGameObject()
        { 
            switch (type)
            {
                default:
                case QuestAudioSourceIdentifierType.MainCamera:
                    return (Camera.main != null) ? Camera.main.gameObject : null;
                case QuestAudioSourceIdentifierType.QuestMachine:
                    return (QuestMachineConfiguration.instance != null) ? QuestMachineConfiguration.instance.gameObject : null;
                case QuestAudioSourceIdentifierType.GameObjectWithTag:
                    return GameObject.FindGameObjectWithTag(id);
                case QuestAudioSourceIdentifierType.GameObjectWithName:
                    return GameObject.Find(id);
            }
        }
    }

}
