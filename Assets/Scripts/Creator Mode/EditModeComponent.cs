using System;
using UnityEngine;

public class EditModeComponent : MonoBehaviour
{
    public EditMode editMode;

    [Serializable]
    public enum EditMode
    {
        Default,
        PlacingGameObject,
        EditingGameObject,
        EditingTileMap,
        PlacingBoxArea,
        EditingArea,
        None,
    }
}