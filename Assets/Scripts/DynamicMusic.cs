using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicMusic : MonoBehaviour
{
    [SerializeField] AudioSource source;
    [SerializeField] Song song;
    [Range(0.0f, 1.0f)]
    [SerializeField] float finalVolume = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        EnterSong();
    }

    public void EnterSong() {
        source.volume = 0.0f;
        source.clip = song.value;
        source.Play();
        StartCoroutine(StartFade(source, 2.0f, finalVolume));
    }

    public void ExitSong()
    {
        source.volume = finalVolume;
        StartCoroutine(StartFade(source, 1.0f, 0.0f));
    }

    public static IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }
}
