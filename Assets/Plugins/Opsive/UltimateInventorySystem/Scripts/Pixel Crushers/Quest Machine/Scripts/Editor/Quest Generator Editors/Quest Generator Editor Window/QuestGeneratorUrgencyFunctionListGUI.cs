using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PixelCrushers.QuestMachine
{

    [Serializable]
    public class QuestGeneratorUrgencyFunctionListGUI : QuestGeneratorAssetListGUI
    {
        public override Type AssetType { get { return typeof(UrgencyFunction); } }

        public override Type WrapperAssetType { get { return GetWrapperType(typeof(UrgencyFunction)); } }

        public override Texture2D Icon { get { return QuestEditorStyles.urgencyFunctionImage; } }

        public override string HelpText
        {
            get
            {
                return "Urgency functions are attached to entities. They tell quest givers how urgent it is to generate a quest about the entity. " +
                    "Quest Machine ships with a library of different urgency function types. You can also write your own. Use this page to manage the list " +
                    "of urgency functions. The New button will display a menu of available urgency function types from which you can create an urgency " +
                    "function. Click on an urgency function's name to edit its properties. To assign urgency functions to an entity, go to the Entities page " +
                    "and click on the entity type.";
            }
        }

        protected override void CreateNewAsset(List<AssetInfo> assetInfoList)
        {
            // Create a list of all urgency function types, excluding abstract types and 
            // types that have wrappers:
            var list = new List<System.Type>();
            var assemblies = RuntimeTypeUtility.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                try
                {
                    var assemblyList = (from assemblyType in assembly.GetExportedTypes()
                        where typeof(UrgencyFunction).IsAssignableFrom(assemblyType)
                        select assemblyType).ToArray();
                    list.AddRange(assemblyList);
                }
                catch (System.Exception)
                {
                    // Ignore exceptions and move on to next assembly.
                }
            }
            var menu = new GenericMenu();
            for (int i = 0; i < list.Count; i++)
            {
                var type = list[i];
                if (type == null) continue;
                if (type.IsAbstract) continue;
                if (QuestEditorUtility.HasWrapperType(type)) continue;
                menu.AddItem(new GUIContent(type.Name), false, OnClickNewUrgencyFunction, type);
            }
            menu.ShowAsContext();
        }

        private void OnClickNewUrgencyFunction(object data)
        {
            var type = data as System.Type;
            if (type == null) return;
            var assetInfoList = AssetInfoLists.GetList(AssetType);
            if (assetInfoList == null) return;
            var wrapperType = QuestEditorUtility.GetWrapperType(type);
            if (wrapperType != null) type = wrapperType;
            var newAsset = AssetUtility.CreateAsset(type, AssetType.Name, true);
            assetInfoList.Add(new AssetInfo(newAsset));
        }

    }

}