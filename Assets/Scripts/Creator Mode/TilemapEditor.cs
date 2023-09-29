using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class TilemapEditor : MonoBehaviour
{
    int selectedTileId = -1;
    [SerializeField] Tilemap selectedTilemap;
    [SerializeField] TileIdMapper tileIdMapper;
    [SerializeField] Color ghostCopyColor;
    [SerializeField] Color deleteColor;
    [SerializeField] Transform grid;

    Vector3Int prevMouse = new Vector3Int(0, 0, -1);
    TileBase prevTile;

    void Update()
    {
        // Revert previous hightlight tile
        RevertPreviewTile();

        // Don't continue if nothing is selected
        if (selectedTileId == -1)
        {
            prevTile = null;
            return;
        }

        // Find the game world coordinates that correspond to the current mouse location
        Vector3Int mouseCellPos = GetMouseCellPos();

        // Set selected tile as preview
        prevTile = GetSelectedTilemap().GetTile(mouseCellPos);
        GetSelectedTilemap().SetTile(mouseCellPos, GetSelectedTile());

        // On left click, as long as cursor is not over UI element, place selected tile in the world
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            PlaceSelectedTile(mouseCellPos);
            prevTile = GetSelectedTile();
        }

        prevMouse = mouseCellPos;
    }

    public void RevertPreviewTile()
    {
        GetSelectedTilemap().SetTile(prevMouse, prevTile);
    }

    private void PlaceSelectedTile(Vector3Int pos)
    {
        GetSelectedTilemap().SetTile(pos, GetSelectedTile());
    }

    public void Select(int tileId)
    {
        DeSelect();

        selectedTileId = tileId;
    }

    public void SetLayer(int tilemapLayer)
    {
        GetSelectedTilemap().SetTile(prevMouse, prevTile);
        string name = "Tilemap" + tilemapLayer;
        Transform t = grid.Find(name);
        if (t == null)
        {
            selectedTilemap = CreateTilemap(name, tilemapLayer);
            return;
        }
        selectedTilemap = t.GetComponent<Tilemap>();
    }

    private Tilemap CreateTilemap(string name, int layer)
    {
        GameObject go = new GameObject(name, typeof(Tilemap), typeof(TilemapRenderer));
        go.transform.SetParent(grid);
        go.GetComponent<TilemapRenderer>().sortingOrder = layer;
        return go.GetComponent<Tilemap>();
    }

    public void DeSelect()
    {
        selectedTileId = -1;
    }

    Vector3Int GetMouseCellPos()
    {
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPos = GetSelectedTilemap().WorldToCell(worldMousePos);
        return cellPos;
    }

    private TileBase GetSelectedTile()
    {
        return tileIdMapper.GetTileFromId(selectedTileId);
    }

    private Tilemap GetSelectedTilemap()
    {
        if (selectedTilemap == null)
        {
            Transform t = grid.Find("Tilemap0");
            if (t == null)
            {
                GameObject go = new GameObject("Tilemap0", typeof(Tilemap), typeof(TilemapRenderer));
                go.transform.SetParent(grid);
                t = go.transform;
            }
            selectedTilemap = t.GetComponent<Tilemap>();
        }
        return selectedTilemap;
    }
}
