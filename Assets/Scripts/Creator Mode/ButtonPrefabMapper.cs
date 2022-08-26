using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPrefabMapper : MonoBehaviour
{
    [SerializeField] PrefabEditor pe;
    [SerializeField] PrefabIdMapper pim;
    [SerializeField] List<int> prefabIds;

    private int nextPrefabId;

    void Start() {
        NextPrefab();
    }

    public void OnClick() {
        pe.SetSelector(NextPrefab);
        //NextPrefab();
    }

    /// <summary>
    /// <c>NextPrefab</c> Sets a new random next prefab, and returns the current prefab. Also replaces child image with next prefab.
    /// </summary>
    public int NextPrefab() {
        int currentPrefabId = nextPrefabId;
        nextPrefabId = prefabIds[Random.Range(0, prefabIds.Count)];
        transform.Find("Image").GetComponent<Image>().sprite = pim.GetPrefabFromId(nextPrefabId).GetComponent<SpriteRenderer>().sprite;
        return currentPrefabId;
    }
}
