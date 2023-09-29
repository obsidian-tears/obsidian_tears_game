using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileIdMapper : MonoBehaviour
{
    public List<TileBase> tiles = new List<TileBase>();

    public TileBase GetTileFromId(int id) {
        if (id == -2) return null;
        return tiles[id];
    }

    public int GetIdFromTile(TileBase tile) {
        for (int i = 0; i < tiles.Count; i++) {
            if (tiles[i] == tile)
                return i;
        }
        return -1;
    }
}
