/* [REMOVE THIS LINE] 
 * [REMOVE THIS LINE] If your code references scripts or assets that are outside of the Plugins
 * [REMOVE THIS LINE] folder, move this script outside of the Plugins folder, too.

using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    /// This is a starter template for custom quest conditions. To use it,
    /// make a copy, rename it, and remove the line marked above.
    /// Then fill in your code where indicated below.
    public class QuestConditionTemplate : QuestCondition // Rename this class.
    {

        public override void StartChecking(System.Action trueAction)
        {
            base.StartChecking(trueAction);
            // Add your code here to start checking your condition.
            // When the condition is true, call SetTrue().
        }

        public override void StopChecking()
        {
            base.StopChecking();
            // Add your code here to stop checking your condition.
        }

        // Uncomment and edit if you want to override the name shown in the editor
        // in the quest's Conditions lists.
        //public override string GetEditorName()
        //{
        //    return base.GetEditorName();
        //}

        // Uncomment and edit if you need to save some data to a serializable field
        // (for example, a dictionary to two lists).
        //public override void OnBeforeProxySerialization()
        //{
        //    base.OnBeforeProxySerialization();
        //}

        // Uncomment and edit if you need to copy serialized data back to its original form.
        //public override void OnAfterProxyDeserialization()
        //{
        //    base.OnAfterProxyDeserialization();
        //}
    }

}

/**/