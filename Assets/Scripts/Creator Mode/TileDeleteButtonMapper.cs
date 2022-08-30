using UnityEngine;

public class TileDeleteButtonMapper : ButtonTileMapper 
{
    override public void OnClick() {
        te.Select(tileId);
    }
}