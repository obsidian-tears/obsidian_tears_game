using Opsive.UltimateInventorySystem.UI.Item.ItemViewSlotRestrictions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowEquipmentSlotIcon : MonoBehaviour
{
    [SerializeField] public Image icon;

    public Sprite attackSprite;
    public Sprite defendSprite;
    public Sprite headSprite;
    public Sprite bodySprite;
    public Sprite footSprite;
    public Sprite accessorySprite;
    public Sprite spellsSprite;

    private void LateUpdate()
    {
        GameObject parent = transform.parent.gameObject;
        if (parent != null)
        {
            ItemViewSlotCategoryRestriction restrictionScript = parent.GetComponent<ItemViewSlotCategoryRestriction>();
            if (restrictionScript != null)
            {
                if (restrictionScript.ItemCategory.name == "Weapons")
                {
                    icon.sprite = attackSprite;
                }
                else if (restrictionScript.ItemCategory.name == "Shield")
                {
                    icon.sprite = defendSprite;
                }
                else if (restrictionScript.ItemCategory.name == "Head Wear")
                {
                    icon.sprite = headSprite;
                }
                else if (restrictionScript.ItemCategory.name == "Chest Wear")
                {
                    icon.sprite = bodySprite;
                }
                else if (restrictionScript.ItemCategory.name == "Leg Wear")
                {
                    icon.sprite = footSprite;
                }
                else if (restrictionScript.ItemCategory.name == "Accessory")
                {
                    icon.sprite = accessorySprite;
                }
                else if (restrictionScript.ItemCategory.name == "Spells")
                {
                    icon.sprite = spellsSprite;
                }
                else
                {
                    icon.enabled = false;
                }
            }
        }
    }
}
