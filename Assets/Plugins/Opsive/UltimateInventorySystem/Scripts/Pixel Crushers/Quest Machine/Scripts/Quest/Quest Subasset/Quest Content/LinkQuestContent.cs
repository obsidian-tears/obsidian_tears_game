// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Link UI content. Links to another quest content element.
    /// </summary>
    public class LinkQuestContent : QuestContent
    {

        [Tooltip("Linked content to show.")]
        [SerializeField]
        private int m_linkedContentID = -1;

        public int linkedContentID
        {
            get { return m_linkedContentID; }
            set { m_linkedContentID = value; }
        }

        public override string GetEditorName()
        {
            var linkedContent = (quest != null) ? quest.GetContentByID(linkedContentID) : null;
            return (linkedContent == null) ? "Link (unassigned)" : ("Linked to: " + linkedContent.GetEditorName());
        }

    }

}
