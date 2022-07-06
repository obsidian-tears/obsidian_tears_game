// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Sends a message "Add XP" with a value of XP equal to the reward points.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class XPRewardSystem : RewardSystem
    {

        public override int DetermineReward(int points, Quest quest, EntityType entityType)
        {
            var xp = (entityType != null) ? (int)(points * entityType.GetRewardMultiplier(RewardMultiplier.XP)) : points;

            var bodyText = BodyTextQuestContent.CreateInstance<BodyTextQuestContent>();
            bodyText.bodyText = new StringField(xp + " XP");
            quest.offerContentList.Add(bodyText);

            var xpAction = MessageQuestAction.CreateInstance<MessageQuestAction>();
            xpAction.senderID = new StringField(QuestMachineTags.QUESTGIVERID); // Send from quest giver.
            xpAction.targetID = new StringField(QuestMachineTags.QUESTERID); // Send to quester (player).
            xpAction.message = new StringField("Add XP");
            xpAction.parameter = new StringField(xp.ToString());
            xpAction.value = new MessageValue(xp);
            var successInfo = quest.GetStateInfo(QuestState.Successful);
            successInfo.actionList.Add(xpAction);

            return points;
        }

    }
}