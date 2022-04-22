// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using System.IO;

namespace PixelCrushers.LoveHate.Wrappers
{

    /// <summary>
    /// This class implements asset creation menu items.
    /// </summary>
    public static class LoveHateInteractionMenuItems
    {

        [MenuItem("Assets/Create/Pixel Crushers/Love\u2215Hate/Deed Template Library", false, 1)]
        public static void CreateDeedTemplateLibrary()
        {
            var asset = ScriptableObject.CreateInstance<DeedTemplateLibrary>() as DeedTemplateLibrary;
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            else if (!string.IsNullOrEmpty(Path.GetExtension(path)))
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New Deed Template Library.asset");
            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }

    }

}
