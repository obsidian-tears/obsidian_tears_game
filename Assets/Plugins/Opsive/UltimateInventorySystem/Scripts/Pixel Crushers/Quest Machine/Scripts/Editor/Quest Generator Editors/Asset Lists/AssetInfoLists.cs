using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Holds cached lists of quest assets for use by the Quest Generator editor window and
    /// individual quest asset editors.
    /// </summary>
    public static class AssetInfoLists
    {

        private static List<AssetInfo> m_entityList = null;
        private static List<AssetInfo> m_factionList = null;
        private static List<AssetInfo> m_driveList = null;
        private static List<AssetInfo> m_urgencyList = null;
        private static List<AssetInfo> m_actionList = null;
        private static List<AssetInfo> m_domainList = null;

        public enum SortBy { Name, Path }
        public static SortBy sortEntityBy = SortBy.Name;
        public static SortBy sortFactionBy = SortBy.Name;
        public static SortBy sortDriveBy = SortBy.Name;
        public static SortBy sortUrgencyBy = SortBy.Name;
        public static SortBy sortActionBy = SortBy.Name;
        public static SortBy sortDomainBy = SortBy.Name;

        public static bool IsType(System.Type type, System.Type typeToCheck)
        {
            return (typeToCheck == type) || typeToCheck.IsSubclassOf(type);
        }

        public static List<AssetInfo> GetList(System.Type type)
        {
            if (IsType(typeof(EntityType), type))
            {
                if (m_entityList == null) m_entityList = PopulateList(type, sortEntityBy);
                return m_entityList;
            }
            else if (IsType(typeof(Faction), type))
            {
                if (m_factionList == null) m_factionList = PopulateList(type, sortFactionBy);
                return m_factionList;
            }
            else if (IsType(typeof(Drive), type))
            {
                if (m_driveList == null) m_driveList = PopulateList(type, sortFactionBy);
                return m_driveList;
            }
            else if (IsType(typeof(UrgencyFunction), type))
            {
                if (m_urgencyList == null) m_urgencyList = PopulateList(type, sortFactionBy);
                return m_urgencyList;
            }
            else if (IsType(typeof(PixelCrushers.QuestMachine.Action), type))
            {
                if (m_actionList == null) m_actionList = PopulateList(type, sortFactionBy);
                return m_actionList;
            }
            else if (IsType(typeof(DomainType), type))
            {
                if (m_domainList == null) m_domainList = PopulateList(type, sortDomainBy);
                return m_domainList;
            }
            else
            {
                return null;
            }
        }

        public static void RefreshList(System.Type type)
        {
            if (IsType(typeof(EntityType), type))
            {
                m_entityList = null;
            }
            else if (IsType(typeof(Faction), type))
            {
                m_factionList = null;
            }
            else if (IsType(typeof(Drive), type))
            {
                m_driveList = null;
            }
            else if (IsType(typeof(UrgencyFunction), type))
            {
                m_urgencyList = null;
            }
            else if (IsType(typeof(PixelCrushers.QuestMachine.Action), type))
            {
                m_actionList = null;
            }
            else if (IsType(typeof(DomainType), type))
            {
                m_domainList = null;
            }
            else
            {
                return;
            }
            QuestGeneratorEditorWindow.RepaintNow();
        }

        private static List<AssetInfo> PopulateList(System.Type type, SortBy sortBy)
        {
            var list = new List<AssetInfo>();
            var guids = AssetDatabase.FindAssets("t:" + type.Name);
            if (guids != null)
            {
                for (int i = 0; i < guids.Length; i++)
                {
                    try
                    {
                        var assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                        var asset = AssetDatabase.LoadAssetAtPath(assetPath, type) as ScriptableObject;
                        if (asset != null)
                        {
                            list.Add(new AssetInfo(asset));
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError(e.Message);
                    }
                }
            }
            Sort(list, sortBy);
            return list;
        }

        public static void SortList(System.Type type, SortBy sortBy)
        {
            if (IsType(typeof(EntityType), type))
            {
                sortEntityBy = sortBy;
                Sort(m_entityList, sortBy);
            }
            else if (IsType(typeof(Faction), type))
            {
                sortFactionBy = sortBy;
                Sort(m_factionList, sortBy);
            }
            else if (IsType(typeof(Drive), type))
            {
                sortDriveBy = sortBy;
                Sort(m_driveList, sortBy);
            }
            else if (IsType(typeof(UrgencyFunction), type))
            {
                sortUrgencyBy = sortBy;
                Sort(m_urgencyList, sortBy);
            }
            else if (IsType(typeof(PixelCrushers.QuestMachine.Action), type))
            {
                sortActionBy = sortBy;
                Sort(m_actionList, sortBy);
            }
            else if (IsType(typeof(DomainType), type))
            {
                sortDomainBy = sortBy;
                Sort(m_domainList, sortBy);
            }
        }

        private static void Sort(List<AssetInfo> list, SortBy sortBy)
        {
            if (list == null) return;
            switch (sortBy)
            {
                case SortBy.Name:
                    list.Sort((x, y) => x.name.CompareTo(y.name));
                    break;
                case SortBy.Path:
                    list.Sort((x, y) => x.path.CompareTo(y.path));
                    break;
            }
        }

    }

}
