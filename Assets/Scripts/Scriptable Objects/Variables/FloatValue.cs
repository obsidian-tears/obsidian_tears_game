using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: change to FloatVariable
[CreateAssetMenu]
public class FloatValue : ScriptableObject
{
    private void OnEnable() => hideFlags = HideFlags.DontUnloadUnusedAsset;
    public float value;
}
