using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoxAreaPlacer : MonoBehaviour
{
    [SerializeField] Transform saveSpace;
    private Vector2 startMousePos;
    private void Update()
    {
        Vector2 worldMousePos = CreatorModeUtils.GetMouseWorldPos();

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            startMousePos = worldMousePos;
        }

        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            GameObject go = CreatorModeUtils.CreateVisibleBoxArea("Box Area", (startMousePos, worldMousePos), saveSpace);
            // var collider = go.GetComponent<BoxCollider2D>();
            // if (collider == null) Debug.LogError("Area Collider Null");
            // selectedArea.GetComponent<DebugPolygonCollider2D>().DrawLines();
        }
    }

}
