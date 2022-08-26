using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SceneSaver : MonoBehaviour
{

    [SerializeField] Transform saveSpace;
    [SerializeField] PrefabIdMapper pim;

    public void Save() {
        List<GameObjectData> gameObjectDataList = new List<GameObjectData>();
        foreach (Transform transform in saveSpace) {
            gameObjectDataList.Add(GameObjectToData(transform));
        }
        SaveData saveData = new SaveData();
        saveData.gameObjectDatas = gameObjectDataList.ToArray();
        saveData.dateTime = System.DateTime.Now.ToString();
        saveData.versionNumber = "0.1.0";
        string jsonSave = JsonUtility.ToJson(saveData);
        File.WriteAllText(Application.persistentDataPath + "/CreatorModeSave.json", jsonSave);
    }

    public void Load() {
        ClearSaveSpace();

        List<GameObjectData> gameObjectDataList = new List<GameObjectData>();
        string jsonSave = File.ReadAllText(Application.persistentDataPath + "/CreatorModeSave.json");
        SaveData saveData = JsonUtility.FromJson<SaveData>(jsonSave);
        foreach (GameObjectData data in saveData.gameObjectDatas) {
            GameObject go = InstantiateToSaveSpaceFromGameObjectData(data);
        }
    }

    private GameObject InstantiateToSaveSpaceFromGameObjectData(GameObjectData data) {
        GameObject go = Instantiate(pim.GetPrefabFromId(data.prefabId), new Vector3(data.position[0], data.position[1], 0), Quaternion.identity, saveSpace);
        go.transform.localScale = new Vector2(data.scale[0], data.scale[1]);
        go.AddComponent<CreatorModeID>().id = data.prefabId;
        return go;
    }
    
    private GameObjectData GameObjectToData(Transform t) {
        return new GameObjectData(
            t.gameObject.GetComponent<CreatorModeID>().id,
            t
            );
    }

    public void ClearSaveSpace() {
        foreach (Transform transform in saveSpace) {
            Destroy(transform.gameObject);
        }
    }

    [System.Serializable]
    class SaveData {
        public GameObjectData[] gameObjectDatas;
        public string dateTime;
        public string versionNumber;
    }

    [System.Serializable]
    class GameObjectData {
        public int prefabId = -1;
        public float[] position = new float[2];
        public float[] scale = new float[2];

        public GameObjectData(int prefabId, Transform transform) {
            this.prefabId  = prefabId;
            
            position[0] = transform.position.x;
            position[1] = transform.position.y;

            scale[0] = transform.localScale.x;
            scale[1] = transform.localScale.y;
        }
    }

}