/* [REMOVE THIS LINE]
 * [REMOVE THIS LINE] If your code references scripts or assets that are outside of the Plugins
 * [REMOVE THIS LINE] folder, move this script outside of the Plugins folder, too.

using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    // This is a starter template for urgency functions. To use it,
    /// make a copy, rename it, and remove the line marked above.
    /// Then fill in your code where indicated below.
    [CreateAssetMenu] //<-- Adds a menu item to Assets > Create.
    public class UrgencyFunctionTemplate : UrgencyFunction // Rename this class.
    {

        // Return a type name to display in custom editors. It should help
        // describe what this urgency function does.
        public override string typeName { get { return "MY TYPE NAME"; } }

        // Return a value (typically [0,100]) that indicates how urgent worldModel.observed 
        // is to worldModel.observer. For example, if the observer is a Knight who hates
        // Orcs, then if Orcs are observed this function should return a high number. If
        // Knight only mildly dislikes Wolves, then if Wolves are observed this function 
        // should return a lower number. The observer will generate a quest to address
        // the observed that returns the highest urgency value.
        public override float Compute(WorldModel worldModel)
        {
            return 0;
        }

    }
}

/**/