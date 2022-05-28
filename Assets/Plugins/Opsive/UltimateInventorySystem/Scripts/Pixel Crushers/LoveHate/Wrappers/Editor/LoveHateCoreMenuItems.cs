// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using System.IO;

namespace PixelCrushers.LoveHate.Wrappers
{

    /// <summary>
    /// This class implements asset creation menu items.
    /// </summary>
    public static class LoveHateCoreMenuItems
    {

        /// <summary>
        /// Adds Assets > Create > Love/Hate > Faction Database.
        /// </summary>
        [MenuItem("Assets/Create/Pixel Crushers/Love\u2215Hate/Faction Database", false, 0)]
        public static void CreateFactionDatabase()
        {
            var asset = ScriptableObject.CreateInstance<FactionDatabase>() as FactionDatabase;
            asset.Initialize();
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            else if (!string.IsNullOrEmpty(Path.GetExtension(path)))
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New Faction Database.asset");
            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }

        /// <summary>
        /// Adds menu item GameObject > Love/Hate > Faction Manager.
        /// </summary>
        [MenuItem("GameObject/Love\u2215Hate/Faction Manager")]
        public static void CreateNewFactionManager()
        {
            new GameObject("Faction Manager", typeof(FactionManager));
        }

        /// <summary>
        /// Adds menu item GameObject > Love/Hate > Emotion Model.
        /// </summary>
        [MenuItem("Assets/Create/Pixel Crushers/Love\u2215Hate/Emotion Model", false, 2)]
        public static void CreateEmotionModel()
        {
            var asset = ScriptableObject.CreateInstance<EmotionModel>() as EmotionModel;
            //asset.Initialize(); // EmotionModel doesn't have Initialize.
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            else if (!string.IsNullOrEmpty(Path.GetExtension(path)))
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New Emotion Model.asset");
            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }

    }

}
