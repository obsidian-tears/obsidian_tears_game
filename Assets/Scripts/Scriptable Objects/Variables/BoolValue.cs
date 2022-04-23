using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: change to BoolVariable
[CreateAssetMenu]
public class BoolValue : ScriptableObject
{
    private void OnEnable() => hideFlags = HideFlags.DontUnloadUnusedAsset;
    public bool value;
}
