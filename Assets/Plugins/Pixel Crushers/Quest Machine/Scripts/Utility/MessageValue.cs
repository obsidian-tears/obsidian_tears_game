// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    public enum MessageValueType { None, Int, String }

    /// <summary>
    /// Specifies a value passed with a message and parameter.
    /// </summary>
    [Serializable]
    public class MessageValue
    {
        [Tooltip("Type of optional value to pass with message.")]
        [SerializeField]
        private MessageValueType m_valueType = MessageValueType.None;

        [Tooltip("Optional int value to pass with message.")]
        [SerializeField]
        private int m_intValue;

        [Tooltip("Optional string value to pass with message.")]
        [SerializeField]
        private string m_stringValue;

        /// <summary>
        /// Type of optional value to pass with message (int or string).
        /// </summary>
        public MessageValueType valueType
        {
            get { return m_valueType; }
            set { m_valueType = value; if (value != MessageValueType.String) m_stringValue = null; }
        }

        /// <summary>
        /// Optional int value to pass with message.
        /// </summary>
        public int intValue
        {
            get { return m_intValue; }
            set { valueType = MessageValueType.Int; m_intValue = value; m_stringValue = null; }
        }

        /// <summary>
        /// Optional string value to pass with message.
        /// </summary>
        public string stringValue
        {
            get { return m_stringValue; }
            set { valueType = MessageValueType.String; m_stringValue = value; }
        }

        public MessageValue()
        {
        }

        public MessageValue(int i)
        {
            m_valueType = MessageValueType.Int;
            m_intValue = i;
        }

        public MessageValue(string s)
        {
            m_valueType = MessageValueType.String;
            m_stringValue = s;
        }

        public MessageValue(StringField sf)
        {
            m_valueType = MessageValueType.String;
            m_stringValue = StringField.GetStringValue(sf);
        }

        public override string ToString()
        {
            switch (valueType)
            {
                case MessageValueType.Int:
                    return intValue.ToString();
                case MessageValueType.String:
                    return stringValue;
                default:
                    return "MessageValue";
            }
        }

        public string EditorNameValue()
        {
            switch (valueType)
            {
                case MessageValueType.Int:
                    return intValue.ToString();
                case MessageValueType.String:
                    return stringValue;
                default:
                    return string.Empty;
            }
        }

    }

}
