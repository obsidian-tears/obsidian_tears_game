using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabButtonMapper : MonoBehaviour
{
    [SerializeField]
    public PrefabDropper pd;
    public GameObject prefab;

    // TODO: Allow multiple GameObjects for random selection "vareties"

    public void OnClick() {
        pd.Select(prefab);
    }
}
