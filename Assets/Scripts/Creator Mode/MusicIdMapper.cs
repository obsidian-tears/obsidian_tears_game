using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicIdMapper : MonoBehaviour
{
    public List<AudioClip> audioClips = new List<AudioClip>();

    public AudioClip GetMusicFromId(int id)
    {
        return audioClips[id];
    }
}
