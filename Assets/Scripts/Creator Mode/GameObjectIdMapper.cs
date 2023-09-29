using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectIdMapper : MonoBehaviour
{
    public List<GameObject> gameObjects = new List<GameObject>();

    public GameObject GetGameObjectFromId(int id)
    {
        return gameObjects[id];
    }
}
