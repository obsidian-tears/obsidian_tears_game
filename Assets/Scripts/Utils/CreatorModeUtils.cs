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
    public static GameObject InstantiateToSaveSpaceWithExtras(GameObject prefab, Vector2 position, Transform saveSpace, int id)
    {
        GameObject go = Instantiate(prefab, new Vector3(position.x, position.y, 0), Quaternion.identity, saveSpace);
        go.AddComponent<CreatorModeID>().id = id;
        BoxCollider2D boxCollider2D = go.AddComponent<BoxCollider2D>();
        boxCollider2D.size = go.GetComponent<SpriteRenderer>().size;
        return go;
    }
}
