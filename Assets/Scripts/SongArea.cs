using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongArea : MonoBehaviour
{
    [SerializeField] MySignal signal;
    [SerializeField] Song song;
    [SerializeField] AudioClip areaSong;
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            song.value = areaSong;
            signal.Raise();
        }
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            song.value = song.defaultValue; 
            signal.Raise();
        }
    }
}
