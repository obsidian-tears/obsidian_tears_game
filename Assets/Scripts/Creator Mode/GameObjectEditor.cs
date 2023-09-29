using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class GameObjectEditor : MonoBehaviour
{
    [SerializeField] Transform selectionParent;
    [SerializeField] Transform clipboard;
    [SerializeField] GameObject selectionBox;
    [SerializeField] Transform saveSpace;
    [SerializeField] Color hoveredColor;
    [SerializeField] Color selectedColor;
    private GameObject hoveredObject;
    private Vector2 dragMouseOffset;
    private Vector2 dragMouseInitialPosition;
    private bool makingSelection;

    void Update()
    {
        Vector2 worldMousePos = CreatorModeUtils.GetMouseWorldPos();

        //ColorHoveredObject(worldMousePos);

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            Delete();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Copy();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            Paste();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Cut();
        }

        if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
        {
            dragMouseOffset = selectionParent.position - new Vector3(worldMousePos.x, worldMousePos.y, 0);
        }

        if (Input.GetMouseButton(1))
        {
            selectionParent.position = worldMousePos + dragMouseOffset;
        }

        // Left click + Mouse not over UI
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            DeSelect();
            makingSelection = true;
            selectionBox.SetActive(true);
            dragMouseInitialPosition = worldMousePos;
        }

        if (Input.GetMouseButton(0))
        {
            selectionBox.transform.position = dragMouseInitialPosition;
            selectionBox.transform.localScale = worldMousePos - dragMouseInitialPosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (makingSelection)
            {
                HashSet<GameObject> gameObjects = new HashSet<GameObject>(); // All gameObjects should be unique
                // Find all colliders inside selection box
                Collider2D[] colliders = Physics2D.OverlapAreaAll(dragMouseInitialPosition, worldMousePos);
                // Get the respective gameObjects
                foreach (Collider2D collider in colliders)
                {
                    if (collider.transform.parent == saveSpace)
                        gameObjects.Add(collider.gameObject);
                }
                // Select them
                Select(gameObjects);
            }
            makingSelection = false;
            selectionBox.SetActive(false);
        }

    }

    private void Delete()
    {
        foreach (Transform t in selectionParent)
        {
            Destroy(t.gameObject);
        }
        DeSelect();
    }

    private void Copy()
    {
        foreach (Transform t in clipboard)
            Destroy(t.gameObject);
        foreach (Transform t in selectionParent)
        {
            GameObject go = Instantiate(t.gameObject, t.position, Quaternion.identity, clipboard);
            go.SetActive(false);
        }
        DeSelect();
    }

    private void Paste()
    {
        DeSelect();
        foreach (Transform t in clipboard)
        {
            GameObject go = Instantiate(t.gameObject, t.position, Quaternion.identity, selectionParent);
            go.SetActive(true);
        }
    }

    private void Cut()
    {
        foreach (Transform t in clipboard)
            Destroy(t.gameObject);
        foreach (Transform t in selectionParent)
        {
            GameObject go = Instantiate(t.gameObject, t.position, Quaternion.identity, clipboard);
            go.SetActive(false);
            Destroy(t.gameObject);
        }
        DeSelect();
    }

    public void DeSelect()
    {
        HashSet<Transform> transforms = new HashSet<Transform>();
        foreach (Transform t in selectionParent)
        {
            transforms.Add(t);
        }
        // Two loops are necessary because otherwise the above loop
        // would be self-modifying (trust me I found out the hard way)
        foreach (Transform t in transforms)
        {
            CreatorModeUtils.ColorGameObject(t.gameObject, Color.white);
            t.parent = saveSpace;
        }
    }

    private void ColorHoveredObject(Vector2 worldMousePos)
    {
        // Raycast to hit gameObjects
        RaycastHit2D r = Physics2D.Raycast(worldMousePos, Vector2.zero);
        // If we hit a collider...
        if (r.collider != null)
        {
            // Set hoveredObject and color the object
            hoveredObject = r.collider.gameObject;
            CreatorModeUtils.ColorGameObject(hoveredObject, hoveredColor);
        }
        // If we are no longer hitting that transform...
        else if (hoveredObject != null)
        {
            // Restore original color to objects no longer
            // being hovered over
            CreatorModeUtils.ColorGameObject(hoveredObject, Color.white);
            hoveredObject = null;
        }
    }

    private void Select(GameObject gameObject)
    {
        CreatorModeUtils.ColorGameObject(gameObject, selectedColor);
        gameObject.transform.parent = selectionParent;
    }

    private void Select(IEnumerable<GameObject> gameObjects)
    {
        foreach (GameObject go in gameObjects)
            Select(go);
    }



}