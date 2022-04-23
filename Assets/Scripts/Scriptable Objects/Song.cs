using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: change to AudioVariable
[CreateAssetMenu(fileName = "New Audio", menuName = "Game/Song")]
public class Song : ScriptableObject
{
    public AudioClip value;
    public AudioClip defaultValue; // TODO: move default song functionality to a audio controller
}
