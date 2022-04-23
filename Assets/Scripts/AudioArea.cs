using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioArea : MonoBehaviour
{
    [SerializeField] MySignal audioSignal;
    [SerializeField] Song currentAudioClip; // TODO: change type of currentAudioClip to AudioClipVariable
    // TODO: put areaAudioClip in an AudioClipVariable (scriptable object) because we will be reusing clips 
    // for many areas and they are relatively large in size. this will save memory.
    [SerializeField] AudioClip areaAudioClip;
    
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            currentAudioClip.value = areaAudioClip;
            audioSignal.Raise();
        }
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            currentAudioClip.value = null; // TODO: in audio controller check for null then play default song for scene
            audioSignal.Raise();
        }
    }
}
