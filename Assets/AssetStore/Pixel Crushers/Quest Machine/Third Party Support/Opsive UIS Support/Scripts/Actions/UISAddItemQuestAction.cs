using UnityEngine;
using PixelCrushers.UISSupport;
using Opsive.UltimateInventorySystem.Core.DataStructures;

namespace PixelCrushers.QuestMachine.UISSupport
{

    /// <summary>
    /// Quest action that adds a UIS item.
    /// </summary>
    public class UISAddItemQuestAction : QuestAction
    {
        [HelpBox("Leave Inventory Name blank to use Player's inventory. Leave Item Collection Name blank to use any applicable item collection.", HelpBoxMessageType.None)]
        public StringField itemName = new StringField();
        public StringField inventoryName = new StringField();
        public StringField itemCollectionName = new StringField();
        public QuestNumber amount = new QuestNumber(1);

        public override string GetEditorName()
        {
            if (StringField.IsNullOrEmpty(itemName)) return "Add UIS Item";
            var amountValue = amount.GetValue(quest);
            return !StringField.IsNullOrEmpty(inventoryName)
                ? !StringField.IsNullOrEmpty(itemCollectionName)
                    ? ("Add " + amountValue + " " + itemName + " to " + inventoryName + " (" + itemCollectionName + ")")
                    : ("Add " + amountValue + " " + itemName + " to " + inventoryName)
                : ("Add " + amountValue + " " + itemName + " to Player");
        }

        public override void Execute()
        {
            base.Execute();
            var item = UISUtility.GetItem(StringField.GetStringValue(itemName));
            if (item == null) return;
            var inventory = UISUtility.GetInventory(StringField.GetStringValue(inventoryName));
            if (inventory == null) return;
            var itemCollection = UISUtility.GetItemCollection(inventory, StringField.GetStringValue(itemCollectionName));
            var itemInfo = new ItemInfo(new ItemAmount(item, amount.GetValue(quest)), itemCollection);
            if (itemCollection != null)
            {
                itemCollection.AddItem(itemInfo);
            }
            else
            {
                inventory.AddItem(itemInfo);
            }
        }

    }

}
