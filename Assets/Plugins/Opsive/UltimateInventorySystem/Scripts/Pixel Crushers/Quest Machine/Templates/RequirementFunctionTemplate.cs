/* [REMOVE THIS LINE] 
 * [REMOVE THIS LINE] If your code references scripts or assets that are outside of the Plugins
 * [REMOVE THIS LINE] folder, move this script outside of the Plugins folder, too.


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    // This is a starter template for a requirement function used by Quest Machine's
    // procedural quest generator. To use it,make a copy, rename it, and remove the
    // line marked above. Then fill in your code where indicated below. After creating
    // your script, right-click in the Project view to create an asset that you can
    // then assign to an Action asset's Requirements > Func field.
    public class RequirementFunctionTemplate : RequirementFunction // Rename this class.
    {

        // typeName is just used to display a human readable description in the editor.
        public override string typeName
        {
            get
            {
                return GetType().Name;
            }
        }

        // Checks if a given world model meets the requirement.
        public override bool IsTrue(WorldModel worldModel)
        {
            // Example code:
            //return worldModel.ContainsFact(someDomainType, someEntityType, minAmount, maxAmount);
            return true;
        }
    }
}

/**/
