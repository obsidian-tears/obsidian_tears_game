/* [REMOVE THIS LINE] 
 * [REMOVE THIS LINE] If your code references scripts or assets that are outside of the Plugins
 * [REMOVE THIS LINE] folder, move this script outside of the Plugins folder, too.

using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    /// This is a starter template for custom quest actions. To use it,
    /// make a copy, rename it, and remove the line marked above.
    /// Then fill in your code where indicated below.
    public class QuestActionTemplate : QuestAction // Rename this class.
    {

        public override void Execute()
        {
            base.Execute();
            // Add your code here. This code will run when the action runs.
        }

        // Uncomment and edit if you want to override the name shown in the editor
        // in the quest's Actions lists.
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