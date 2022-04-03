using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Song", menuName = "Game/Song")]
public class Song : ScriptableObject
{
    public AudioClip value;
    public AudioClip defaultValue;
}
