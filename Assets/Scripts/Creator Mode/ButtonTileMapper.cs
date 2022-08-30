using UnityEngine;

public class ButtonTileMapper : MonoBehaviour
{
    [SerializeField] protected TilemapEditor te;
    [SerializeField] protected int tileId;
    [SerializeField] protected int tilemapLayer;

    public virtual void OnClick() {
        te.Select(tileId);
        te.SetLayer(tilemapLayer);
    }
}
