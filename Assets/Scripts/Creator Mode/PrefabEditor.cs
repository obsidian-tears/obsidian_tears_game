using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class PrefabEditor : MonoBehaviour
{
    int selectedPrefabId = -1;
    private GameObject ghostObject;

    [SerializeField] PrefabIdMapper pim;
    [SerializeField] Color ghostCopyColor;
    [SerializeField] Transform saveSpace;

    public delegate int GetPrefabId();
    private GetPrefabId getNextPrefabId;

    void Update() {

        // Deselect selected prefab on right click
        if (Input.GetMouseButtonDown(1)) DeSelect();
       
        // Don't continue if nothing is selected
        if (selectedPrefabId == -1) return;

        // Find the game world coordinates that correspond to the current mouse location
        Vector2 worldMousePos = GetMouseWorldPos();

        // Make ghost object follow the mouse
        if (ghostObject != null) ghostObject.transform.position = worldMousePos;
        
        // On left click, as long as cursor is not over UI element, place selected prefab in the world
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
            InstantiateSelectedPrefabToSaveSpaceWithId(worldMousePos);
            // Select new prefab from Prefab Button Mapper
            Select(getNextPrefabId());
        }

    }

    public void SetSelector(GetPrefabId getPrefabId) {
        this.getNextPrefabId = getPrefabId;
        Select(getNextPrefabId());
    }

    private void Select(int prefabId) {
        // Cleanup current selection
        DeSelect();

        selectedPrefabId = prefabId;
  
        InstantiateGhostObject(pim.GetPrefabFromId(selectedPrefabId));
    }

    private void InstantiateGhostObject(GameObject prefab) {
        Vector3 worldMouseLoc = GetMouseWorldPos();
        ghostObject = Instantiate(prefab, worldMouseLoc, Quaternion.identity);
        Component[] ghostComponents = ghostObject.GetComponents(typeof(Component));
        
        // Strip ghost object of all components except for sprite renderer so it doesn't interact with the game
        foreach (Component component in ghostComponents) {
            if (component is SpriteRenderer) {
                SpriteRenderer sr = (SpriteRenderer)component;
                sr.color = ghostCopyColor;
            } else if (component is Behaviour) {
                Behaviour b = (Behaviour)component;
                b.enabled = false;
            }
        }
    }
    public void DeSelect() {
        selectedPrefabId = -1;
        if(ghostObject != null) {
            Destroy(ghostObject);
            ghostObject = null;
        }
    }

    Vector3 GetMouseWorldPos() {
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        return worldMousePos;
    }

    private GameObject InstantiateSelectedPrefabToSaveSpaceWithId(Vector2 position) {
        GameObject go = Instantiate(GetSelectedPrefab(), new Vector3(position.x, position.y, 0), Quaternion.identity, saveSpace);
        go.AddComponent<CreatorModeID>().id = selectedPrefabId;
        return go;
    }

    /*
    private GameObject InstantiateFromIdToSaveSpace(int prefabId, Vector2 position) {
        return InstantiateFromId(prefabId, new Vector3(position.x, position.y, 0), Quaternion.identity, saveSpace);
    }

    private GameObject InstantiateFromId(int prefabId, Vector3 position, Quaternion rotation, Transform parent) {
        return Instantiate(pim.GetPrefabFromId(prefabId), position, rotation, parent);
    }
    */

    private GameObject GetSelectedPrefab() {
        return pim.GetPrefabFromId(selectedPrefabId);
    }
}
