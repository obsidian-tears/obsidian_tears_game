// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Quest Machine-specific string constants (tags) and utility methods to 
    /// replace tags with their runtime values.
    /// </summary>
    public static class QuestMachineTags
    {

        #region Constants

        /// <summary>
        /// All tags start with an open brace.
        /// 
        /// Examples:
        /// - {QUESTGIVER}: Name of quest's giver.
        /// - {Hello}: Looks up "Hello" in the default text table.
        /// - {#numOrcsKilled}: Counter value.
        /// </summary>
        public const string TagPrefix = @"{";

        /// <summary>
        /// All counter value tags start with an open brace and hash sign.
        /// Format can be {#counter} or {#questID:counter}.
        /// 
        /// Examples:
        /// - {#numOrcsKilled}
        /// - {#farming:numApplesPicked}
        /// </summary>
        public const string CounterValueTagPrefix = @"{#";

        /// <summary>
        /// All counter min value tags start with an open brace and less-than sign.
        /// </summary>
        public const string CounterMinValueTagPrefix = @"{<#";

        /// <summary>
        /// All counter max value tags start with an open brace and greater-than sign.
        /// </summary>
        public const string CounterMaxValueTagPrefix = @"{>#";

        /// <summary>
        /// Tag to show a counter value as HH:MM:SS time.
        /// Format can be {:counter} or {:questID:counter}.
        /// 
        /// Examples:
        /// - {:secondsLeft}
        /// </summary>
        public const string CounterTimeValueTagPrefix = @"{:";

        /// <summary>
        /// Separator character between quest name and counter name.
        /// </summary>
        public const string CounterTagQuestNameSeparator = @":";

        /// <summary>
        /// The quest's ID.
        /// </summary>
        public const string QUESTID = @"{QUESTID}";

        /// <summary>
        /// The quest's title.
        /// </summary>
        public const string QUEST= @"{QUEST}";

        /// <summary>
        /// The display name of the quest's giver.
        /// </summary>
        public const string QUESTGIVER = @"{QUESTGIVER}";

        /// <summary>
        /// The ID of the quest's giver.
        /// </summary>
        public const string QUESTGIVERID = @"{QUESTGIVERID}";

        /// <summary>
        /// The display name of the quester.
        /// </summary>
        public const string QUESTER = @"{QUESTER}";

        /// <summary>
        /// The ID of the quest's quester.
        /// </summary>
        public const string QUESTERID = @"{QUESTERID}";

        /// <summary>
        /// The ID of the quester who is greeting the quest giver.
        /// </summary>
        public const string GREETERID = @"{GREETERID}";

        /// <summary>
        /// The display name of the quester who is greeting the quest giver.
        /// </summary>
        public const string GREETER = @"{GREETER}";

        // Generator tags:
        public const string DOMAIN = @"{DOMAIN}";
        public const string ACTION = @"{ACTION}";
        public const string TARGETDESCRIPTOR = @"{TARGETDESCRIPTOR}";
        public const string TARGET = @"{TARGET}";
        public const string TARGETS = @"{TARGETS}";
        public const string COUNTERGOAL = @"{COUNTERGOAL}";
        public const string REWARD = @"{REWARD}";

        public static StringField QuestGiverIDStringField = new StringField(QUESTGIVERID);

        public static StringField QuesterIDStringField = new StringField(QUESTERID);

        private enum CounterTagType { Current, Min, Max, AsTime }

        #endregion

        #region Utility Methods

        private static bool ContainsAnyTag(StringField stringField)
        {
            return ContainsAnyTag(StringField.GetStringValue(stringField));
        }

        private static bool ContainsAnyTag(string s)
        {
            return s.Contains(TagPrefix);
        }

        private static bool IsDynamicTag(string s)
        {
            return !string.IsNullOrEmpty(s) &&
                (s.StartsWith(CounterValueTagPrefix) ||
                 s.StartsWith(CounterMinValueTagPrefix) ||
                 s.StartsWith(CounterMaxValueTagPrefix) ||
                 s.StartsWith(CounterTimeValueTagPrefix));
        }

        private static bool IsIDTag(string s)
        {
            return !string.IsNullOrEmpty(s) && (s.Equals(QUESTERID) || s.Equals(QUESTER) || s.Equals(QUESTGIVERID) || s.Equals(QUESTGIVER) ||
                s.Equals(DOMAIN) || s.Equals(ACTION) ||  s.Equals(TARGETDESCRIPTOR) || s.Equals(TARGET) || s.Equals(TARGETS) || 
                s.Equals(COUNTERGOAL) || s.Equals(REWARD));
        }

        /// <summary>
        /// Returns the ID string (pre-ReplaceTags) for a participant specifier.
        /// </summary>
        /// <param name="specifier"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static StringField GetIDBySpecifier(QuestMessageParticipant specifier, StringField id)
        {
            switch (specifier)
            {
                case QuestMessageParticipant.Any:
                    return StringField.empty;
                case QuestMessageParticipant.Quester:
                    return QuesterIDStringField;
                case QuestMessageParticipant.QuestGiver:
                    return QuestGiverIDStringField;
                default:
                    return id;
            }
        }

        #endregion

        #region Tag Dictionary Management

        /// <summary>
        /// Adds any tags in a StringField to a dictionary.
        /// </summary>
        /// <param name="staticTags">Dictionary.</param>
        /// <param name="stringField">StringField to scan for tags.</param>
        public static void AddTagsToDictionary(TagDictionary staticTags, StringField stringField)
        {
            AddTagsToDictionary(staticTags, StringField.GetStringValue(stringField));
        }

        /// <summary>
        /// Adds any tags in a string to a dictionary.
        /// </summary>
        /// <param name="staticTags">Dictionary.</param>
        /// <param name="stringField">string to scan for tags.</param>
        public static void AddTagsToDictionary(TagDictionary staticTags, string s)
        {
            if (!ContainsAnyTag(s) || staticTags == null) return;
            foreach (Match match in Regex.Matches(s, @"\{[^\}]+\}"))
            {
                var tag = match.Value;
                if (IsDynamicTag(tag)) continue;
                if (staticTags.ContainsTag(tag)) continue;
                staticTags.SetTag(tag, string.Empty);
            }
        }

        /// <summary>
        /// Associates tags in a tag dictionary with values from a primary text table 
        /// (i.e., quest giver's text table), or failing that QuestMachine's default text 
        /// table, or failing that the tag name itself. Leaves ID tags such as {QUESTERID}
        /// untouched.
        /// </summary>
        /// <param name="tagDictionary">The tag dictionary containing tags that need values assigned.</param>
        /// <param name="textTable">The primary text table from which to look up values.</param>
        public static void AddTagValuesToDictionary(TagDictionary tagDictionary, TextTable textTable)
        {
            if (tagDictionary == null) return;
            var newDict = new Dictionary<string, string>();
            foreach (var kvp in tagDictionary.dict)
            {
                var tag = kvp.Key;
                if (string.IsNullOrEmpty(tag) || tag.Length <= 2) continue;
                if (IsIDTag(tag))
                {
                    // Leave ID tags untouched:
                    newDict.Add(tag, kvp.Value);
                    continue;
                }
                var fieldName = tag.Substring(1, tag.Length - 2).Trim();
                if (textTable != null && textTable.HasField(fieldName))
                {
                    // Check in current text table;
                    newDict.Add(tag, PrepareFieldText(textTable.GetFieldText(fieldName)));
                }
                else if (QuestMachine.textTable != null && QuestMachine.textTable.HasField(fieldName))
                {
                    // Otherwise look in QuestMachine's text table:
                    newDict.Add(tag, PrepareFieldText(QuestMachine.textTable.GetFieldText(fieldName)));
                }
                else
                {
                    // Otherwise use the tag's text:
                    newDict.Add(tag, fieldName);
                }
            }
            tagDictionary.dict = newDict;
        }

        /// <summary>
        /// If the string has pipe characters, splits values on pipes and returns a random value.
        /// Otherwise returns the entire string.
        /// </summary>
        private static string PrepareFieldText(string s)
        {
            if (string.IsNullOrEmpty(s) || !s.Contains("|")) return s;
            var values = s.Split('|');
            return values[Random.Range(0, values.Length)];
        }

        #endregion

        #region Replace Tags

        private static Quest currentQuest { get; set; }
        private static TextTable currentTextTable { get; set; }
        private static TagDictionary questTagDictionary { get; set; }
        private static TagDictionary nodeTagDictionary { get; set; }

        private static TextTable m_fallbackTextTable = null;
        public static TextTable fallbackTextTable { get { return m_fallbackTextTable; } set { m_fallbackTextTable = value; } }

        /// <summary>
        /// Replaces the tags in a StringField.
        /// </summary>
        /// <returns>A string in which the tags have been replaced with their current values.</returns>
        public static string ReplaceTags(StringField stringField, Quest quest)
        {
            return ReplaceTags(StringField.GetStringValue(stringField), quest);
        }

        /// <summary>
        /// Replaces the tags in a string.
        /// </summary>
        /// <returns>A string in which the tags have been replaced with their current values.</returns>
        public static string ReplaceTags(string s, Quest quest)
        {
            if (!ContainsAnyTag(s)) return s;
            currentQuest = quest;
            currentTextTable = (quest != null && quest.currentSpeaker != null) ? quest.currentSpeaker.textTable : fallbackTextTable;
            questTagDictionary = (quest != null) ? quest.tagDictionary : null;
            nodeTagDictionary = GetActiveNodeTagDictionary(quest);
            var regex = new Regex(@"\{[^\}]+\}");
            return regex.Replace(s, new MatchEvaluator(ReplaceTag));
        }

        private static TagDictionary GetActiveNodeTagDictionary(Quest quest)
        {
            // Return the latest active node's tag dictionary:
            if (quest == null || quest.GetState() != QuestState.Active) return null;
            TagDictionary dict = null;
            for (int i = 0; i < quest.nodeList.Count; i++)
            {
                var node = quest.nodeList[i];
                if (node == null) continue;
                if (node.GetState() == QuestNodeState.Active) dict = node.tagDictionary;
            }
            return dict;
        }

        private static string ReplaceTag(Match m)
        {
            string tag = m.ToString();

            if (tag.Equals(QUEST)) // Handle QUEST and QUESTID manually; no need to put in dictionary.
            {
                return StringField.GetStringValue(currentQuest.title);
            }
            else if (tag.Equals(QUESTID))
            {
                return StringField.GetStringValue(currentQuest.id);
            }
            else if (tag.StartsWith(CounterValueTagPrefix))
            {
                // Replace {#counter} tags:
                return ReplaceCounterTag(tag, CounterTagType.Current);
            }
            else if (tag.StartsWith(CounterMinValueTagPrefix))
            {
                // Replace counter min value tags:
                return ReplaceCounterTag(tag, CounterTagType.Min);
            }
            else if (tag.StartsWith(CounterMaxValueTagPrefix))
            {
                // Replace counter max value tags:
                return ReplaceCounterTag(tag, CounterTagType.Max);
            }
            else if (tag.StartsWith(CounterTimeValueTagPrefix))
            {
                // Replace {:counter} tags:
                return ReplaceCounterTag(tag, CounterTagType.AsTime);
            }
            else
            {
                // Otherwise try from active node tag dictionary:
                if (nodeTagDictionary != null && nodeTagDictionary.ContainsTag(tag))
                {
                    return nodeTagDictionary.dict[tag];
                }

                // Otherwise try from quest tag dictionary:
                if (questTagDictionary != null && questTagDictionary.ContainsTag(tag))
                {
                    return questTagDictionary.dict[tag];
                }

                // Otherwise try to use quest's current speaker text table:
                var fieldName = tag.Substring(1, tag.Length - 2).Trim();
                if (currentTextTable != null && currentTextTable.HasField(fieldName))
                {
                    return currentTextTable.GetFieldText(fieldName);
                }

                // Otherwise try global text table:
                if (QuestMachine.textTable != null && QuestMachine.textTable.HasField(fieldName))
                {
                    return QuestMachine.textTable.GetFieldText(fieldName);
                }

                // Otherwise return tag itself:
                return fieldName;
            }
        }

        private static string ReplaceCounterTag(string s, CounterTagType tagType)
        {
            if (string.IsNullOrEmpty(s) || currentQuest == null) return s;
            
            var counterName = (tagType == CounterTagType.Current || tagType == CounterTagType.AsTime) ? s.Substring(2, s.Length - 3).Trim()
                : s.Substring(3, s.Length - 4).Trim();

            // Look for counter in current quest:
            var counter = currentQuest.GetCounter(counterName);
            if (counter != null) return GetCounterTagValue(counter, tagType);

            // Otherwise look for quest by ID:
            var index = counterName.IndexOf(CounterTagQuestNameSeparator);
            if (index > 0)
            {
                var questName = counterName.Substring(0, index);
                counterName = counterName.Substring(index + 1);
                var quest = QuestMachine.GetQuestInstance(questName);
                counter = (quest != null) ? quest.GetCounter(counterName) : null;
                if (counter != null) return GetCounterTagValue(counter, tagType);
            }
            return s;
        }

        private static string GetCounterTagValue(QuestCounter counter, CounterTagType tagType)
        {
            if (counter == null) return string.Empty;
            switch (tagType)
            {
                default:
                case CounterTagType.Current:
                    return counter.currentValue.ToString();
                case CounterTagType.Min:
                    return counter.minValue.ToString();
                case CounterTagType.Max:
                    return counter.maxValue.ToString();
                case CounterTagType.AsTime:
                    return SecondsToTimeString(counter.currentValue);
            }
        }

        /// <summary>
        /// Converts seconds into DD HH:MM:SS time format.
        /// </summary>
        public static string SecondsToTimeString(int seconds)
        {
            if (seconds < 60) return seconds.ToString();
            var t = System.TimeSpan.FromSeconds(seconds);
            if (t.Days > 0)
            {
                return string.Format("{0}d, {1:D2}:{2:D2}:{3:D2}", new object[] { t.Days, t.Hours, t.Minutes, t.Seconds });
            }
            else if (t.Hours > 0)
            {
                return string.Format("{0:D2}:{1:D2}:{2:D2}", new object[] { t.Hours, t.Minutes, t.Seconds });
            }
            else
            {
                return string.Format("{0:D2}:{1:D2}", new object[] { t.Minutes, t.Seconds });
            }
        }

        #endregion

        #region Tags to Text Table

        public static void AddQuestTagsToTextTable(Quest quest, TextTable textTable)
        {
            if (quest == null || textTable == null) return;
            AddQuestTagsToTextTable(quest.offerContentList, textTable);
            AddQuestTagsToTextTable(quest.stateInfoList, textTable);
            AddQuestTagsToTextTable(quest.nodeList, textTable);
        }

        public static void AddQuestTagsToTextTable(List<QuestNode> nodeList, TextTable textTable)
        {
            if (nodeList == null) return;
            for (int i = 0; i < nodeList.Count; i++)
            {
                AddQuestTagsToTextTable(nodeList[i], textTable);
            }
        }

        public static void AddQuestTagsToTextTable(QuestNode node, TextTable textTable)
        {
            if (node == null) return;
            AddQuestTagsToTextTable(node.stateInfoList, textTable);
        }

        public static void AddQuestTagsToTextTable(List<QuestStateInfo> stateInfoList, TextTable textTable)
        {
            if (stateInfoList == null) return;
            for (int i = 0; i < stateInfoList.Count; i++)
            {
                AddQuestTagsToTextTable(stateInfoList[i], textTable);
            }
        }

        public static void AddQuestTagsToTextTable(QuestStateInfo stateInfo, TextTable textTable)
        {
            if (stateInfo == null || stateInfo.categorizedContentList == null) return;
            for (int i = 0; i < stateInfo.categorizedContentList.Count; i++)
            {
                AddQuestTagsToTextTable(stateInfo.categorizedContentList[i].contentList, textTable);
            }
        }

        public static void AddQuestTagsToTextTable(QuestContentSet contentSet, TextTable textTable)
        {
            if (contentSet == null) return;
            AddQuestTagsToTextTable(contentSet.contentList, textTable);
        }

        public static void AddQuestTagsToTextTable(List<QuestContent> contentList, TextTable textTable)
        {
            if (contentList == null) return;
            for (int i = 0; i < contentList.Count; i++)
            {
                AddQuestTagsToTextTable(contentList[i], textTable);
            }
        }

        public static void AddQuestTagsToTextTable(QuestContent content, TextTable textTable)
        {
            if (content == null) return;
            AddQuestTagsToTextTable(content.originalText, textTable);
        }

        public static void AddQuestTagsToTextTable(StringField stringField, TextTable textTable)
        {
            if (stringField == null) return;
            AddQuestTagsToTextTable(stringField.text, textTable);
        }

        public static void AddQuestTagsToTextTable(string s, TextTable textTable)
        {
            if (textTable == null || !ContainsAnyTag(s)) return;
            foreach (Match match in Regex.Matches(s, @"\{[^\}]+\}"))
            {
                var tag = match.Value;
                if (IsDynamicTag(tag) || IsIDTag(tag) || tag.Length < 2) continue;
                var fieldName = tag.Substring(1, tag.Length - 2);
                if (!textTable.HasField(fieldName)) textTable.AddField(fieldName);
            }
        }

        #endregion

    }
}
