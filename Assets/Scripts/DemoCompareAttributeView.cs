/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Demo.UI.VisualStructures.AttributeUIs
{
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.Core.InventoryCollections;
    using Opsive.UltimateInventorySystem.Demo.CharacterControl.Player;
    using Opsive.UltimateInventorySystem.Equipping;
    using Opsive.UltimateInventorySystem.UI.Item.AttributeViewModules;
    using UnityEngine;
    using UnityEngine.UI;
    using Text = Opsive.Shared.UI.Text;

    /// <summary>
    /// Compare an attribute value to the matching character equipper item attribute value.
    /// </summary>
    public class DemoCompareAttributeView : AttributeViewModule
    {
        [Tooltip("The stat name.")]
        [SerializeField] protected string m_StatName;
        [Tooltip("The current value text.")]
        [SerializeField] protected Text m_CurrentValueText;
        [Tooltip("The new potential value text.")]
        [SerializeField] protected Text m_NewValueText;
        [Tooltip("The arrow image that can change color.")]
        [SerializeField] protected Image m_ArrowImage;

        // protected CharStats m_PlayerCharacter;
        protected Equipper m_Equipper;
        private GameObject player;
        private Inventory playerInventory;
        private CharStats playerStats;

        /// <summary>
        /// Set the text.
        /// </summary>
        /// <param name="info">the attribute info.</param>
        public override void SetValue(AttributeInfo info)
        {
            if (info.Attribute == null)
            {
                Clear();
                return;
            }

            var item = info.ItemInfo.Item;

            // if (m_PlayerCharacter == null) { m_PlayerCharacter = info.ItemInfo.Inventory?.gameObject?.GetComponent<CharStats>(); }
            // if (m_PlayerCharacter == null) { m_PlayerCharacter = FindObjectOfType<CharStats>(); }

            // if (m_PlayerCharacter == null) {
            //     m_Equipper = FindObjectOfType<Equipper>();
            // } else {
            //     m_Equipper = m_PlayerCharacter.equipper as Equipper;
            // }

            if (!player)
                player = GameObject.FindGameObjectWithTag("Player");

            if (!m_Equipper)
                m_Equipper = player.GetComponent<Equipper>();

            if (m_Equipper == null)
            {
                Debug.LogWarning("Player character or it's equipper was not found for attribute stat comparision.", gameObject);
                return;
            }

            var currentValue = m_Equipper.GetEquipmentStatInt(m_StatName);
            int baseValue = 0;


            if (!playerStats)
                playerStats = player.GetComponent<CharStats>();

            if (playerStats != null)
            {
                if (m_StatName == "Attack")
                {
                    currentValue = playerStats.attackTotal;
                    baseValue = playerStats.attackBase;
                }
                else if (m_StatName == "Defense")
                {
                    currentValue = playerStats.defenseTotal;
                    baseValue = playerStats.defenseBase;
                }
                else if (m_StatName == "Speed")
                {
                    currentValue = playerStats.speedTotal;
                    baseValue = playerStats.speedBase;
                }
                else if (m_StatName == "MaxHp")
                {
                    currentValue = playerStats.healthMax;
                    baseValue = playerStats.healthBase;
                }
                else if (m_StatName == "MaxMp")
                {
                    currentValue = playerStats.magicMax;
                    baseValue = playerStats.magicBase;
                }
                else if (m_StatName == "MagicPower")
                {
                    currentValue = playerStats.magicPowerTotal;
                    baseValue = playerStats.magicPowerBase;
                }
            }

            int statValue = 0;
            if (item.TryGetAttributeValue<int>(m_StatName, out var intAttributeValue))
            {
                statValue = intAttributeValue;
                if (intAttributeValue == 0)
                {
                    gameObject.SetActive(false);
                }
            }
            /*if (item.TryGetAttributeValue<float>(m_StatName, out var floatAttributeValue))
            {
                statValue = floatAttributeValue;
                if (intAttributeValue == 0f)
                {
                    gameObject.SetActive(false);
                }
            }*/

            bool isEquipped = false;

            if (!playerInventory)
                playerInventory = player.GetComponent<Inventory>();

            ItemCollection equippedItems = playerInventory.GetItemCollection("Equipped");
            int otherItemAttributeValues = 0;
            bool foundAccessoryAlready = false;
            foreach (ItemStack itemStack in equippedItems.GetAllItemStacks())
            {
                if (itemStack.Item == item)
                {
                    isEquipped = true;
                    break;
                }
                else
                {
                    if (itemStack.Item.Category.name == item.Category.name)
                    {
                        if (item.Category.name == "Accessory" && !foundAccessoryAlready)
                        {
                            if (itemStack.Item.TryGetAttributeValue<int>(m_StatName, out var accessoryAttributeValue))
                            {
                                otherItemAttributeValues += accessoryAttributeValue;
                            }
                            foundAccessoryAlready = true;
                        }
                    }
                    else if (itemStack.Item.TryGetAttributeValue<int>(m_StatName, out var attributeValue))
                    {
                        otherItemAttributeValues += attributeValue;
                    }
                }
            }

            /*var previewValue = m_Equipper.IsEquipped(item)
                ? m_Equipper.GetEquipmentStatPreviewRemove(m_StatName, item) + addedValue
                : m_Equipper.GetEquipmentStatPreviewAdd(m_StatName, item) + addedValue;*/


            int previewValue = 0;
            if (isEquipped)
            {
                previewValue = currentValue - statValue;
            }
            else
            {
                previewValue = baseValue + otherItemAttributeValues + statValue;

            }

            m_CurrentValueText.text = currentValue.ToString();

            m_ArrowImage.color = previewValue > currentValue ? Color.green : previewValue < currentValue ? Color.red : Color.white;
            m_NewValueText.text = previewValue.ToString();
        }

        /// <summary>
        /// Clear the box.
        /// </summary>
        public override void Clear()
        {
            m_CurrentValueText.text = "?";
            m_ArrowImage.color = Color.grey;
            m_NewValueText.text = "?";
        }
    }
}