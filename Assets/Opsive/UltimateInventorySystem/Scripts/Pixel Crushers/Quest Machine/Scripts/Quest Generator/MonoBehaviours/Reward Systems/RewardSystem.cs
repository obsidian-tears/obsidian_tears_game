// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Base reward system class. Add subclasses to a QuestGeneratorEntity to allow it to 
    /// offer rewards when generating a quest.
    /// </summary>
    public abstract class RewardSystem : MonoBehaviour
    {

        [Tooltip("Probability that this reward system will give a reward, where 0 is never and 1 is always.")]
        [Range(0, 1)]
        [SerializeField]
        public float probability = 1;

        // Base version falls back to the non-entityType version:
        public virtual int DetermineReward(int points, Quest quest, EntityType entityType)
        {
            return DetermineReward(points, quest);
        }

        // This method without entity type is retained for compatibility with older reward systems:
        public virtual int DetermineReward(int points, Quest quest)
        {
            return points; // Base class doesn't do anything, so don't use up any points.            
        }

    }
}
