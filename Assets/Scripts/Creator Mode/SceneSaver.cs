using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SceneSaver : MonoBehaviour
{

    [SerializeField] Transform saveSpace;
    [SerializeField] Transform grid;
    [SerializeField] GameObjectIdMapper prefabIdMapper;
    [SerializeField] TileIdMapper tileIdMapper;


    public void Save()
    {
        SaveData saveData = new SaveData();

        List<GameObjectData> gameObjectDataList = new List<GameObjectData>();
        foreach (Transform transform in saveSpace)
        {
            gameObjectDataList.Add(GameObjectToData(transform));
        }
        saveData.gameObjectDatas = gameObjectDataList.ToArray();

        List<TilemapData> tilemapDataList = new List<TilemapData>();
        foreach (Transform transform1 in grid)
        {
            tilemapDataList.Add(TilemapToData(transform1));
        }
        saveData.tilemapDatas = tilemapDataList.ToArray();

        saveData.dateTime = System.DateTime.Now.ToString();
        saveData.versionNumber = "0.1.0";
        string jsonSave = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(Application.persistentDataPath + "/CreatorModeSave.json", jsonSave);
    }

    public void Load()
    {
        ClearSaveSpace();
        ClearGrid();

        string savePath = Application.persistentDataPath + "/CreatorModeSave.json";

        if (!File.Exists(savePath))
        {
            Debug.LogError("Save file not found. Cannot load");
            return;
        }

        string jsonSave = File.ReadAllText(savePath);

        List<GameObjectData> gameObjectDataList = new List<GameObjectData>();
        SaveData saveData = JsonUtility.FromJson<SaveData>(jsonSave);

        foreach (GameObjectData data in saveData.gameObjectDatas)
        {
            InstantiateToSaveSpaceFromGameObjectData(data);
        }

        foreach (TilemapData data1 in saveData.tilemapDatas)
        {
            GameObject go = new GameObject(data1.GetName(), typeof(Tilemap), typeof(TilemapRenderer));
            go.transform.SetParent(grid);
            go.GetComponent<TilemapRenderer>().sortingOrder = data1.tilemapId;

            Tilemap t = go.GetComponent<Tilemap>();

            foreach (TileSaveData tbd in data1.tileDatas)
            {
                t.SetTile(new Vector3Int(tbd.position[0], tbd.position[1]), tileIdMapper.GetTileFromId(tbd.tileId));
            }
        }
    }

    private GameObject InstantiateToSaveSpaceFromGameObjectData(GameObjectData data)
    {
        GameObject go = CreatorModeUtils.InstantiateWithExtras(prefabIdMapper.GetGameObjectFromId(data.prefabId), new Vector2(data.position[0], data.position[1]), saveSpace, data.prefabId);
        go.transform.localScale = new Vector2(data.scale[0], data.scale[1]);
        return go;
    }

    private GameObjectData GameObjectToData(Transform t)
    {
        return new GameObjectData(
            t.gameObject.GetComponent<GameObjectID>().id,
            t
            );
    }

    private TilemapData TilemapToData(Transform t)
    {
        Tilemap tilemap = t.GetComponent<Tilemap>();
        TilemapRenderer tilemapRenderer = t.GetComponent<TilemapRenderer>();
        List<TileSaveData> tileDatas = new List<TileSaveData>();

        BoundsInt bounds = tilemap.cellBounds;
        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                TileBase tile = tilemap.GetTile(new Vector3Int(x, y));
                if (tile == null) continue;
                tileDatas.Add(new TileSaveData(tileIdMapper.GetIdFromTile(tile), x, y));
            }
        }

        return new TilemapData(
            tilemapRenderer.sortingOrder,
            tileDatas.ToArray()
        );
    }

    public void ClearSaveSpace()
    {
        foreach (Transform transform in saveSpace)
        {
            Destroy(transform.gameObject);
        }
    }

    public void ClearGrid()
    {
        foreach (Transform transform in grid)
        {
            Destroy(transform.gameObject);
        }
    }

    [System.Serializable]
    class SaveData
    {
        public GameObjectData[] gameObjectDatas;
        public TilemapData[] tilemapDatas;
        public string dateTime;
        public string versionNumber;
    }

    [System.Serializable]
    class GameObjectData
    {
        public int prefabId = -1;
        public float[] position = new float[2];
        public float[] scale = new float[2];

        public GameObjectData(int prefabId, Transform transform)
        {
            this.prefabId = prefabId;

            position[0] = transform.position.x;
            position[1] = transform.position.y;

            scale[0] = transform.localScale.x;
            scale[1] = transform.localScale.y;
        }
    }

    [System.Serializable]
    class TilemapData
    {
        public int tilemapId;
        public TileSaveData[] tileDatas;

        public TilemapData(int tilemapId, TileSaveData[] tileDatas)
        {
            this.tilemapId = tilemapId;
            this.tileDatas = tileDatas;
        }

        public string GetName()
        {
            return "Tilemap" + tilemapId;
        }
    }

    [System.Serializable]
    class TileSaveData
    {
        public int tileId;
        public int[] position = new int[2];

        public TileSaveData(int tileId, int x, int y)
        {
            this.tileId = tileId;

            position[0] = x;
            position[1] = y;
        }
    }

}