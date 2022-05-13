using UnityEngine;
using PixelCrushers.UISSupport;
using Opsive.UltimateInventorySystem.Core.DataStructures;

namespace PixelCrushers.QuestMachine.UISSupport
{

    /// <summary>
    /// Quest action that removes a UIS item.
    /// </summary>
    public class UISRemoveItemQuestAction : QuestAction
    {
        [HelpBox("Leave Inventory Name blank to use Player's inventory. Leave Item Collection Name blank to use any applicable item collection.", HelpBoxMessageType.None)]
        public StringField itemName = new StringField();
        public StringField inventoryName = new StringField();
        public StringField itemCollectionName = new StringField();
        public QuestNumber amount = new QuestNumber(1);

        public override string GetEditorName()
        {
            if (StringField.IsNullOrEmpty(itemName)) return "Remove UIS Item";
            var amountValue = amount.GetValue(quest);
            return !StringField.IsNullOrEmpty(inventoryName)
                ? !StringField.IsNullOrEmpty(itemCollectionName)
                    ? ("Remove " + amountValue + " " + itemName + " from " + inventoryName + " (" + itemCollectionName + ")")
                    : ("Remove " + amountValue + " " + itemName + " from " + inventoryName)
                : ("Remove " + amountValue + " " + itemName + " from Player");
        }

        public override void Execute()
        {
            base.Execute();
            var itemDefinition = UISUtility.GetItemDefinition(StringField.GetStringValue(itemName));
            if (itemDefinition == null) return;
            var inventory = UISUtility.GetInventory(StringField.GetStringValue(inventoryName));
            if (inventory == null) return;
            var itemCollection = UISUtility.GetItemCollection(inventory, StringField.GetStringValue(itemCollectionName));
            var itemInfo = itemCollection != null
                ? itemCollection.GetItemInfo(itemDefinition, false)
                : inventory.GetItemInfo(itemDefinition, false);
            if (!itemInfo.HasValue) return;
            var itemInfoToRemove = (amount.GetValue(quest), itemInfo.Value);
            if (itemCollection != null)
            {
                itemCollection.RemoveItem(itemInfoToRemove);
            }
            else
            {
                inventory.RemoveItem(itemInfoToRemove);
            }
        }

    }

}
