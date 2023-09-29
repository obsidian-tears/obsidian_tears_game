using Opsive.UltimateInventorySystem.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class GameObjectPlacer : MonoBehaviour
{
    [SerializeField] GameObjectIdMapper prefabIdMapper;
    [SerializeField] Color ghostCopyColor;
    [SerializeField] Transform saveSpace;

    public delegate int GetPrefabId();
    private GetPrefabId getNextPrefabId;

    private int selectedPrefabId = -1;
    private string selectedItem = "";
    private GameObject ghostObject;

    void Update()
    {
        if (selectedPrefabId == -1) return;

        Vector2 worldMousePos = CreatorModeUtils.GetMouseWorldPos();

        // Make ghost object follow the mouse
        if (ghostObject != null) ghostObject.transform.position = worldMousePos;

        // On left click, as long as cursor is not over UI element, place selected prefab in the world
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            GameObject instantiatedObject = CreatorModeUtils.InstantiateWithExtras(GetSelectedPrefab(), worldMousePos, saveSpace, selectedPrefabId);
            ItemObject itemObj = instantiatedObject.GetComponent<ItemObject>();
            if(itemObj != null)
            {

                var newItem = InventorySystemManager.CreateItem(selectedItem);
                itemObj.SetItem(newItem);
            }
            // Select new prefab from Prefab Button Mapper
            Select(getNextPrefabId());
        }

    }

    /// <summary> 
    /// Sets the selector, which will be used to determine the next
    /// selected prefab
    /// </summary>
    public void SetSelector(GetPrefabId getPrefabId)
    {
        this.getNextPrefabId = getPrefabId;
        Select(getNextPrefabId());
    }

    public void SetItem(string itemName)
    {
        selectedItem = itemName;
    }

    /// <summary>
    /// Selects a prefab by prefabId
    /// </summary>
    private void Select(int prefabId)
    {
        // Cleanup current selection
        DeSelect();

        selectedPrefabId = prefabId;

        InstantiateGhostObject(prefabIdMapper.GetGameObjectFromId(selectedPrefabId));
    }

    /// <summary>
    /// Instantiates a ghost version of the selected prefab to show 
    /// where the prefab will be placed
    /// </summary>
    private void InstantiateGhostObject(GameObject prefab)
    {
        Vector3 worldMouseLoc = CreatorModeUtils.GetMouseWorldPos();
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
    public void DeSelect()
    {
        selectedPrefabId = -1;
        if (ghostObject != null)
        {
            Destroy(ghostObject);
            ghostObject = null;
        }
    }

    /// <summary>
    /// Returns the GameObject that corresponds to selectedPrefabId
    /// </summary>
    private GameObject GetSelectedPrefab()
    {
        return prefabIdMapper.GetGameObjectFromId(selectedPrefabId);
    }
}