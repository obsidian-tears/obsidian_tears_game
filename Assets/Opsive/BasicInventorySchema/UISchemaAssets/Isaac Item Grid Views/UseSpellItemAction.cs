/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Demo.ItemActions
{
    using Opsive.UltimateInventorySystem.Core.AttributeSystem;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.Core.InventoryCollections;
    using Opsive.UltimateInventorySystem.Demo.CharacterControl;
    using Opsive.UltimateInventorySystem.ItemActions;
    using System;
    using UnityEngine;

    /// <summary>
    /// Demo Item action used to consume an item.
    /// </summary>
    [System.Serializable]
    public class UseSpellItemAction : ItemAction
    {
        
        protected int m_ManaNeeded;
        protected int m_MagicPowerAdded;
        protected float m_MagicPowerMultiplier;
        protected int m_HealAmount;
        protected bool m_FrostDamage;
        protected bool m_FireDamage;
        protected bool m_LightningDamage;
        protected string m_SpellName;
        protected GameObject m_SpellAnimation;

        BattleSystem battleSystem = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public UseSpellItemAction()
        {
            m_Name = "Consume";
        }

        /// <summary>
        /// Can the item action be invoked.
        /// </summary>
        /// <param name="itemInfo">The item info.</param>
        /// <param name="itemUser">The item user (can be null).</param>
        /// <returns>True if it can be invoked.</returns>
        protected override bool CanInvokeInternal(ItemInfo itemInfo, ItemUser itemUser)
        {
            var item = itemInfo.Item;
            var inventory = itemInfo.Inventory;
            var character = itemUser.GetComponent<CharStats>();
            GameObject battleSystemGameObject = GameObject.Find("Battle System");
            if(battleSystemGameObject != null)
            {
                battleSystem = battleSystemGameObject.GetComponent<BattleSystem>();
            }

            ItemCollection equipmentCollection = inventory.GetItemCollection("Equipped");

            if(battleSystem != null && equipmentCollection != null)
            {
                if (character != null && equipmentCollection.HasItem((1, item)))
                {
                    if (item.HasAttribute("Mana Needed"))
                    {
                        if (item.GetAttribute<Attribute<int>>("Mana Needed").GetValue() < character.magicTotal)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }


                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Consume the item.
        /// </summary>
        /// <param name="itemInfo">The item info.</param>
        /// <param name="itemUser">The item user (can be null).</param>
        protected override void InvokeActionInternal(ItemInfo itemInfo, ItemUser itemUser)
        {
            var item = itemInfo.Item;
            var inventory = itemInfo.Inventory;
            var character = itemUser.GetComponent<CharStats>();
            inventory.MainItemCollection.RemoveItem(item);

            //Get all spell values
            m_ManaNeeded = item.GetAttribute<Attribute<int>>("Mana Needed").GetValue();
            m_MagicPowerAdded = item.GetAttribute<Attribute<int>>("Magic Power Added").GetValue();
            m_MagicPowerMultiplier = item.GetAttribute<Attribute<float>>("Magic Power Multiplier").GetValue();
            m_HealAmount = item.GetAttribute<Attribute<int>>("Heal Amount").GetValue();
            m_FrostDamage = item.GetAttribute<Attribute<bool>>("Frost Damage").GetValue();
            m_LightningDamage = item.GetAttribute<Attribute<bool>>("Lightning Damage").GetValue();
            m_FireDamage = item.GetAttribute<Attribute<bool>>("Fire Damage").GetValue();
            m_SpellName = item.GetAttribute<Attribute<string>>("Spell Name").GetValue();
            m_SpellAnimation = item.GetAttribute<Attribute<GameObject>>("Animation Object").GetValue();

            if(battleSystem != null)
            {
                battleSystem.OnSpellUse(m_ManaNeeded, m_MagicPowerAdded, m_MagicPowerMultiplier, m_HealAmount, m_FrostDamage, m_LightningDamage, m_FireDamage, m_SpellName, m_SpellAnimation);
            }
        }
    }
}