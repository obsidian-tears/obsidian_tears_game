using System;
using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    [Serializable]
    public class QuestGeneratorEntityTypeListGUI : QuestGeneratorAssetListGUI
    {
        public override Type AssetType { get { return typeof(EntityType); } }

        public override Type WrapperAssetType { get { return GetWrapperType(typeof(EntityType)); } }

        public override Texture2D Icon { get { return QuestEditorStyles.entityImage; } }

        public override string HelpText
        {
            get
            {
                return "Entity types are abstract definitions of entities. Every entity in the game world has an entity type that defines its attributes " +
                    "such as its faction with other entities and actions that can be performed on it. Use this page to manage the list of entity types. " +
                    "Click on an entity type's name to edit it.";
            }
        }
    }

}