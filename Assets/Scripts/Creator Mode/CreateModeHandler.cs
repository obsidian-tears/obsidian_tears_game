using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateModeHandler : MonoBehaviour
{
    [SerializeField] GameObjectPlacer gameObjectPlacer;
    [SerializeField] GameObjectEditor gameObjectEditor;
    [SerializeField] TilemapEditor tileMapEditor;
    [SerializeField] BoxAreaPlacer boxAreaCreator;
    [SerializeField] AreaEditor areaEditor;

    private EditModeComponent.EditMode mode = EditModeComponent.EditMode.EditingGameObject;


    private void Update()
    {
        if (Input.GetMouseButtonUp(1))
            SetActiveMode(EditModeComponent.EditMode.Default);
    }

    public void SetActiveModeFromComponent(EditModeComponent comp)
    {
        SetActiveMode(comp.editMode);
    }

    public void SetActiveMode(EditModeComponent.EditMode mode)
    {
        if (mode == EditModeComponent.EditMode.Default)
            mode = EditModeComponent.EditMode.EditingGameObject;

        if (this.mode == mode)
            return;

        SetMode(this.mode, false);
        this.mode = mode;
        SetMode(mode, true);
    }

    private void SetMode(EditModeComponent.EditMode mode, bool enabled)
    {
        switch (mode)
        {
            case EditModeComponent.EditMode.PlacingGameObject:
                gameObjectPlacer.enabled = enabled;
                if (!enabled) gameObjectPlacer.DeSelect();
                break;
            case EditModeComponent.EditMode.EditingGameObject:
                gameObjectEditor.enabled = enabled;
                if (!enabled) gameObjectEditor.DeSelect();
                break;
            case EditModeComponent.EditMode.EditingTileMap:
                tileMapEditor.enabled = enabled;
                if (!enabled)
                {
                    tileMapEditor.DeSelect();
                    tileMapEditor.RevertPreviewTile();
                }
                break;
            case EditModeComponent.EditMode.PlacingBoxArea:
                boxAreaCreator.enabled = enabled;
                break;
            case EditModeComponent.EditMode.EditingArea:
                areaEditor.enabled = enabled;
                break;
            case EditModeComponent.EditMode.None:
                break;
        }
    }

}
