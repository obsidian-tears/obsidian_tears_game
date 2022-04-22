using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    [Serializable]
    public class QuestGeneratorFactionListGUI : QuestGeneratorAssetListGUI
    {
        public override Type AssetType { get { return typeof(Faction); } }

        public override Type WrapperAssetType { get { return GetWrapperType(typeof(Faction)); } }

        public override Texture2D Icon { get { return QuestEditorStyles.factionImage; } }

        public override string HelpText
        {
            get
            {
                return "Factions define relationships between entities, which influence the quests that quest givers " +
                "generate. Use this page to manage factions. Click on a faction's name to edit it. To assign a faction to " +
                "an entity, go to the Entities page and click on the entity type.";
            }
        }
    }

}