using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;


/// <summary>
/// Helper class to get objects used as Dont destroy on load - currently used by GameObjectUtility class
/// </summary>
public class DontDestroyOnLoadAccessor : MonoSingleton<DontDestroyOnLoadAccessor>
{
    protected override void Init()
    {
    }

    public GameObject[] GetAllRootsOfDontDestroyOnLoad()
    {
        return gameObject.scene.GetRootGameObjects();
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

