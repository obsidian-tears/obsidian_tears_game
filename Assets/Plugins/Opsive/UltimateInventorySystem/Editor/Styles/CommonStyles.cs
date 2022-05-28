/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Editor.Styles
{
    using Opsive.Shared.Editor.Utility;
    using UnityEngine.UIElements;

    /// <summary>
    /// A static class that compiles the common styles.
    /// </summary>
    public static class CommonStyles
    {
        public static StyleSheet StyleSheet =>
            EditorUtility.LoadAsset<StyleSheet>("4ff86fbeece30534ca88317c5f5702b9");

        public static readonly string VerticalLayout = "vertical-layout";
        public static readonly string HorizontalAlignCenter = "horizontal-align-center";
        public static readonly string AlignChildrenCenter = "align-children-center";

        public static readonly string AddListItemContainer = "add-list-item-container";

        public static readonly string FlexGrow = "flex-grow";
        public static readonly string FlexWrap = "flex-wrap";

        public static readonly string ReverseToggle = "reverse-toggle";

        public static readonly string SearchList = "search-list";
        public static readonly string SearchList_SearchSortContainer = "search-list__search-sort-container";
        public static readonly string SearchList_FilterPresetContainer = "search-list__filter-preset-container";

        public static readonly string ObjectPreview = "object-preview";
        public static readonly string ObjectPreviewSmall = "object-preview-small";

        public static readonly string ShrinkZero = "shrink-zero";
    }
}
