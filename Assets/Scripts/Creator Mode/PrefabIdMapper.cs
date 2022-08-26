using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabIdMapper : MonoBehaviour
{
    public List<GameObject> prefabs = new List<GameObject>();

    public GameObject GetPrefabFromId(int id) {
        return prefabs[id];
    }
}
