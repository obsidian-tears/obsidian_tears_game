// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Quest action that plays a Dialogue System bark.
    /// </summary>
    public class BarkQuestAction : QuestAction
    {

        public enum Source { Conversation, String }

        [SerializeField]
        private Source m_source;

        [Tooltip("Conversation to bark from.")]
        [ConversationPopup(true)]
        [SerializeField]
        private string m_conversation;

        [Tooltip("Literal text to bark. Respects localization if a text table is assigned to the Dialogue Manager.")]
        [SerializeField]
        private StringField m_barkText;

        [Tooltip("GameObject that will bark. Must have a bark UI.")]
        [SerializeField]
        private StringField m_barker;

        [Tooltip("If this bark needs to wait in a queue, specify a duration to delay after exiting the queue. Otherwise it will delay using bark group member's delay settings.")]
        [SerializeField]
        private bool m_specifyDelay;

        [Tooltip("Duration to delay if waiting in bark group queue. If Specify Delay is unticked, will delay using bark group member's delay settings instead.")]
        [SerializeField]
        private float m_delay;

        [Tooltip("Optional sequence to play.")]
        [SerializeField]
        private StringField m_sequence;

        public Source source
        {
            get { return m_source; }
            set { m_source = value; }
        }

        public string conversation
        {
            get { return m_conversation; }
            set { m_conversation = value; }
        }

        public StringField barkText
        {
            get { return m_barkText; }
            set { m_barkText = value; }
        }

        public StringField barker
        {
            get { return m_barker; }
            set { m_barker = value; }
        }

        public bool specifyDelay
        {
            get { return m_specifyDelay; }
            set { m_specifyDelay = value; }
        }

        public float delay
        {
            get { return m_delay; }
            set { m_delay = value; }
        }

        public StringField sequence
        {
            get { return m_sequence; }
            set { m_sequence = value; }
        }

        public override string GetEditorName()
        {
            return "Bark [" + barker + "]: " + ((source == Source.Conversation) ? conversation : StringField.GetStringValue(barkText));
        }

        protected Transform GetBarkerTransform()
        {
            var name = StringField.GetStringValue(barker);
            if (string.IsNullOrEmpty(name)) return null;
            var t = PixelCrushers.DialogueSystem.CharacterInfo.GetRegisteredActorTransform(name);
            if (t != null) return t;
            var go = GameObject.Find(name);
            return (go != null) ? go.transform : null;
        }

        public override void Execute()
        {
            base.Execute();
            var barkerTransform = GetBarkerTransform();
            var barkGroupMember = (barkerTransform != null) ? barkerTransform.GetComponent<BarkGroupMember>() : null;
            var runtimeDelay = specifyDelay ? delay : -1;
            switch (source)
            {
                case Source.Conversation:
                    if (barkGroupMember != null)
                    {
                        barkGroupMember.GroupBark(conversation, null, null, runtimeDelay);
                    }
                    else
                    {
                        DialogueManager.Bark(conversation, barkerTransform);
                    }
                    break;
                case Source.String:
                    var runtimeText = DialogueManager.GetLocalizedText(StringField.GetStringValue(barkText));
                    if (barkGroupMember != null)
                    {
                        barkGroupMember.GroupBarkString(runtimeText, barkerTransform, StringField.GetStringValue(sequence), runtimeDelay);
                    }
                    else
                    {
                        DialogueManager.BarkString(runtimeText, GetBarkerTransform(), null, StringField.GetStringValue(sequence));
                    }
                    break;
            }
        }

    }

}
