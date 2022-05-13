// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using PixelCrushers.QuestMachine.DialogueSystemSupport;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Utility class for procedurally building quests.
    /// </summary>
    public class QuestBuilderWithDialogueSystem : QuestBuilder
    {
        public QuestBuilderWithDialogueSystem(StringField name) : base(name) { }
        public QuestBuilderWithDialogueSystem(string name) : base(name) { }
        public QuestBuilderWithDialogueSystem(string name, StringField id, StringField title) : base(name, id, title) { }
        public QuestBuilderWithDialogueSystem(string name, string id, string title) : base(name, id, title) { }
        public QuestBuilderWithDialogueSystem(Quest quest) : base(quest) { }

        /// <summary>
        /// Creates a DialogueSystemConversationQuestContent object that can be added
        /// to the quest using AddContent.
        /// </summary>
        /// <param name="conversationTitle">Title of conversation in dialogue database.</param>
        public QuestContent CreateConversationContent(StringField conversationTitle)
        {
            return CreateConversationContent(StringField.GetStringValue(conversationTitle));
        }

        /// <summary>
        /// Creates a DialogueSystemConversationQuestContent object that can be added
        /// to the quest using AddContent.
        /// </summary>
        /// <param name="conversationTitle">Title of conversation in dialogue database.</param>
        public QuestContent CreateConversationContent(string conversationTitle)
        {
            if (string.IsNullOrEmpty(conversationTitle))
            {
                if (Debug.isDebugBuild) Debug.LogError(GetType().Name + ".CreateConversationContent can't create content because conversation title is blank.");
                return null;
            }
            var content = DialogueSystemConversationQuestContent.CreateInstance<DialogueSystemConversationQuestContent>();
            content.name = "conversation";
            content.conversation = conversationTitle;
            return content;
        }

        /// <summary>
        /// Creates a DialogueSystemTextQuestContent object that can be added to the
        /// quest using AddContent.
        /// </summary>
        /// <param name="text">Text that will be processed using Dialogue System markup tags.</param>
        public QuestContent CreateDialogueSystemTextContent(StringField text)
        {
            return CreateDialogueSystemTextContent(StringField.GetStringValue(text));
        }

        /// <summary>
        /// Creates a DialogueSystemTextQuestContent object that can be added to the
        /// quest using AddContent.
        /// </summary>
        /// <param name="text">Text that will be processed using Dialogue System markup tags.</param>
        public QuestContent CreateDialogueSystemTextContent(string text)
        {
            var content = DialogueSystemTextQuestContent.CreateInstance<DialogueSystemTextQuestContent>();
            content.name = "formattedText";
            content.bodyText = new StringField(text);
            return content;
        }

    }
}
