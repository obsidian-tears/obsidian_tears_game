// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

namespace PixelCrushers.DialogueSystem.MenuSystem
{

    /// <summary>
    /// Add this to the Menu System if you want to manage music tracks.
    /// </summary>
    public class MusicManager : MonoBehaviour
    {

        public AudioSource musicAudioSource;

        public AudioClip titleMusic;
        public AudioClip[] gameplayMusic;

        private float originalVolume = -1;
        private int titleScene = 0;

        private void Awake()
        {
            if (musicAudioSource == null) musicAudioSource = GetComponent<AudioSource>();
            if (musicAudioSource != null) originalVolume = musicAudioSource.volume;
            titleScene = SceneManager.GetActiveScene().buildIndex;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;                
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex == titleScene)
            {
                PlayTitleMusic();
            }
            else
            {
                var saveHelper = GetComponent<SaveHelper>();
                if (saveHelper != null && scene.name == saveHelper.firstGameplayScene)
                {
                    if (gameplayMusic != null && gameplayMusic.Length >= 1)
                    {
                        PlayGameplayMusic(0);
                    }
                    else
                    {
                        FadeOutMusic(1);
                    }
                }
            }
        }

        public void PlayTitleMusic()
        {
            PlayAudioClip(titleMusic);
        }

        public void PlayGameplayMusic(int index)
        {
            if (gameplayMusic == null) return;
            if (0 <= index && index < gameplayMusic.Length)
            {
                PlayAudioClip(gameplayMusic[index]);
            }
        }

        public void PlayAudioClip(AudioClip audioClip)
        {
            if (audioClip == null || musicAudioSource == null || !musicAudioSource.enabled) return;
            if (musicAudioSource.isPlaying && musicAudioSource.clip == audioClip) return;
            musicAudioSource.Stop();
            musicAudioSource.clip = audioClip;
            musicAudioSource.volume = originalVolume;
            if (musicAudioSource.enabled) musicAudioSource.Play();
        }

        public void StopMusic()
        {
            if (musicAudioSource == null || !musicAudioSource.isPlaying) return;
            musicAudioSource.Stop();
        }

        public void FadeOutMusic(float duration)
        {
            StartCoroutine(FadeOutCoroutine(duration));
        }

        private IEnumerator FadeOutCoroutine(float duration)
        {
            if (musicAudioSource == null) yield break;
            float startingVolume = musicAudioSource.volume;
            float remaining = duration;
            while (remaining > 0)
            {
                musicAudioSource.volume = (remaining / duration) * startingVolume;
                yield return null;
                remaining -= Time.deltaTime;
            }
            musicAudioSource.Stop();
            musicAudioSource.volume = originalVolume;
        }

    }
}