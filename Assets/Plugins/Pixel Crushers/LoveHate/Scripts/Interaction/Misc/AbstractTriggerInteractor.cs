// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// This is the base class for trigger components such as AbstractGreetingTrigger and
    /// AbstractGossipTrigger.
    /// </summary>
    public abstract class AbstractTriggerInteractor : MonoBehaviour
    {

        /// <summary>
        /// The max size of the cache of other GameObjects to keep track of.
        /// </summary>
        [Tooltip("Max GameObjects to remember to reduce searches for FactionMember components.")]
        public int cacheSize = 32;

        /// <summary>
        /// A cache of faction members that have been found through GetComponentXXX(). Also 
        /// includes returns null for GameObjects that don't have FactionMember components.
        /// </summary>
        private Dictionary<GameObject, FactionMember> factionMemberCache = new Dictionary<GameObject, FactionMember>();

        /// <summary>
        /// Gets the FactionMember on a GameObject. This method maintains a cache of FactionMember
        /// components found on GameObjects and uses the cache when possible. Since characters 
        /// often bump into the same triggers repeatedly, this significantly reduces the number of
        /// calls to GetComponentXXX().
        /// </summary>
        /// <returns>The FactionMember.</returns>
        /// <param name="other">A GameObject.</param>
        public virtual FactionMember GetFactionMember(GameObject other)
        {
            if (other == null)
            {
                return null;
            }
            else if (factionMemberCache.ContainsKey(other))
            {
                return factionMemberCache[other];
            }
            else
            {
                var factionMember = other.GetComponentInChildren<FactionMember>() ?? other.GetComponentInParent<FactionMember>();
                if (factionMemberCache.Count < cacheSize)
                {
                    factionMemberCache.Add(other, factionMember);
                }
                return factionMember;
            }
        }
    }

}
