using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPrefabMapper : MonoBehaviour
{
    [SerializeField] GameObjectPlacer prefabEditor;
    [SerializeField] GameObjectIdMapper prefabIdMapper;
    [SerializeField] public List<int> prefabIds;
    [SerializeField] string itemNameIfChest;

    private int nextPrefabId;

    void Start()
    {
        NextPrefab();
    }

    public void OnClick()
    {
        prefabEditor.SetItem(itemNameIfChest);
        prefabEditor.SetSelector(NextPrefab);
        //NextPrefab();
    }

    /// <summary>
    /// <c>NextPrefab</c> Sets a new random next prefab, and returns the current prefab. Also replaces child image with next prefab.
    /// </summary>
    public int NextPrefab()
    {
        int currentPrefabId = nextPrefabId;
        nextPrefabId = prefabIds[Random.Range(0, prefabIds.Count)];
        if(prefabIdMapper.GetGameObjectFromId(nextPrefabId).GetComponent<SpriteRenderer>() != null)
            transform.Find("Image").GetComponent<Image>().sprite = prefabIdMapper.GetGameObjectFromId(nextPrefabId).GetComponent<SpriteRenderer>().sprite;
        else
            transform.Find("Image").GetComponent<Image>().sprite = prefabIdMapper.GetGameObjectFromId(nextPrefabId).GetComponentInChildren<SpriteRenderer>().sprite;
        return currentPrefabId;
    }
}
