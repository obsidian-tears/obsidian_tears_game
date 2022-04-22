// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// AudioClip UI content.
    /// </summary>
    public class AudioClipQuestContent : QuestContent
    {

        [Tooltip("Audio clip to play.")]
        [SerializeField]
        private AudioClip m_audioClip;

        [Tooltip("Audio source to use.")]
        [SerializeField]
        private QuestAudioSourceIdentifier m_useAudioSourceOn = new QuestAudioSourceIdentifier();

        [HideInInspector]
        [SerializeField]
        private string m_audioNameProxy; // Temporary variable for proxy serialization.

        /// <summary>
        /// Audio clip to play.
        /// </summary>
        public AudioClip audioClip
        {
            get { return m_audioClip; }
            set { m_audioClip = value; }
        }

        /// <summary>
        /// Identifies the audio source to use.
        /// </summary>
        public QuestAudioSourceIdentifier useAudioSourceOn
        {
            get { return m_useAudioSourceOn; }
            set { m_useAudioSourceOn = value; }
        }

        public override string GetEditorName()
        {
            return (audioClip == null) ? "Audio Clip" : "Audio Clip: " + audioClip.name;
        }

        public override void OnBeforeProxySerialization()
        {
            base.OnBeforeProxySerialization();
            m_audioNameProxy = QuestMachine.GetAudioClipPath(m_audioClip);
        }

        public override void OnAfterProxyDeserialization()
        {
            base.OnAfterProxyDeserialization();
            m_audioClip = QuestMachine.GetAudioClip(m_audioNameProxy);
            m_audioNameProxy = null; // Free memory.
        }

        public override AudioClip[] GetAudioClips()
        {
            return new AudioClip[] { audioClip };
        }

    }

}
