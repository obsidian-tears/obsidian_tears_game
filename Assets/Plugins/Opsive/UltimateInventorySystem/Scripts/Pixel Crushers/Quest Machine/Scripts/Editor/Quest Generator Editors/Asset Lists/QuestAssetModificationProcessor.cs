using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Intercepts modifications to quest assets to update the cached lists in AssetInfoLists.
    /// </summary>
    public class QuestAssetModificationProcessor : UnityEditor.AssetModificationProcessor
    {

        public static void OnWillCreateAsset(string path)
        {
            if (path == null || !path.EndsWith(".asset")) return;
            if (path.EndsWith("Entity Type.asset") || path.Contains("EntityType.asset"))
            {
                AssetInfoLists.RefreshList(typeof(EntityType));
            }
            else if (path.EndsWith("Faction.asset"))
            {
                AssetInfoLists.RefreshList(typeof(Faction));
            }
            else if (path.EndsWith("Drive.asset"))
            {
                AssetInfoLists.RefreshList(typeof(Drive));
            }
            else if (path.EndsWith("Urgency Function.asset") || path.Contains("UrgencyFunction.asset"))
            {
                AssetInfoLists.RefreshList(typeof(UrgencyFunction));
            }
            else if (path.EndsWith("Action.asset"))
            {
                AssetInfoLists.RefreshList(typeof(PixelCrushers.QuestMachine.Action));
            }
            else if (path.EndsWith("Entity Specifier.asset") || path.Contains("EntitySpecifier.asset"))
            {
                AssetInfoLists.RefreshList(typeof(EntitySpecifier));
            }
            else if (path.EndsWith("Domain Specifier.asset") || path.Contains("DomainSpecifier.asset"))
            {
                AssetInfoLists.RefreshList(typeof(DomainSpecifier));
            }
        }

        public static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions removeAssetOptions)
        {
            if (!path.EndsWith(".asset")) return AssetDeleteResult.DidNotDelete;
            var asset = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
            if (asset != null) AssetInfoLists.RefreshList(asset.GetType());
            return AssetDeleteResult.DidNotDelete;
        }

        public static AssetMoveResult OnWillMoveAsset(string oldPath, string newPath)
        {
            if (!oldPath.EndsWith(".asset")) return AssetMoveResult.DidNotMove;
            var asset = AssetDatabase.LoadAssetAtPath(oldPath, typeof(UnityEngine.Object));
            if (asset != null) AssetInfoLists.RefreshList(asset.GetType());
            return AssetMoveResult.DidNotMove;
        }

    }
}