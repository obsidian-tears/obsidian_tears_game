// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Maintains a dictionary of tags and their values.
    /// </summary>
    [Serializable]
    public class TagDictionary : ISerializationCallbackReceiver
    {

        [SerializeField]
        private List<string> m_keys = new List<string>();

        [SerializeField]
        private List<string> m_values = new List<string>();

        [NonSerialized]
        private Dictionary<string, string> m_dict = new Dictionary<string, string>();

        private List<string> keys { get { return m_keys; } }

        private List<string> values { get { return m_values; } }

        [System.Xml.Serialization.XmlIgnore]
        public Dictionary<string, string> dict
        {
            get { return m_dict; }
            set { m_dict = value; }
        }

        public TagDictionary() { }

        public TagDictionary(TagDictionary source)
        {
            foreach (var kvp in source.dict)
            {
                dict.Add(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// True if the dictionary contains the specified tag, false otherwise.
        /// </summary>
        public bool ContainsTag(string tag)
        {
            return dict.ContainsKey(tag);
        }

        /// <summary>
        /// Looks up the value of a tag.
        /// </summary>
        public string GetTagValue(string tag, string defaultValue)
        {
            return dict.ContainsKey(tag) ? dict[tag] : defaultValue;
        }

        /// <summary>
        /// Records a static tag and its value in the dictionary.
        /// </summary>
        /// <param name="tag">Tag.</param>
        /// <param name="value">Value.</param>
        public void SetTag(string tag, string value)
        {
            if (dict.ContainsKey(tag))
            {
                dict[tag] = value;
            }
            else
            {
                dict.Add(tag, value);
            }
        }

        public void CopyInto(TagDictionary other)
        {
            foreach (var kvp in dict)
            {
                if (!other.dict.ContainsKey(kvp.Key)) other.dict.Add(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Records a static tag and its value in the dictionary.
        /// </summary>
        public void SetTag(string tag, StringField value)
        {
            SetTag(tag, StringField.GetStringValue(value));
        }

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (var kvp in dict)
            {
                keys.Add(kvp.Key);
                values.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            dict.Clear();
            for (int i = 0; i < Mathf.Min(keys.Count, values.Count); i++)
            {
                dict.Add(keys[i], values[i]);
            }
        }
    }
}
