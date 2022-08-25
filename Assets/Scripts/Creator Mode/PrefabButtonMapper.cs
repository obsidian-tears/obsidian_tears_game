using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrefabButtonMapper : MonoBehaviour
{
    [SerializeField]
    public PrefabDropper pd;
    public List<GameObject> prefabs;

    private GameObject nextPrefab;

    void Start() {
        SetNextPrefab();
    }

    public void OnClick() {
        pd.SetSelector(SetNextPrefab);
        SetNextPrefab();
    }

    /// <summary>
    /// <c>SetNextPrefab</c> Sets a new random next prefab, and returns the current prefab. Also replaces child image with next prefab.
    /// </summary>
    public GameObject SetNextPrefab() {
        GameObject currentPrefab = nextPrefab;
        nextPrefab = prefabs[Random.Range(0, prefabs.Count)];
        transform.Find("Image").GetComponent<Image>().sprite = nextPrefab.GetComponent<SpriteRenderer>().sprite;
        return currentPrefab;
    }
}
