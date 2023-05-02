using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class PrefabEditor : MonoBehaviour
{
    [SerializeField] PrefabIdMapper pim;
    [SerializeField] Color ghostCopyColor;
    [SerializeField] Color hoveredColor;
    [SerializeField] Transform saveSpace;

    public delegate int GetPrefabId();
    private GetPrefabId getNextPrefabId;

    private int selectedPrefabId = -1;
    private GameObject ghostObject;
    private GameObject hoveredObject;
    private GameObject selectedObject;
    private Vector2 mouseOffset;

    void Update()
    {

        // Deselect selected prefab on right click
        if (Input.GetMouseButtonDown(1)) DeSelectPrefab();

        if (selectedPrefabId == -1)
        {
            // If nothing is selected, go into editing mode
            HandleEditing();
            // TODO: make sure a tile isn't selected before handling editing
        }
        else
        {
            // If something is selected, go into placing mode
            HandlePlacing();
        }

    }

    private void HandleEditing()
    {
        Vector2 worldMousePos = GetMouseWorldPos();
        RaycastHit2D r = Physics2D.Raycast(worldMousePos, Vector2.zero);
        if (r.transform != null)
        {
            hoveredObject = r.transform.gameObject;
            CreatorModeUtils.ColorGameObject(hoveredObject, hoveredColor);
        }
        else if (hoveredObject != null)
        {
            // Restore original color to objects no longer
            // being hovered over
            CreatorModeUtils.ColorGameObject(hoveredObject, Color.white);
            hoveredObject = null;
        }

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (hoveredObject == null)
            {
                // TODO: Drag
                // TODO: Multiple selected objects
                // TODO: Copy Paste
                return;
            }

            if (selectedObject == null)
            {
                selectedObject = hoveredObject;
                mouseOffset = selectedObject.transform.position - new Vector3(worldMousePos.x, worldMousePos.y, 0);
            }
            else
            {
                selectedObject = null;
            }
        }

        if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (hoveredObject != null)
            {
                Destroy(hoveredObject);
                hoveredObject = null;
            }
        }

        if (selectedObject != null)
        {
            selectedObject.transform.position = worldMousePos + mouseOffset;
        }


    }

    private void HandlePlacing()
    {
        Vector2 worldMousePos = GetMouseWorldPos();

        // Make ghost object follow the mouse
        if (ghostObject != null) ghostObject.transform.position = worldMousePos;

        // On left click, as long as cursor is not over UI element, place selected prefab in the world
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            InstantiateSelectedPrefabToSaveSpaceWithId(worldMousePos);
            // Select new prefab from Prefab Button Mapper
            SelectPrefab(getNextPrefabId());
        }

    }

    /// <summary> 
    /// Sets the selector, which will be used to determine the next
    /// selected prefab
    /// </summary>
    public void SetSelector(GetPrefabId getPrefabId)
    {
        this.getNextPrefabId = getPrefabId;
        SelectPrefab(getNextPrefabId());
    }

    /// <summary>
    /// Selects a prefab by prefabId
    /// </summary>
    private void SelectPrefab(int prefabId)
    {
        // Cleanup current selection
        DeSelectPrefab();

        selectedPrefabId = prefabId;

        InstantiateGhostObject(pim.GetPrefabFromId(selectedPrefabId));
    }

    /// <summary>
    /// Instantiates a ghost version of the selected prefab to show 
    /// where the prefab will be placed
    /// </summary>
    private void InstantiateGhostObject(GameObject prefab)
    {
        Vector3 worldMouseLoc = GetMouseWorldPos();
        ghostObject = Instantiate(prefab, worldMouseLoc, Quaternion.identity);

        // Color ghostObject its respective color
        CreatorModeUtils.ColorGameObject(ghostObject, ghostCopyColor);

        // Strip ghost object of all components except for sprite 
        // renderers so it doesn't interact with the game
        foreach (Component component in ghostObject.GetComponents(typeof(Component)))
            if (component is Behaviour && component is not SpriteRenderer)
            {
                Behaviour b = (Behaviour)component;
                b.enabled = false;
            }
    }

    /// <summary>
    /// Deselects the selected prefab.
    /// Sets selectedPrefabId to -1 and Destroys the ghostObject
    /// </summary>
    public void DeSelectPrefab()
    {
        selectedPrefabId = -1;
        if (ghostObject != null)
        {
            Destroy(ghostObject);
            ghostObject = null;
        }
    }

    /// <summary>
    /// Returns the game world coordinates that corresponds to the 
    /// current mouse location
    /// </summary>
    Vector3 GetMouseWorldPos()
    {
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        return worldMousePos;
    }

    private GameObject InstantiateSelectedPrefabToSaveSpaceWithId(Vector2 position)
    {
        return CreatorModeUtils.InstantiateToSaveSpaceWithExtras(GetSelectedPrefab(), position, saveSpace, selectedPrefabId);
    }

    /// <summary>
    /// Returns the GameObject that corresponds to selectedPrefabId
    /// </summary>
    private GameObject GetSelectedPrefab()
    {
        return pim.GetPrefabFromId(selectedPrefabId);
    }
}
