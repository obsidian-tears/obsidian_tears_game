using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    [Serializable]
    public class QuestGeneratorDomainListGUI : QuestGeneratorAssetListGUI
    {
        public override Type AssetType { get { return typeof(DomainType); } }

        public override Type WrapperAssetType { get { return GetWrapperType(typeof(DomainType)); } }

        public override Texture2D Icon { get { return QuestEditorStyles.domainImage; } }

        public override string HelpText
        {
            get
            {
                return "Domain types are abstract definitions of domains. Domains are locations where entities are located. " +
                    "Use this page to manage the list of domain types. Click on a domain type's name to edit it.";
            }
        }
    }

}