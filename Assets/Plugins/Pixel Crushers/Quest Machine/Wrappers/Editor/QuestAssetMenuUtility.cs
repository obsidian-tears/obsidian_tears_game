// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine.Wrappers
{

    public static class QuestAssetMenuUtility
    {

        [MenuItem("Assets/Create/Pixel Crushers/Quest Machine/Quest", false, 0)]
        public static void CreateAsset()
        {
            var quest = AssetUtility.CreateAsset<Quest>("Quest");
            quest.isInstance = false;
            EditorUtility.SetDirty(quest);
        }

    }
}
