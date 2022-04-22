using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    [Serializable]
    public class QuestGeneratorAssetListGUI
    {

        public virtual Type AssetType { get { return typeof(ScriptableObject); } }

        public virtual Type WrapperAssetType { get { return typeof(ScriptableObject); } }

        public virtual Texture2D Icon { get { return null; } }

        public virtual string HelpText { get { return string.Empty; } }

        [SerializeField]
        protected bool sortByName = true;

        protected const float ButtonWidth = 60;

        public Type GetWrapperType(Type defaultType)
        {
            if (defaultType == null) return null;
            var typeName = defaultType.Name;
            var type = Type.GetType("PixelCrushers.QuestMachine.Wrappers." + typeName + ", Assembly-CSharp-firstpass");
            if (type != null) return type;
            type = Type.GetType("PixelCrushers.QuestMachine.Wrappers." + typeName + ", Assembly-CSharp");
            if (type != null) return type;
            return defaultType;
        }

        public virtual void Draw(float width)
        {
            var assetInfoList = AssetInfoLists.GetList(AssetType);
            if (assetInfoList == null) return;
            DrawTopInfo(assetInfoList);
            DrawAssetList(assetInfoList, width);
        }

        protected virtual void DrawTopInfo(List<AssetInfo> assetInfoList)
        {
            var createNew = false;
            try
            {
                GUILayout.BeginHorizontal();
                if (Icon != null) GUILayout.Box(Icon, GUILayout.Width(48), GUILayout.Height(48));
                EditorGUILayout.HelpBox(HelpText, MessageType.None, true);
                try
                {
                    GUILayout.BeginVertical(GUILayout.Width(ButtonWidth));
                    if (GUILayout.Button(new GUIContent("Refresh", "Rescan the project for " + AssetType.Name + " assets."), GUILayout.Width(ButtonWidth))) AssetInfoLists.RefreshList(AssetType);
                    if (GUILayout.Button(new GUIContent("New", "Create a new " + AssetType.Name + "."), GUILayout.Width(ButtonWidth))) createNew = true;
                }
                finally
                {
                    GUILayout.EndVertical();
                }
            }
            finally
            {
                GUILayout.EndHorizontal();
            }
            if (createNew) CreateNewAsset(assetInfoList);
        }

        protected virtual void DrawAssetList(List<AssetInfo> assetInfoList, float width)
        {
            var fieldWidth = (width - ButtonWidth) / 2;
            // Heading:
            try
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent("Name", "Sort by name."), GUILayout.Width(fieldWidth))) AssetInfoLists.SortList(AssetType, AssetInfoLists.SortBy.Name);
                if (GUILayout.Button(new GUIContent("Folder Path", "Sort by folder path."), GUILayout.Width(fieldWidth))) AssetInfoLists.SortList(AssetType, AssetInfoLists.SortBy.Path);
                GUILayout.Label(string.Empty, GUILayout.Width(ButtonWidth));
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }
            // Rows:
            if (assetInfoList == null) return;
            var indexToDelete = -1;
            for (int i = 0; i < assetInfoList.Count; i++)
            {
                var assetInfo = assetInfoList[i];
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(assetInfo.name, EditorStyles.textField, GUILayout.Width(fieldWidth)))
                {
                    Selection.activeObject = EditorUtility.InstanceIDToObject(assetInfo.instanceID);
                    EditorGUIUtility.PingObject(assetInfo.instanceID);
                }
                EditorGUI.BeginDisabledGroup(true);
                GUILayout.TextField(assetInfo.path, GUILayout.Width(fieldWidth));
                EditorGUI.EndDisabledGroup();
                if (GUILayout.Button(new GUIContent("Delete", "Delete " + assetInfo.name + "."), GUILayout.Width(ButtonWidth)))
                {
                    if (EditorUtility.DisplayDialog("Delete " + AssetType.Name + "?", assetInfo.name + "\n\nYou cannot undo this action.", "Delete", "Cancel"))
                    {
                        indexToDelete = i;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            if (indexToDelete != -1) DeleteAsset(assetInfoList, indexToDelete);
        }

        protected virtual void CreateNewAsset(List<AssetInfo> assetInfoList)
        {
            var newAsset = AssetUtility.CreateAsset(WrapperAssetType, AssetType.Name, true);
            assetInfoList.Add(new AssetInfo(newAsset));
        }

        protected virtual void DeleteAsset(List<AssetInfo> assetInfoList, int indexToDelete)
        {
            if (!(assetInfoList != null && 0 <= indexToDelete && indexToDelete < assetInfoList.Count)) return;
            var assetInfo = assetInfoList[indexToDelete];
            AssetDatabase.DeleteAsset(assetInfo.assetPath);
            AssetInfoLists.RefreshList(AssetType);
        }

    }

}