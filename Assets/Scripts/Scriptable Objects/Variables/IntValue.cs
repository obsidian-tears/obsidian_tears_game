using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: change to IntVariable
[CreateAssetMenu]
public class IntValue : ScriptableObject
{
    private void OnEnable() => hideFlags = HideFlags.DontUnloadUnusedAsset;
    public int value;
}
