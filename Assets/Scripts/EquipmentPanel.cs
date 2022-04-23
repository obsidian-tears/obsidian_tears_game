using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: maybe we keep all UI (dialog and menus, etc) as signals communicating directly with the game object scripts
// TODO: rework with updated inventory
public class EquipmentPanel : MonoBehaviour
{
    [SerializeField] Inventory equipment;
    [SerializeField] InventorySlot inventorySlotTemplate;
    [SerializeField] GameObject helmetSection;
    [SerializeField] GameObject armorSection;
    [SerializeField] GameObject shieldSection;
    [SerializeField] GameObject weaponSection;
    [SerializeField] GameObject otherSection;

    void Start() {
        UpdateEquiptmentSlots();
    }
    public void UpdateEquiptmentSlots()
    {
        // destroy all inventory slots
        var childrenTransforms = transform.GetComponentsInChildren<InventorySlot>();
        foreach (InventorySlot slot in childrenTransforms)
        {
            GameObject.Destroy(slot.transform.gameObject);
        }

        // add updated slots
        foreach (InventoryItem item in equipment.myInventory)
        {
            GameObject tempObject = item.subType switch
            {
                EquipmentType.Helmet => helmetSection,
                EquipmentType.BodyArmor => armorSection,
                EquipmentType.Shield => shieldSection,
                EquipmentType.Weapon => weaponSection,
                _ => otherSection
            };
            InventorySlot newSlot = Instantiate(inventorySlotTemplate, tempObject.transform.position,
                                                tempObject.transform.rotation).GetComponent<InventorySlot>();
            newSlot.transform.SetParent(tempObject.transform);
            newSlot.Setup(item);
        }
    }
}
