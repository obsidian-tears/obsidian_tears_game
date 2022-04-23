using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: change to ItemVariable
[CreateAssetMenu(fileName = "SelectedItem", menuName = "Inventory/Selected Item")]
public class SelectedItem : ScriptableObject
{
    public InventoryItem item;
}
