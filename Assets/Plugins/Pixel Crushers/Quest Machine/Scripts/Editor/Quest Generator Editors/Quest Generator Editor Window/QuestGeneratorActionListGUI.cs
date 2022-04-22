using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    [Serializable]
    public class QuestGeneratorActionListGUI : QuestGeneratorAssetListGUI
    {
        public override Type AssetType { get { return typeof(PixelCrushers.QuestMachine.Action); } }

        public override Type WrapperAssetType { get { return GetWrapperType(typeof(Action)); } }

        public override Texture2D Icon { get { return QuestEditorStyles.actionImage; } }

        public override string HelpText
        {
            get
            {
                return "Actions are tasks that the player can do to an entity. Quest givers generate quests by choosing actions to do to entities. " +
                    "Use this page to manage the list of actions. Click on an action's name to edit its properties. To edit the list of actions that " +
                    "can be done to an entity, go to the Entities page and click on the entity type.";
            }
        }

    }

}