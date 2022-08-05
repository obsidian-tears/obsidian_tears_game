using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabDropper : MonoBehaviour
{

    GameObject selectedPrefab;
    private GameObject ghostObject;

    [SerializeField]
    public Color ghostCopyColor;
    [SerializeField]
    public Transform saveSpace;

    void Update() {

        if (Input.GetMouseButtonDown(1)) DeSelect();
       
        if (selectedPrefab == null) return;

        Vector2 worldMouseLoc = GetMouseWorldLoc();

        if (ghostObject != null) ghostObject.transform.position = worldMouseLoc;
        
        if (Input.GetMouseButtonDown(0)) {
            InstantiateToSaveSpace(selectedPrefab, worldMouseLoc);
        }

    }

    public void Select(GameObject prefab) {
        DeSelect();

        selectedPrefab = prefab;
  
        Vector2 worldMouseLoc = GetMouseWorldLoc();

        ghostObject = Instantiate(selectedPrefab, new Vector3(worldMouseLoc.x, worldMouseLoc.y, 0), Quaternion.identity);
        Component[] ghostComponents = ghostObject.GetComponents(typeof(Component));
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

    void DeSelect() {
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
