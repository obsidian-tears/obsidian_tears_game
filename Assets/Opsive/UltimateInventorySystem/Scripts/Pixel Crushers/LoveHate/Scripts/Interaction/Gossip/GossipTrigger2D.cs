// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// Gossips with another character when entering its trigger area.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class GossipTrigger2D : AbstractGossipTrigger
    {

#if USE_PHYSICS2D || !UNITY_2018_1_OR_NEWER
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            HandleOnTriggerEnter(other.gameObject);
        }
#endif

        /// <summary>
        /// For optional UtopiaWorx Zone Controller integration.
        /// </summary>
        /// <returns>The properties that Zone Controller can control.</returns>
        public static List<string> ZonePluginActivator()
        {
            List<string> controllable = new List<string>();
            controllable.Add("timeBetweenGreetings|System.Single|0.1|99999|1|This many seconds must pass before greeting the same character again.");
            controllable.Add("cacheSize|System.Int32|1|99999|1|Max GameObjects to remember. Reduces searches for FactionMember components.");
            return controllable;
        }

    }

}
