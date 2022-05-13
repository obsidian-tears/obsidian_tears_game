// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Exchange;
using PixelCrushers.UISSupport;

namespace PixelCrushers.QuestMachine.UISSupport
{

    [AddComponentMenu("Tools/Pixel Crushers/Quest Machine/Third Party/UIS/Generator/UIS Reward System")]
    public class UISRewardSystem : RewardSystem
    {

        [Tooltip("Currency to offer.")]
        [SerializeField]
        private Currency m_currency = null;
        public Currency currency
        {
            get { return m_currency; }
            set { m_currency = value; }
        }
        [SerializeField]
        private int m_currencyAmount = 100;
        public int currencyAmount
        {
            get { return m_currencyAmount; }
            set { m_currencyAmount = value; }
        }

        [Tooltip("Items to offer.")]
        [SerializeField]
        private List<ItemDefinition> m_items = new List<ItemDefinition>();
        public List<ItemDefinition> items
        {
            get { return m_items; }
            set { m_items = value; }
        }

        // The quest generator will call this method to try to use up points
        // by choosing rewards to offer.
        public override int DetermineReward(int points, Quest quest, EntityType goalEntityType)
        {
            var successInfo = quest.GetStateInfo(QuestState.Successful);

            // Offer currency:
            if (currency != null)
            { 
                var currencyPoints = Mathf.Min(points, currencyAmount);
                var amountToOffer = (int)(currencyPoints * goalEntityType.GetRewardMultiplier(RewardMultiplier.Currency));
                if (amountToOffer > 0)
                {
                    currencyAmount -= amountToOffer;

                    // Add currency UI content to the quest's offerContentList:
                    if (currency.Icon == null)
                    {
                        var currencyText = BodyTextQuestContent.CreateInstance<BodyTextQuestContent>();
                        currencyText.bodyText = new StringField(amountToOffer + " " + currency.name);
                        quest.offerContentList.Add(currencyText);
                    }
                    else
                    {
                        var currencyIcon = IconQuestContent.CreateInstance<IconQuestContent>();
                        currencyIcon.image = currency.Icon;
                        currencyIcon.count = amountToOffer;
                        currencyIcon.caption = new StringField(currency.name);
                        quest.offerContentList.Add(currencyIcon);
                    }

                    // Add a UISAddCurrencyQuestAction action to the quest's Successful state:
                    var currencyAction = UISAddCurrencyQuestAction.CreateInstance<UISAddCurrencyQuestAction>();
                    currencyAction.currencyName = new StringField(currency.name);
                    currencyAction.currencyOwnerName = new StringField();
                    currencyAction.amount = new QuestNumber(amountToOffer);
                    successInfo.actionList.Add(currencyAction);

                    // Reduce points left:
                    points -= currencyPoints;
                    if (points <= 0) return 0;
                }
            }

            // If not enough currency, offer items:
            for (int i = items.Count - 1; i >= 0; i--)
            {
                var itemDefinition = items[i];
                var currencyAmount = itemDefinition.GetAttribute<Opsive.UltimateInventorySystem.Core.AttributeSystem.Attribute<CurrencyAmount>>("BuyPrice").GetValue();
                var itemValue = (int)((currencyAmount != null) ? currencyAmount.Amount : Mathf.Infinity);
                if (itemValue <= points)
                {
                    items.Remove(itemDefinition);

                    // Add some UI content to the quest's offerContentList:
                    var itemDefIcon = itemDefinition.GetAttribute<Opsive.UltimateInventorySystem.Core.AttributeSystem.Attribute<Sprite>>("Icon").GetValue();
                    if (itemDefIcon == null)
                    {
                        var itemText = BodyTextQuestContent.CreateInstance<BodyTextQuestContent>();
                        itemText.bodyText = new StringField(itemDefinition.name);
                        quest.offerContentList.Add(itemText);
                    }
                    else
                    {
                        var itemIcon = IconQuestContent.CreateInstance<IconQuestContent>();
                        itemIcon.image = itemDefIcon;
                        itemIcon.count = 1;
                        itemIcon.caption = new StringField(itemDefinition.name);
                        quest.offerContentList.Add(itemIcon);
                    }

                    // Add a UISAddItemQuestAction action to the quest's Successful state:
                    var itemAction = UISAddItemQuestAction.CreateInstance<UISAddItemQuestAction>();
                    itemAction.itemName = new StringField(itemDefinition.name);
                    itemAction.inventoryName = new StringField();
                    itemAction.itemCollectionName = new StringField();
                    itemAction.amount = new QuestNumber(1);
                    successInfo.actionList.Add(itemAction);

                    // Reduce points left:
                    points -= itemValue;
                    if (points <= 0) return 0;
                }
            }

            return points;
        }

    }
}
