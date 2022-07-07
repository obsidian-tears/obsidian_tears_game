using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace PixelCrushers.QuestMachine
{

    [Serializable]
    public class AssetInfo
    {
        public string path;
        public string name;
        public int instanceID;

        public const string AssetsPath = "Assets/";

        public string assetPath { get { return AssetsPath + path + ((string.IsNullOrEmpty(path)) ? string.Empty : "/") + name + ".asset"; } }

        public string pathAndName { get { return path + ((string.IsNullOrEmpty(path)) ? string.Empty : "/") + name; } }

        public AssetInfo() { }

        public AssetInfo(ScriptableObject asset)
        {
            var assetPath = AssetDatabase.GetAssetPath(asset);
            path = Path.GetDirectoryName(assetPath);
            path = (path.Length > AssetsPath.Length) ? path.Substring(AssetsPath.Length) : string.Empty;
            name = Path.GetFileNameWithoutExtension(assetPath);
            instanceID = asset.GetInstanceID();
        }
    }

}