using UnityEngine;

public class TileDeleteButtonMapper : ButtonTileMapper
{
    override public void OnClick()
    {
        tilemapEditor.Select(tileId);
    }
}