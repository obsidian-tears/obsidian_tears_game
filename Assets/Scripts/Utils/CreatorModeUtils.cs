using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatorModeUtils : MonoBehaviour
{
    /// <summary>
    /// Changes all colors found in SpriteRenderers in the 
    /// GameObject to color
    /// </summary>
    public static void ColorGameObject(GameObject go, Color color)
    {
        foreach (SpriteRenderer spriteRenderer in go.GetComponents(typeof(SpriteRenderer)))
            spriteRenderer.color = color;
    }

    /// <summary>
    /// Instatiates given prefab into a saveSpace and adds an 
    /// ID component for save/loading purposes, as well as
    /// a new box collider for editing purposes
    /// </summary>
    public static GameObject InstantiateWithExtras(GameObject prefab, Vector2 position, Transform parent, int id)
    {
        GameObject go = Instantiate(prefab, new Vector3(position.x, position.y, 0), Quaternion.identity, parent);
        go.AddComponent<GameObjectID>().id = id;
        BoxCollider2D boxCollider2D = go.AddComponent<BoxCollider2D>();
        boxCollider2D.size = go.GetComponent<SpriteRenderer>().size;
        boxCollider2D.isTrigger = true;
        return go;
    }

    public static GameObject CreateBoxArea(string name, (Vector2, Vector2) position, Transform parent)
    {
        GameObject go = new GameObject(name);
        go.transform.parent = parent;
        BoxCollider2D collider = go.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        var (point1, point2) = position;
        collider.offset = (point1 + point2) / 2f;
        collider.size = new Vector2(Mathf.Abs(point1.x - point2.x), Mathf.Abs(point1.y - point2.y));
        go.AddComponent<AreaID>();
        return go;
    }

    public static GameObject CreateVisibleBoxArea(string name, (Vector2, Vector2) position, Transform parent)
    {
        GameObject go = CreateBoxArea(name, position, parent);
        go.AddComponent<DrawBoxCollider2D>();
        return go;
    }


    public static GameObject CreatePolygonArea(string name, Vector2 position, Transform parent)
    {
        GameObject go = new GameObject(name);
        go.transform.position = position;
        go.transform.parent = parent;
        PolygonCollider2D collider = go.AddComponent<PolygonCollider2D>();
        collider.isTrigger = true;
        collider.points = new Vector2[0];
        go.AddComponent<AreaID>();
        return go;
    }

    public static GameObject CreateVisiblePolygonArea(string name, Vector2 position, Transform parent)
    {
        GameObject go = CreatePolygonArea(name, position, parent);
        go.AddComponent<DebugPolygonCollider2D>();
        return go;
    }

    /// <summary>
    /// Returns the game world coordinates that corresponds to the 
    /// current mouse location
    /// </summary>
    public static Vector3 GetMouseWorldPos()
    {
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        return worldMousePos;
    }
}
