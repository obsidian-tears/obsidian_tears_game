using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.UltimateInventorySystem.DropsAndPickups;
using PixelCrushers.Wrappers;

[RequireComponent(typeof(ItemObjectVisualizer))]
[RequireComponent(typeof(DestructibleSaver))]
public class Pickupable : MonoBehaviour
{
    
    public bool isTreasureChest = true;

    public GameObject chestOpenedPrefab;

    [HideInInspector] public ItemObjectVisualizer vis;
    [HideInInspector] public DestructibleSaver saver;

    void Awake()
    {
        vis = gameObject.GetComponent<ItemObjectVisualizer>();
        if (isTreasureChest)
        {
            
            saver = gameObject.GetComponent<DestructibleSaver>();
            vis.enabled = false;
            saver.destroyedVersionPrefab = chestOpenedPrefab;
        }
        else
        {
            vis.enabled = true;
        }
    }

    void Start()
    {
        if (!isTreasureChest)
        {
            vis.UpdateVisual();
        }
    }

    public void placeOpenedChest()
    {
        if (isTreasureChest)
        {
            Vector3 position = transform.position;
            Instantiate(chestOpenedPrefab, position, Quaternion.identity);
        }
    }
}
