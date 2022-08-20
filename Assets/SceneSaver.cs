using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SceneSaver : MonoBehaviour
{

    [SerializeField]
    public Transform saveSpace;
    List<GameObject> gameObjectsToSave = new List<GameObject>();

    public void SaveAll() {
        foreach (Transform transform in saveSpace) {
            Save(transform);
        }
        SaveData saveData = new SaveData();
        saveData.gameObjects = gameObjectsToSave.ToArray();
        saveData.dateTime = System.DateTime.Now.ToString();
        saveData.versionNumber = "0.1.0";
        string jsonSave = JsonUtility.ToJson(saveData);
        File.WriteAllText(Application.persistentDataPath + "/CreatorModeSave.json", jsonSave);
    }

    private void Save(Transform transform) {
        
    }

    [System.Serializable]
    class SaveData {
        public GameObject[] gameObjects;
        public string dateTime;
        public string versionNumber;
    }

    class GameObjectData {

    }

}