// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// Applies an aura effect when a faction member enters the trigger.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class AuraTrigger2D : AbstractAuraTrigger
    {

#if USE_PHYSICS2D || !UNITY_2018_1_OR_NEWER
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other == null) return;
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
            controllable.Add("timeBetweenEffects|System.Single|0.1|99999|1|This many seconds must pass before the aura affects the same character again.");
            controllable.Add("impact|System.Single|-100|100|1|How powerfully the aura affects characters.");
            controllable.Add("aggression|System.Single|-100|100|1|How submissive (-100) or aggressive (+100) the aura is.");
            controllable.Add("cacheSize|System.Int32|1|99999|1|Max GameObjects to remember. Reduces searches for FactionMember components.");
            controllable.Add("debug|System.Boolean|0|1|1|Log to the console when applying aura effects.");
            return controllable;
        }

    }

}
