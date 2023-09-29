using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongArea : MonoBehaviour
{
    [SerializeField] public MySignal enterSignal;
    [SerializeField] public MySignal exitSignal;

    [SerializeField] public Song song;
    [SerializeField] public AudioClip areaSong;
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            song.value = areaSong;
            enterSignal.Raise();
        }
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            song.value = null;
            exitSignal.Raise();
        }
    }
}
