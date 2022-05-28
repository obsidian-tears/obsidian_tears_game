using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    [Serializable]
    public class QuestGeneratorDriveListGUI : QuestGeneratorAssetListGUI
    {
        public override Type AssetType { get { return typeof(Drive); } }

        public override Type WrapperAssetType { get { return GetWrapperType(typeof(Drive)); } }

        public override Texture2D Icon { get { return QuestEditorStyles.driveImage; } }

        public override string HelpText
        {
            get
            {
                return "Drive values define quest givers' personalities, which influence the quests they generate. Use this page to manage the list " +
                "of drives. Click on a drive's name to edit its description. To assign a quest giver's drive values, go to the Entities page and click on " +
                "the quest giver's entity type.";
            }
        }
    }

}