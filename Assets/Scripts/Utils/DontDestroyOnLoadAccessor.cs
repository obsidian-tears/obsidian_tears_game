using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Helper class to get objects used as Dont destroy on load - currently used by GameObjectUtility class
/// </summary>
public class DontDestroyOnLoadAccessor : MonoBehaviour
{
    private static DontDestroyOnLoadAccessor _instance;
    public static DontDestroyOnLoadAccessor Instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance != null) Destroy(gameObject);
        this.gameObject.name = this.GetType().ToString();
        _instance = this;
        DontDestroyOnLoad(this);
    }

    public GameObject[] GetAllRootsOfDontDestroyOnLoad()
    {
        return this.gameObject.scene.GetRootGameObjects();
    }

    public GameObject GetDdolGameObjectByName(string goName)
    {
        GameObject[] listOfGO = GetAllRootsOfDontDestroyOnLoad();
        return GameObjectHardFindRootObjects(goName, string.Empty, listOfGO);
    }

    private GameObject GameObjectHardFindRootObjects(string goName, string tag, GameObject[] rootGameObjects)
    {
        if (rootGameObjects == null) return null;
        for (int i = 0; i < rootGameObjects.Length; i++)
        {
            var result = GameObjectSearchHierarchy(rootGameObjects[i].transform, goName, tag);
            if (result != null) return result;
        }
        return null;
    }

    private GameObject GameObjectSearchHierarchy(Transform t, string goName, string tag)
    {
        if (t == null) return null;
        if (string.Equals(t.name, goName) && (string.IsNullOrEmpty(tag) || string.Equals(t.tag, tag))) return t.gameObject;
        foreach (Transform child in t)
        {
            var result = GameObjectSearchHierarchy(child, goName, tag);
            if (result != null) return result;
        }
        return null;
    }
}

