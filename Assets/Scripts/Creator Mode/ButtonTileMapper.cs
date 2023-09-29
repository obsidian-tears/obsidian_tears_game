using UnityEngine;

public class ButtonTileMapper : MonoBehaviour
{
    [SerializeField] protected TilemapEditor tilemapEditor;
    // [SerializeField] protected int tileId;
    [SerializeField] public int tileId;
    //[SerializeField] protected int tilemapLayer;
    [SerializeField] public int tilemapLayer;

    public virtual void OnClick()
    {
        tilemapEditor.Select(tileId);
        tilemapEditor.SetLayer(tilemapLayer);
    }
}
