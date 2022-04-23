using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: change this to AudioController. Controllers can be scripts on empty game objects.
public class DynamicMusic : MonoBehaviour
{
    [SerializeField] AudioSource source;
    [SerializeField] Song song;
    // Start is called before the first frame update
    void Start()
    {
        UpdateSong();
    }

    // Update is called once per frame
    public void UpdateSong() {
        source.clip = song.value;
        source.Play();
    }
}
