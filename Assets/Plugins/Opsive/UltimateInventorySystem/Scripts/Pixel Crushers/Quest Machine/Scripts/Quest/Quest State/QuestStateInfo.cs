// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Information about a specific quest state (Inactive, Active, etc.).
    /// </summary>
    [Serializable]
    public class QuestStateInfo
    {

        [Tooltip("Run these actions when this state becomes active.")]
        [SerializeField]
        private List<QuestAction> m_actionList = new List<QuestAction>();

        [Tooltip("Content for each UI category (dialogue, journal, HUD).")]
        [SerializeField]
        private List<QuestContentSet> m_categorizedContentList = new List<QuestContentSet>();

        public static int NumContentCategories = Enum.GetNames(typeof(QuestContentCategory)).Length - 3; // Omit Alert, Offer & OfferConditionsUnmet.

        /// <summary>
        /// Run these actions when this state becomes active.
        /// </summary>
        public List<QuestAction> actionList
        {
            get { return m_actionList; }
            set { m_actionList = value; }
        }

        /// <summary>
        /// Contents for each UI category (dialogue, journal, etc.).  This list is
        /// indexed by the int value of the QuestContentCategory enum, such as 
        /// <c>stateInfoList[(int)QuestContentCategory.Dialogue]</c>.
        /// </summary>
        public List<QuestContentSet> categorizedContentList
        {
            get { return m_categorizedContentList; }
            set { m_categorizedContentList = value; }
        }

        /// <summary>
        /// Constructs a new QuestStateInfo instance with empty lists for each
        /// content category.
        /// </summary>
        public QuestStateInfo()
        {
            if (categorizedContentList.Count >= NumContentCategories) return;
            for (int i = categorizedContentList.Count; i < NumContentCategories; i++)
            {
                categorizedContentList.Add(new QuestContentSet());
            }
        }

        public void SetRuntimeReferences(Quest quest, QuestNode questNode)
        {
            if (actionList != null)
            {
                for (int i = 0; i < actionList.Count; i++)
                {
                    if (actionList[i] != null) actionList[i].SetRuntimeReferences(quest, questNode);
                }
            }
            if (categorizedContentList != null)
            {
                for (int i = 0; i < categorizedContentList.Count; i++)
                {
                    if (categorizedContentList[i] != null) categorizedContentList[i].SetRuntimeReferences(quest, questNode);
                }
            }
        }

        #region UI Content

        /// <summary>
        /// Checks if the quest state has any UI content for a specified category.
        /// </summary>
        /// <param name="category">The content category to check.</param>
        /// <returns>true if there is content.</returns>
        public bool HasContent(QuestContentCategory category)
        {
            var contentList = GetContentList(category);
            return contentList != null && contentList.Count > 0;
        }

        /// <summary>
        /// Gets the UI content for a specified category.
        /// </summary>
        /// <param name="category">The content category for which to get content.</param>
        /// <returns>The UI content.</returns>
        public List<QuestContent> GetContentList(QuestContentCategory category)
        {
            var i = (int)category;
            if (categorizedContentList == null || i >= categorizedContentList.Count) return null;
            return m_categorizedContentList[i].contentList;
        }

        #endregion

        public void CloneSubassetsInto(QuestStateInfo copy)
        {
            // Assumes lists are identical except subassets haven't been copied.
            if (copy == null || copy.categorizedContentList == null)
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: QuestStateInfo.CloneSubassetsInto() failed because the destination copy or its content list is null.");
            }
            else if (m_actionList == null || m_categorizedContentList == null)
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: QuestStateInfo.CloneSubassetsInto() failed because the original state info's action list or content list is null.");
            }
            else if (copy.categorizedContentList.Count != m_categorizedContentList.Count)
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: QuestStateInfo.CloneSubassetsInto() failed because the destination copy's content list is a different size than the original's.");
            }
            else
            {
                copy.actionList = QuestSubasset.CloneList(m_actionList);
                for (int i = 0; i < m_categorizedContentList.Count; i++)
                {
                    if (copy.categorizedContentList[i] == null) copy.categorizedContentList[i] = new QuestContentSet();
                    copy.categorizedContentList[i].contentList = QuestSubasset.CloneList(m_categorizedContentList[i].contentList);
                }
            }
        }

        public void DestroySubassets()
        {
            QuestSubasset.DestroyList(actionList);
            if (categorizedContentList != null)
            {
                for (int i = 0; i < categorizedContentList.Count; i++)
                {
                    categorizedContentList[i].DestroySubassets();
                }
            }
        }

        #region Static Helper Methods

        public static int GetNumQuestStates()
        {
            return Enum.GetNames(typeof(QuestState)).Length;
        }

        public static int GetNumQuestNodeStates()
        {
            return Enum.GetNames(typeof(QuestNodeState)).Length;
        }

        public static QuestStateInfo GetStateInfo(List<QuestStateInfo> stateInfoList, QuestState questState)
        {
            ValidateStateInfoListCount(stateInfoList, GetNumQuestStates());
            return stateInfoList[(int)questState];
        }

        public static QuestStateInfo GetStateInfo(List<QuestStateInfo> stateInfoList, QuestNodeState questNodeState)
        {
            ValidateStateInfoListCount(stateInfoList, GetNumQuestNodeStates());
            return stateInfoList[(int)questNodeState];
        }

        public static void ValidateStateInfoListCount(List<QuestStateInfo> stateInfoList, int numStates = -1)
        {
            if (numStates == -1) numStates = GetNumQuestNodeStates(); // Default: QuestNodeState size.
            if (stateInfoList.Count >= numStates) return;
            for (int i = stateInfoList.Count; i < numStates; i++)
            {
                stateInfoList.Add(new QuestStateInfo());
            }
        }

        public static void ValidateCategorizedContentListCount(List<QuestContentSet> categorizedContentList, int numStates = -1)
        {
            if (numStates == -1) numStates = 3; // Default: Dialogue, Journal, HUD.
            if (categorizedContentList.Count >= numStates) return;
            for (int i = categorizedContentList.Count; i < numStates; i++)
            {
                categorizedContentList.Add(new QuestContentSet());
            }
        }
        public static void CloneSubassets(List<QuestStateInfo> original, List<QuestStateInfo> copy)
        {
            // Assumes lists are identical except subassets haven't been copied.
            if (original == null)
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: QuestStateInfo.CloneSubassets() failed because original is null.");
            }
            else if (copy == null)
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: QuestStateInfo.CloneSubassets() failed because copy is null.");
            }
            else if (copy.Count != original.Count)
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: QuestStateInfo.CloneSubassets() failed because original and copy are different sizes.");
            }
            else
            {
                for (int i = 0; i < original.Count; i++)
                {
                    original[i].CloneSubassetsInto(copy[i]);
                }
            }
        }

        public static void DestroyListSubassets(List<QuestStateInfo> list)
        {
            if (list == null) return;
            for (int i = 0; i < list.Count; i++)
            {
                list[i].DestroySubassets();
            }
        }

        #endregion

    }

}
