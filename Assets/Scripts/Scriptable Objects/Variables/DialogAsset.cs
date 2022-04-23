using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: change to TextAssetVariable
[CreateAssetMenu(fileName = "New TextAsset", menuName = "Game/TextAsset")]
public class DialogAsset : ScriptableObject
{
    public TextAsset value;
}
