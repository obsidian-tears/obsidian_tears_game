/* [REMOVE THIS LINE]

using UnityEngine;

namespace PixelCrushers.QuestMachine.Demo
{

    /// <summary>
    /// This example script demonstrates how to create a quest using QuestBuilder.
    /// </summary>
    [RequireComponent(typeof(QuestGiver))]
    public class QuestBuilderExample : MonoBehaviour
    {
        // Quest parameters:
        public string questId = "FindCoins";
        public string questTitle = "Find Coins";
        public Sprite questIcon;
        public string questOfferText = "Orcs stole my gold and hid it in containers around the village! Will you please find 3 coins so I can buy seeds next season?";

        // Counter parameters:
        public string counterName = "coins";
        public int counterInitial = 0;
        public int counterMin = 0;
        public int counterMax = 3;
        public bool counterRandomizeInitialValue = false;

        // Condition node parameters:
        public string node1Id = "Condition";
        public string node1Name = "Find Coins";
        public string node1ActDialogText = "Please find my money.";
        public string node1ActJournalText = "Find the farmer's coins hidden in containers.";
        public string node1HUDText = "{#coins}/3 Coins"; // {#coins} is a special tag representing the current value of the coins counter.

        // Return to NPC node parameters:
        public string node2Id = "Talk to NPC";
        public string node2ActDialogText = "Thanks for finding my coins!";
        public string node2ActJournalText = "Return to the villager.";
        public string node2HUDText = "Return to villager";
        public string node2TrueJournalText = "You found the farmer's coins and saved the farm!";

        private void Start()
        {
            // Start our QuestBuilder: (It's like .NET's StringBuilder, but for quests.)
            var questBuilder = new QuestBuilder(name, questId, questTitle);

            // Set up some basic properties:
            questBuilder.quest.icon = questIcon;
            questBuilder.quest.isTrackable = true;
            questBuilder.quest.showInTrackHUD = true;
            // You could also set questBuilder.quest.group if you want to put this quest in a group.

            // Add offer text:
            questBuilder.AddOfferContents(questBuilder.CreateTitleContent(), questBuilder.CreateBodyContent(questOfferText));

            // Add quest title to the Active and Successful states' dialogue, journal, and HUD text:
            //
            // The first line below looks up the Active state, and the Dialogue content for the Active state. 
            // Then it adds title content, which is a HeadingTextQuestContent object with useQuestTitle set true.
            questBuilder.AddContents(questBuilder.quest.GetStateInfo(QuestState.Active).GetContentList(QuestContentCategory.Dialogue), questBuilder.CreateTitleContent());
            // The remaining lines do the same for Journal and HUD content in the Active and Successful states.
            questBuilder.AddContents(questBuilder.quest.GetStateInfo(QuestState.Active).GetContentList(QuestContentCategory.Journal), questBuilder.CreateTitleContent());
            questBuilder.AddContents(questBuilder.quest.GetStateInfo(QuestState.Active).GetContentList(QuestContentCategory.HUD), questBuilder.CreateTitleContent());
            questBuilder.AddContents(questBuilder.quest.GetStateInfo(QuestState.Successful).GetContentList(QuestContentCategory.Dialogue), questBuilder.CreateTitleContent());
            questBuilder.AddContents(questBuilder.quest.GetStateInfo(QuestState.Successful).GetContentList(QuestContentCategory.Journal), questBuilder.CreateTitleContent());


            //===== COUNTER CONDITION NODE =====
            // Add counter to keep track of number of coins picked up:
            var counter = questBuilder.AddCounter(counterName, counterInitial, counterMin, counterMax, counterRandomizeInitialValue, QuestCounterUpdateMode.Messages);
            // When picked up, coins send the message "Get" "Coin". Tell the counter to increment when this message is sent:
            var counterMessageEvent = new QuestCounterMessageEvent(null, new StringField("Get"), new StringField("Coin"), QuestCounterMessageEvent.Operation.ModifyByLiteralValue, 1);
            counter.messageEventList.Add(counterMessageEvent);

            // Add the counter condition node ([START] --> [Condition]):
            var conditionNode = questBuilder.AddConditionNode(questBuilder.GetStartNode(), node1Id, node1Name, ConditionCountMode.All);
            questBuilder.AddCounterCondition(conditionNode, counterName, CounterValueConditionMode.AtLeast, counterMax);
            // Add text to show when node is active:
            questBuilder.AddContents(conditionNode.GetStateInfo(QuestNodeState.Active).GetContentList(QuestContentCategory.Dialogue), questBuilder.CreateBodyContent(node1ActDialogText));
            questBuilder.AddContents(conditionNode.GetStateInfo(QuestNodeState.Active).GetContentList(QuestContentCategory.Journal), questBuilder.CreateBodyContent(node1ActJournalText));
            questBuilder.AddContents(conditionNode.GetStateInfo(QuestNodeState.Active).GetContentList(QuestContentCategory.HUD), questBuilder.CreateBodyContent(node1HUDText));


            //===== RETURN TO NPC NODE =====
            // Add a "return to quest giver" node ([START] --> [Condition] --> [Return]):
            var questGiver = GetComponent<QuestGiver>();
            var returnNode = questBuilder.AddDiscussQuestNode(conditionNode, QuestMessageParticipant.QuestGiver, questGiver.id);

            // Text when active:
            // Add text to show when node is active:
            questBuilder.AddContents(returnNode.GetStateInfo(QuestNodeState.Active).GetContentList(QuestContentCategory.Dialogue), questBuilder.CreateBodyContent(node2ActDialogText));
            questBuilder.AddContents(returnNode.GetStateInfo(QuestNodeState.Active).GetContentList(QuestContentCategory.Journal), questBuilder.CreateBodyContent(node2ActJournalText));
            questBuilder.AddContents(returnNode.GetStateInfo(QuestNodeState.Active).GetContentList(QuestContentCategory.HUD), questBuilder.CreateBodyContent(node2HUDText));

            // Add text to show when node is true:
            questBuilder.AddContents(returnNode.GetStateInfo(QuestNodeState.True).GetContentList(QuestContentCategory.Dialogue), questBuilder.CreateBodyContent(node2ActDialogText));
            questBuilder.AddContents(returnNode.GetStateInfo(QuestNodeState.True).GetContentList(QuestContentCategory.Journal), questBuilder.CreateBodyContent(node2TrueJournalText));

            // Add an alert action when this node becomes active:
            var actionList = returnNode.GetStateInfo(QuestNodeState.Active).actionList;
            var alertAction = questBuilder.CreateAlertAction(node2HUDText);
            actionList.Add(alertAction);

            // Set the quest indicator to Talk when active:
            var indicatorAction = questBuilder.CreateSetIndicatorAction(questBuilder.quest.id, questGiver.id, QuestIndicatorState.Talk);
            returnNode.GetStateInfo(QuestNodeState.Active).actionList.Add(indicatorAction);

            // Set the Indicator to None when true:
            indicatorAction = questBuilder.CreateSetIndicatorAction(questBuilder.quest.id, questGiver.id, QuestIndicatorState.None);
            returnNode.GetStateInfo(QuestNodeState.True).actionList.Add(indicatorAction);


            //===== SUCCESS NODE =====
            // Add a Success node: [START] --> [Condition] --> [Return] -- > [SUCCESS]
            questBuilder.AddSuccessNode(returnNode);


            //===== FINISH CREATION PROCESS =====
            // Finally, get the quest:
            Quest myQuest = questBuilder.ToQuest();
            // And add it to the QuestGiver:
            GetComponent<QuestGiver>().AddQuest(myQuest);
        }
    }
}


/**/