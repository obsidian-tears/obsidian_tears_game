// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Icon UI content (image with optional count and caption) with an optional
    /// list of actions. If the list of actions is non-empty, the icon acts as a
    /// button.
    /// </summary>
    public class IconQuestContent : QuestContent
    {

        [Tooltip("The image.")]
        [SerializeField]
        private Sprite m_image;

        [Tooltip("The caption. Can be blank.")]
        [SerializeField]
        private StringField m_caption;

        [Tooltip("The count to show on the count label. If 0 or 1, count is not shown.")]
        [SerializeField]
        private int m_count;

        [HideInInspector]
        [SerializeField]
        private string m_imageNameProxy; // Temporary variable for proxy serialization.

        /// <summary>
        /// The image.
        /// </summary>
        public Sprite image
        {
            get { return m_image; }
            set { m_image = value; }
        }

        /// <summary>
        /// The caption. Can be blank.
        /// </summary>
        public virtual StringField caption
        {
            get { return m_caption; }
            set { m_caption = value; }
        }

        /// <summary>
        /// The count to show on the count label. If 0 or 1, count is not shown.
        /// </summary>
        public virtual int count
        {
            get { return m_count; }
            set { m_count = value; }
        }

        public override StringField originalText
        {
            get { return caption; }
            set { caption = value; }
        }

        public virtual string captionText { get { return StringField.GetStringValue(caption); } }

        public override string GetEditorName()
        {
            return !StringField.IsNullOrEmpty(m_caption) ? ("Icon: " + ((m_count > 1) ? m_count + " " : string.Empty) + m_caption)
                : ((m_image != null) ? "Icon: " + m_image.name : "Icon");
        }

        public override void OnBeforeProxySerialization()
        {
            base.OnBeforeProxySerialization();
            m_imageNameProxy = QuestMachine.GetImagePath(m_image);
        }

        public override void OnAfterProxyDeserialization()
        {
            base.OnAfterProxyDeserialization();
            m_image = QuestMachine.GetImage(m_imageNameProxy);
            m_imageNameProxy = null; // Free memory.
        }

    }

}
