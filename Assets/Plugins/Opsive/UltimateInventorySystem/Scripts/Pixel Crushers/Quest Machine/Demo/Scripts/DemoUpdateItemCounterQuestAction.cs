namespace PixelCrushers.QuestMachine.Demo
{
    /// <summary>
    /// This example action for the Demo scene updates a quest counter with the 
    /// current amount of items in the player's inventory. When the Harvest Carrots
    /// quest starts, it uses this action to update the counter.
    /// </summary>
    public class DemoUpdateItemCounterQuestAction : QuestAction
    {
        [HelpBox("This custom action is for the Demo scene. You can delete the DemoUpdateItemCounterQuestAction script when you're done experimenting with the Demo.", HelpBoxMessageType.Info)]

        public DemoInventory.ItemType itemType;

        public override string GetEditorName()
        {
            return "Update " + itemType.ToString().ToLower() + "s counter with current value in DemoInventory";
        }

        public override void Execute()
        {
            base.Execute();
            QuestCounter counter = null;
            switch (itemType)
            {
                case DemoInventory.ItemType.Carrot:
                    counter = quest.GetCounter("carrots");
                    if (counter != null) counter.currentValue = FindObjectOfType<DemoInventory>().GetItemCount((int)itemType);
                    break;
                case DemoInventory.ItemType.Coin:
                    counter = quest.GetCounter("coins");
                    if (counter != null) counter.currentValue = FindObjectOfType<DemoInventory>().GetItemCount((int)itemType);
                    break;
            }
        }
    }
}
