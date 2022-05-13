// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// This reward system sends a message with number of user-definable "things".
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class MessageRewardSystem : RewardSystem
    {

        [Tooltip("Consume reward points when determining reward.")]
        public bool consumePoints = true;

        [Tooltip("How the number passed with the message scales according to the points.")]
        public AnimationCurve pointsCurve = new AnimationCurve(new Keyframe(1, 1), new Keyframe(100, 100));

        public StringField thing = new StringField("Coins");
        public StringField target = new StringField(QuestMachineTags.QUESTERID);
        public StringField message = new StringField("Get");
        public StringField parameter = new StringField("Coin");

        public override int DetermineReward(int points, Quest quest, EntityType entityType)
        {
            var val = (int) pointsCurve.Evaluate(points);

            if (!StringField.IsNullOrEmpty(thing))
            {
                var bodyText = BodyTextQuestContent.CreateInstance<BodyTextQuestContent>();
                bodyText.bodyText = new StringField(val + " " + thing);
                quest.offerContentList.Add(bodyText);
            }

            var messageQuestAction = MessageQuestAction.CreateInstance<MessageQuestAction>();
            messageQuestAction.senderID = new StringField(QuestMachineTags.QUESTGIVERID);
            messageQuestAction.targetID = target;
            messageQuestAction.message = message;
            messageQuestAction.parameter = parameter;
            messageQuestAction.value.valueType = MessageValueType.Int;
            messageQuestAction.value.intValue = val;
            var successInfo = quest.GetStateInfo(QuestState.Successful);
            successInfo.actionList.Add(messageQuestAction);

            return consumePoints ? (points - val) : points;
        }

    }
}