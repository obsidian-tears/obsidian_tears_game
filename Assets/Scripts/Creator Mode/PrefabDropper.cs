using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class PrefabDropper : MonoBehaviour
{

    GameObject selectedPrefab;
    private GameObject ghostObject;

    [SerializeField]
    public Color ghostCopyColor;
    [SerializeField]
    public Transform saveSpace;

    public delegate GameObject GetPrefab();
    private GetPrefab getPrefab;
    

    void Update() {

        // Deselect selected prefab on right click
        if (Input.GetMouseButtonDown(1)) DeSelect();
       
        // Don't bother continuing if nothing is selected
        if (selectedPrefab == null) return;

        // Find the game world coordinates that correspond to the current mouse location
        Vector2 worldMouseLoc = GetMouseWorldLoc();

        // Make ghost object follow the mouse
        if (ghostObject != null) ghostObject.transform.position = worldMouseLoc;
        
        // On left click, as long as cursor is not over UI element, place selected prefab in the world
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
            InstantiateToSaveSpace(selectedPrefab, worldMouseLoc);
            // Select new prefab from Prefab Button Mapper
            Select(getPrefab());
        }

    }

    // Using a selector enables one button to be mapped to multiple prefabs
    public void SetSelector(GetPrefab getPrefab) {
        this.getPrefab = getPrefab;
        Select(getPrefab());
    }

    private void Select(GameObject prefab) {
        // Cleanup current selection
        DeSelect();

        selectedPrefab = prefab;
  
        // Instantiate a ghost object to follow the mouse
        Vector2 worldMouseLoc = GetMouseWorldLoc();
        ghostObject = Instantiate(selectedPrefab, new Vector3(worldMouseLoc.x, worldMouseLoc.y, 0), Quaternion.identity);
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
        selectedPrefab = null;
        if(ghostObject != null) {
            Destroy(ghostObject);
            ghostObject = null;
        }
    }

    Vector2 GetMouseWorldLoc() {
        Vector2 screenMouseLoc = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldMouseLoc = Camera.main.ScreenToWorldPoint(screenMouseLoc);
        return worldMouseLoc;
    }

    void InstantiateToSaveSpace(GameObject prefab, Vector2 position) {
        Instantiate(prefab, new Vector3(position.x, position.y, 0), Quaternion.identity, saveSpace);
    }

}
