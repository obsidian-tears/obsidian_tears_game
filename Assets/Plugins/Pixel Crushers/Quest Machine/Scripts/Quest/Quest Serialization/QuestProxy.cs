// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    #region Quest Proxy

    /// <summary>
    /// Runtime-serializable proxy object for Quest. To save or export a procedurally-generated quest, 
    /// Quest Machine copies its data to a QuestProxy which it then serializes using JsonUtility.
    /// Quests that are instantiated from assets, on the other hand, are saved in a more compact form
    /// using QuestStateSerializer.
    /// </summary>
    [Serializable]
    public class QuestProxy
    {

        public static bool includeCanvasRect = false;

        public bool isInstance;
        public string id;
        public string displayName;
        public string iconPath;
        public string group;
        public string[] labels;
        public string giver;
        public bool isTrackable;
        public bool track;
        public bool isAbandonable;
        public bool rememberIfAbandoned;
        public QuestConditionSetProxy autostartConditionSet;
        public QuestConditionSetProxy offerConditionSet;
        public QuestContentProxy[] offerUnmetContentList;
        public QuestContentProxy[] offerContentList;
        public int maxTimes;
        public int timesAccepted;
        public float cooldownSecs;
        public float cooldownSecsRemain;
        public QuestState state;
        public QuestStateInfoProxy[] stateInfoList;
        public QuestCounterProxy[] counterList;
        public QuestNodeProxy[] nodeList;
        public TagDictionary tags;
        public QuestIndicatorStateRecordProxy[] indicators;
        public string goalEntity;

        public QuestProxy() { }

        public QuestProxy(Quest quest)
        {
            CopyFrom(quest);
        }

        public void CopyFrom(Quest quest)
        {
            if (quest == null)
            {
                Debug.LogWarning("Quest Machine: QuestProxy.CopyFrom source quest is null.");
                return;
            }
            isInstance = quest.isInstance;
            id = StringField.GetStringValue(quest.id);
            displayName = StringField.GetStringValue(quest.title);
            iconPath = QuestMachine.GetImagePath(quest.icon);
            group = StringField.GetStringValue(quest.group);
            labels = StringFieldsToStrings(quest.labels);
            giver = StringField.GetStringValue(quest.questGiverID);
            isTrackable = quest.isTrackable;
            track = quest.showInTrackHUD;
            isAbandonable = quest.isAbandonable;
            rememberIfAbandoned = quest.rememberIfAbandoned;
            autostartConditionSet = new QuestConditionSetProxy(quest.autostartConditionSet);
            offerConditionSet = new QuestConditionSetProxy(quest.offerConditionSet);
            offerUnmetContentList = QuestContentProxy.NewArray(quest.offerConditionsUnmetContentList);
            offerContentList = QuestContentProxy.NewArray(quest.offerContentList);
            maxTimes = quest.maxTimes;
            timesAccepted = quest.timesAccepted;
            cooldownSecs = quest.cooldownSeconds;
            cooldownSecsRemain = quest.cooldownSecondsRemaining;
            state = quest.GetState();
            stateInfoList = QuestStateInfoProxy.NewArray(quest.stateInfoList);
            counterList = QuestCounterProxy.NewArray(quest.counterList);
            nodeList = QuestNodeProxy.NewArray(quest.nodeList);
            tags = new TagDictionary(quest.tagDictionary);
            indicators = QuestIndicatorStateRecordProxy.NewArray(quest.indicatorStates);
            goalEntity = quest.goalEntityTypeName;
        }

        public void CopyTo(Quest quest)
        {
            if (quest == null)
            {
                Debug.LogWarning("Quest Machine: QuestProxy.CopyTo destination quest is null.");
                return;
            }
            quest.isInstance = isInstance;
            quest.id = new StringField(id);
            quest.title = new StringField(displayName);
            quest.group = new StringField(group);
            quest.labels = StringsToStringFields(labels);
            quest.icon = QuestMachine.GetImage(iconPath);
            quest.questGiverID = new StringField(giver);
            quest.isTrackable = isTrackable;
            quest.showInTrackHUD = track;
            quest.isAbandonable = isAbandonable;
            quest.rememberIfAbandoned = rememberIfAbandoned;
            quest.autostartConditionSet = autostartConditionSet.CreateConditionSet();
            quest.offerConditionSet = offerConditionSet.CreateConditionSet();
            quest.offerConditionsUnmetContentList = QuestContentProxy.CreateList(offerUnmetContentList);
            quest.offerContentList = QuestContentProxy.CreateList(offerContentList);
            quest.maxTimes = maxTimes;
            quest.timesAccepted = timesAccepted;
            quest.UpdateCooldown();
            quest.cooldownSeconds = cooldownSecs;
            quest.cooldownSecondsRemaining = cooldownSecsRemain;
            quest.stateInfoList = QuestStateInfoProxy.CreateList(stateInfoList);
            quest.counterList = QuestCounterProxy.CreateList(counterList);
            quest.nodeList = QuestNodeProxy.CreateList(nodeList);
            quest.tagDictionary = new TagDictionary(tags);
            quest.indicatorStates = QuestIndicatorStateRecordProxy.ArrayToDictionary(indicators);
            quest.goalEntityTypeName = goalEntity;
            quest.SetRuntimeReferences();
            quest.SetState(state, false);
            QuestNodeProxy.CopyStatesTo(nodeList, quest);
        }

        private string[] StringFieldsToStrings(List<StringField> stringFields)
        {
            if (stringFields == null) return new string[0];
            var strings = new string[stringFields.Count];
            for (int i = 0; i < stringFields.Count; i++)
            {
                strings[i] = StringField.GetStringValue(stringFields[i]);
            }
            return strings;
        }

        private List<StringField> StringsToStringFields(string[] strings)
        {
            var stringFields = new List<StringField>();
            if (strings == null) return stringFields;
            for (int i = 0; i < strings.Length; i++)
            {
                stringFields.Add(new StringField(strings[i]));
            }
            return stringFields;
        }
    }

    #endregion

    #region QuestConditionSet Proxy

    [Serializable]
    public class QuestConditionSetProxy
    {

        public QuestConditionProxy[] conds;
        public ConditionCountMode mode;
        public int min;
        public int numTrue;

        public QuestConditionSetProxy() { }

        public QuestConditionSetProxy(QuestConditionSet conditionSet)
        {
            CopyFrom(conditionSet);
        }

        public void CopyFrom(QuestConditionSet conditionSet)
        {
            if (conditionSet == null)
            {
                Debug.LogWarning("Quest Machine: QuestConditionSetProxy.CopyFrom source is null.");
                return;
            }
            conds = QuestConditionProxy.NewArray(conditionSet.conditionList);
            mode = conditionSet.conditionCountMode;
            min = conditionSet.minConditionCount;
            numTrue = conditionSet.numTrueConditions;
        }

        public void CopyTo(QuestConditionSet conditionSet)
        {
            if (conditionSet == null)
            {
                Debug.LogWarning("Quest Machine: QuestConditionSetProxy.CopyTo destination is null.");
                return;
            }
            conditionSet.conditionList = QuestConditionProxy.CreateList(conds);
            conditionSet.conditionCountMode = mode;
            conditionSet.minConditionCount = min;
            conditionSet.numTrueConditions = numTrue;
        }

        public QuestConditionSet CreateConditionSet()
        {
            var result = new QuestConditionSet();
            CopyTo(result);
            return result;
        }
    }

    [Serializable]
    public class QuestConditionProxy
    {

        public string t;
        public string s;

        // To reduce proxy data size, we specially handle the most common cases (counter & message conditions).
        public const string CounterTypeString = "Counter";
        public const string MessageTypeString = "Message";
        public const string TimerTypeString = "Timer";

        public QuestConditionProxy() { }

        public QuestConditionProxy(QuestCondition questCondition)
        {
            CopyFrom(questCondition);
        }

        public void CopyFrom(QuestCondition questCondition)
        {
            if (questCondition == null)
            {
                Debug.LogWarning("Quest Machine: QuestConditionProxy.CopyFrom source is null.");
                return;
            }
            t = questCondition.GetType().FullName;
            questCondition.OnBeforeProxySerialization();
            if (questCondition is CounterQuestCondition)
            {
                t = CounterTypeString;
                s = GetCounterQuestConditionProxyData(questCondition as CounterQuestCondition);
            }
            else if (questCondition is MessageQuestCondition)
            {
                t = MessageTypeString;
                s = GetMessageQuestConditionProxyData(questCondition as MessageQuestCondition);
            }
            else if (questCondition is TimerQuestCondition)
            {
                t = TimerTypeString;
                s = GetTimerQuestConditionProxyData(questCondition as TimerQuestCondition);
            }
            else
            {
                s = JsonUtility.ToJson(questCondition);
            }
        }

        public static QuestConditionProxy[] NewArray(List<QuestCondition> conditions)
        {
            if (conditions == null)
            {
                Debug.LogWarning("Quest Machine: QuestConditionProxy.NewArray source is null.");
                return new QuestConditionProxy[0];
            }
            var array = new QuestConditionProxy[conditions.Count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new QuestConditionProxy(conditions[i]);
            }
            return array;
        }

        public static List<QuestCondition> CreateList(QuestConditionProxy[] conditionsProxy)
        {
            var list = new List<QuestCondition>();
            if (conditionsProxy == null)
            {
                Debug.LogWarning("Quest Machine: QuestConditionProxy.CreateList source is null.");
                return list;
            }
            for (int i = 0; i < conditionsProxy.Length; i++)
            {
                var conditionProxy = conditionsProxy[i];
                if (conditionProxy == null || string.IsNullOrEmpty(conditionProxy.t) || string.IsNullOrEmpty(conditionProxy.s)) continue;
                QuestCondition condition = null;
                if (string.Equals(conditionProxy.t, CounterTypeString))
                {
                    condition = ScriptableObjectUtility.CreateScriptableObject(RuntimeTypeUtility.GetWrapperType(typeof(CounterQuestCondition))) as CounterQuestCondition;
                    ApplyCounterQuestConditionProxyData(condition as CounterQuestCondition, conditionProxy.s);
                }
                else if (string.Equals(conditionProxy.t, MessageTypeString))
                {
                    condition = ScriptableObjectUtility.CreateScriptableObject(RuntimeTypeUtility.GetWrapperType(typeof(MessageQuestCondition))) as MessageQuestCondition;
                    ApplyMessageQuestConditionProxyData(condition as MessageQuestCondition, conditionProxy.s);
                }
                else if (string.Equals(conditionProxy.t, TimerTypeString))
                {
                    condition = ScriptableObjectUtility.CreateScriptableObject(RuntimeTypeUtility.GetWrapperType(typeof(TimerQuestCondition))) as TimerQuestCondition;
                    ApplyTimerQuestConditionProxyData(condition as TimerQuestCondition, conditionProxy.s);
                }
                else
                {
                    var baseType = RuntimeTypeUtility.GetTypeFromName(conditionProxy.t);
                    var type = RuntimeTypeUtility.GetWrapperType(baseType) ?? baseType;
                    condition = ScriptableObject.CreateInstance(type) as QuestCondition;
                    if (condition != null) JsonUtility.FromJsonOverwrite(conditionProxy.s, condition);
                }
                if (condition != null)
                {
                    condition.OnAfterProxyDeserialization();
                    list.Add(condition);
                }
            }
            return list;
        }

        private static string GetCounterQuestConditionProxyData(CounterQuestCondition counterQuestCondition)
        {
            return (int)counterQuestCondition.counterValueMode + ";" + (int)counterQuestCondition.requiredCounterValue.valueType + ";" +
                counterQuestCondition.requiredCounterValue.literalValue + ";" + counterQuestCondition.requiredCounterValue.counterIndex + ";" +
                counterQuestCondition.counterIndex;
        }

        private static void ApplyCounterQuestConditionProxyData(CounterQuestCondition counterQuestCondition, string s)
        {
            if (counterQuestCondition == null || s == null) return;
            var fields = s.Split(';');
            if (fields.Length < 5) return;
            counterQuestCondition.counterValueMode = (CounterValueConditionMode)SafeConvert.ToInt(fields[0]);
            var valueType = (QuestNumber.ValueType)SafeConvert.ToInt(fields[1]);
            switch (valueType)
            {
                case QuestNumber.ValueType.Literal:
                    counterQuestCondition.requiredCounterValue = new QuestNumber(SafeConvert.ToInt(fields[2]));
                    break;
                case QuestNumber.ValueType.CounterValue:
                case QuestNumber.ValueType.CounterMinValue:
                case QuestNumber.ValueType.CounterMaxValue:
                    counterQuestCondition.requiredCounterValue = new QuestNumber();
                    counterQuestCondition.requiredCounterValue.valueType = valueType;
                    counterQuestCondition.requiredCounterValue.counterIndex = SafeConvert.ToInt(fields[3]);
                    break;
                default:
                    counterQuestCondition.requiredCounterValue = new QuestNumber();
                    break;
            }
            counterQuestCondition.counterIndex = SafeConvert.ToInt(fields[4]);
        }

        private static string GetMessageQuestConditionProxyData(MessageQuestCondition messageQuestCondition)
        {
            var valueType = (messageQuestCondition.value != null) ? messageQuestCondition.value.valueType : MessageValueType.None;
            var s = ((int)messageQuestCondition.senderSpecifier).ToString() + ";" + messageQuestCondition.senderID + ";" + 
                ((int)messageQuestCondition.targetSpecifier).ToString() + ";" + messageQuestCondition.targetID + ";" + 
                messageQuestCondition.message + ";" + messageQuestCondition.parameter + ";" + (int)valueType;
            switch (valueType)
            {
                case MessageValueType.Int:
                    s += ";" + messageQuestCondition.value.intValue;
                    break;
                case MessageValueType.String:
                    s += ";" + messageQuestCondition.value.stringValue;
                    break;
            }
            return s;
        }

        private static void ApplyMessageQuestConditionProxyData(MessageQuestCondition messageQuestCondition, string s)
        {
            if (messageQuestCondition == null || s == null) return;
            var fields = s.Split(';');
            if (fields.Length < 7) return;
            messageQuestCondition.senderSpecifier = (QuestMessageParticipant)SafeConvert.ToInt(fields[0]);
            messageQuestCondition.senderID = new StringField(fields[1]);
            messageQuestCondition.targetSpecifier = (QuestMessageParticipant)SafeConvert.ToInt(fields[2]);
            messageQuestCondition.targetID = new StringField(fields[3]);
            messageQuestCondition.message = new StringField(fields[4]);
            messageQuestCondition.parameter = new StringField(fields[5]);
            var valueType = (MessageValueType)SafeConvert.ToInt(fields[6]);
            switch (valueType)
            {
                case MessageValueType.Int:
                    messageQuestCondition.value = new MessageValue(SafeConvert.ToInt(fields[7]));
                    break;
                case MessageValueType.String:
                    messageQuestCondition.value = new MessageValue(fields[7]);
                    break;
                default:
                    messageQuestCondition.value = new MessageValue();
                    break;
            }
        }

        private static string GetTimerQuestConditionProxyData(TimerQuestCondition timerQuestCondition)
        {
            return timerQuestCondition.counterIndex.ToString();
        }

        private static void ApplyTimerQuestConditionProxyData(TimerQuestCondition timerQuestCondition, string s)
        {
            if (timerQuestCondition == null || s == null) return;
            timerQuestCondition.counterIndex = SafeConvert.ToInt(s);
        }

    }

    #endregion

    #region QuestContent Proxy

    [Serializable]
    public class QuestContentProxy
    {

        public string t;
        public string s;

        // To reduce proxy data size, we specially handle the most common cases (counter & message conditions).
        public const string HeadingTypeString = "Head";
        public const string BodyTypeString = "Body";
        public const string IconTypeString = "Icon";
        public const string ButtonTypeString = "Button";

        public QuestContentProxy() { }

        public QuestContentProxy(QuestContent content)
        {
            CopyFrom(content);
        }

        public void CopyFrom(QuestContent content)
        {
            if (content == null)
            {
                Debug.LogWarning("Quest Machine: QuestContentProxy.CopyFrom source is null.");
                return;
            }
            t = content.GetType().FullName;
            content.OnBeforeProxySerialization();
            if (content is HeadingTextQuestContent)
            {
                t = HeadingTypeString;
                s = GetHeadingTextQuestContentProxyData(content as HeadingTextQuestContent);
            }
            else if (content is BodyTextQuestContent)
            {
                t = BodyTypeString;
                s = GetBodyTextQuestContentProxyData(content as BodyTextQuestContent);
            }
            else if(content is IconQuestContent)
            {
                t = IconTypeString;
                s = GetIconQuestContentProxyData(content as IconQuestContent);
            }
            else if (content is ButtonQuestContent)
            {
                t = ButtonTypeString;
                s = GetButtonQuestContentProxyData(content as ButtonQuestContent);
            }
            else
            {
                s = JsonUtility.ToJson(content);
            }
        }

        public static QuestContentProxy[] NewArray(List<QuestContent> contentList)
        {
            if (contentList == null)
            {
                Debug.LogWarning("Quest Machine: QuestContentProxy.NewArray source is null.");
                return new QuestContentProxy[0];
            }
            var array = new QuestContentProxy[contentList.Count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new QuestContentProxy(contentList[i]);
            }
            return array;
        }

        public static List<QuestContent> CreateList(QuestContentProxy[] contentListProxy)
        {
            var list = new List<QuestContent>();
            if (contentListProxy == null)
            {
                Debug.LogWarning("Quest Machine: QuestContentProxy.CreateList source is null.");
                return list;
            }
            for (int i = 0; i < contentListProxy.Length; i++)
            {
                var contentProxy = contentListProxy[i];
                if (contentProxy == null || string.IsNullOrEmpty(contentProxy.t) || string.IsNullOrEmpty(contentProxy.s)) continue;
                QuestContent content = null;
                if (string.Equals(contentProxy.t, HeadingTypeString))
                {
                    content = ScriptableObjectUtility.CreateScriptableObject(RuntimeTypeUtility.GetWrapperType(typeof(HeadingTextQuestContent))) as HeadingTextQuestContent;
                    ApplyHeadingTextQuestContentProxyData(content as HeadingTextQuestContent, contentProxy.s);
                }
                else if (string.Equals(contentProxy.t, BodyTypeString))
                {
                    content = ScriptableObjectUtility.CreateScriptableObject(RuntimeTypeUtility.GetWrapperType(typeof(BodyTextQuestContent))) as BodyTextQuestContent;
                    ApplyBodyTextQuestContentProxyData(content as BodyTextQuestContent, contentProxy.s);
                }
                else if (string.Equals(contentProxy.t, IconTypeString))
                {
                    content = ScriptableObjectUtility.CreateScriptableObject(RuntimeTypeUtility.GetWrapperType(typeof(IconQuestContent))) as IconQuestContent;
                    ApplyIconQuestContentProxyData(content as IconQuestContent, contentProxy.s);
                }
                else if (string.Equals(contentProxy.t, ButtonTypeString))
                {
                    content = ScriptableObjectUtility.CreateScriptableObject(RuntimeTypeUtility.GetWrapperType(typeof(ButtonQuestContent))) as ButtonQuestContent;
                    ApplyButtonQuestContentProxyData(content as ButtonQuestContent, contentProxy.s);
                }
                else
                {
                    var baseType = RuntimeTypeUtility.GetTypeFromName(contentProxy.t);
                    var type = RuntimeTypeUtility.GetWrapperType(baseType) ?? baseType;
                    content = ScriptableObject.CreateInstance(type) as QuestContent;
                    if (content != null) JsonUtility.FromJsonOverwrite(contentProxy.s, content);
                }
                if (content != null)
                {
                    content.OnAfterProxyDeserialization();
                    list.Add(content);
                }
            }
            return list;
        }

        public static QuestContentSet CreateContentSet(QuestContentProxy[] uiContentListProxy)
        {
            var container = new QuestContentSet();
            container.contentList = CreateList(uiContentListProxy);
            return container;
        }

        private static string GetHeadingTextQuestContentProxyData(HeadingTextQuestContent headingTextQuestContent)
        {
            return headingTextQuestContent.headingLevel + ";" + 
                StringField.GetStringValue(headingTextQuestContent.originalText) + ";" + 
                (headingTextQuestContent.useQuestTitle ? "1" : "0");
        }

        private static void ApplyHeadingTextQuestContentProxyData(HeadingTextQuestContent headingTextQuestContent, string s)
        {
            if (headingTextQuestContent == null || s == null) return;
            var fields = s.Split(';');
            if (fields.Length < 2) return;
            headingTextQuestContent.headingLevel = SafeConvert.ToInt(fields[0]);
            headingTextQuestContent.originalText = new StringField(fields[1]);
            headingTextQuestContent.useQuestTitle = (fields.Length == 3) ? (fields[2] == "1") : false;
        }

        private static string GetBodyTextQuestContentProxyData(BodyTextQuestContent bodyTextQuestContent)
        {
            return StringField.GetStringValue(bodyTextQuestContent.originalText);
        }

        private static void ApplyBodyTextQuestContentProxyData(BodyTextQuestContent bodyTextQuestContent, string s)
        {
            if (bodyTextQuestContent == null || s == null) return;
            bodyTextQuestContent.originalText = new StringField(s);
        }

        private static string GetIconQuestContentProxyData(IconQuestContent iconQuestContent)
        {
            return iconQuestContent.count + ";" + ((iconQuestContent.image != null) ? iconQuestContent.image.name : string.Empty) + ";" + 
                StringField.GetStringValue(iconQuestContent.originalText);

        }

        private static void ApplyIconQuestContentProxyData(IconQuestContent iconQuestContent, string s)
        {
            if (iconQuestContent == null || s == null) return;
            var fields = s.Split(';');
            if (fields.Length < 3) return;
            iconQuestContent.count = SafeConvert.ToInt(fields[0]);
            iconQuestContent.image = string.IsNullOrEmpty(fields[1]) ? null : QuestMachine.GetImage(fields[1]);
            iconQuestContent.originalText = new StringField(fields[2]);
        }

        private static string GetButtonQuestContentProxyData(ButtonQuestContent buttonQuestContent)
        {
            return buttonQuestContent.count + ";" + ((buttonQuestContent.image != null) ? buttonQuestContent.image.name : string.Empty) + ";" + 
                StringField.GetStringValue(buttonQuestContent.originalText) + ";" + JsonUtility.ToJson(buttonQuestContent.m_actionsProxy);
        }

        private static void ApplyButtonQuestContentProxyData(ButtonQuestContent buttonQuestContent, string s)
        {
            if (buttonQuestContent == null || s == null) return;
            var fields = s.Split(';');
            if (fields.Length < 4) return;
            buttonQuestContent.count = SafeConvert.ToInt(fields[0]);
            buttonQuestContent.image = string.IsNullOrEmpty(fields[1]) ? null : QuestMachine.GetImage(fields[1]);
            buttonQuestContent.originalText = new StringField(fields[2]);
            buttonQuestContent.m_actionsProxy = JsonUtility.FromJson<QuestActionProxyContainer>(fields[3]);
        }

    }

    #endregion

    #region QuestCounter Proxy

    [Serializable]
    public class QuestCounterProxy
    {
        public string name;
        public int val;
        public int min;
        public int max;
        public bool rand;
        public QuestCounterUpdateMode mode;
        public QuestCounterMessageEventProxy[] messages;

        public QuestCounterProxy() { }

        public QuestCounterProxy(QuestCounter counter)
        {
            CopyFrom(counter);
        }

        public void CopyFrom(QuestCounter counter)
        {
            if (counter == null)
            {
                Debug.LogWarning("Quest Machine: QuestCounterProxy.CopyFrom source is null.");
                return;
            }
            name = StringField.GetStringValue(counter.name);
            val = counter.currentValue;
            min = counter.minValue;
            max = counter.maxValue;
            rand = counter.randomizeInitialValue;
            mode = counter.updateMode;
            messages = QuestCounterMessageEventProxy.NewArray(counter.messageEventList);
        }

        public void CopyTo(QuestCounter counter)
        {
            if (counter == null)
            {
                Debug.LogWarning("Quest Machine: QuestCounterProxy.CopyTo destination is null.");
                return;
            }
            counter.name = new StringField(name);
            counter.SetValue(val, QuestCounterSetValueMode.DontInformListeners);
            counter.minValue = min;
            counter.maxValue = max;
            counter.randomizeInitialValue = rand;
            counter.updateMode = mode;
            counter.messageEventList = QuestCounterMessageEventProxy.CreateList(messages);
        }

        public static QuestCounterProxy[] NewArray(List<QuestCounter> counters)
        {
            if (counters == null)
            {
                Debug.LogWarning("Quest Machine: QuestCounterProxy.NewArray source is null.");
                return new QuestCounterProxy[0];
            }
            var array = new QuestCounterProxy[counters.Count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new QuestCounterProxy(counters[i]);
            }
            return array;
        }

        public static List<QuestCounter> CreateList(QuestCounterProxy[] countersProxy)
        {
            var list = new List<QuestCounter>();
            if (countersProxy == null)
            {
                Debug.LogWarning("Quest Machine: QuestCounterProxy.CreateList source is null.");
                return list;
            }
            for (int i = 0; i < countersProxy.Length; i++)
            {
                var counter = new QuestCounter();
                countersProxy[i].CopyTo(counter);
                list.Add(counter);
            }
            return list;
        }
    }

    [Serializable]
    public class QuestCounterMessageEventProxy
    {
        public QuestMessageParticipant sndSpec;
        public string snd;
        public QuestMessageParticipant tgtSpec;
        public string tgt;
        public string msg;
        public string parm;
        public QuestCounterMessageEvent.Operation op;
        public int val;

        public QuestCounterMessageEventProxy() { }

        public QuestCounterMessageEventProxy(QuestCounterMessageEvent messageEvent)
        {
            CopyFrom(messageEvent);
        }

        public void CopyFrom(QuestCounterMessageEvent messageEvent)
        {
            if (messageEvent == null)
            {
                Debug.LogWarning("Quest Machine: QuestCounterMessageEventProxy.CopyFrom source is null.");
                return;
            }
            sndSpec = messageEvent.senderSpecifier;
            snd = StringField.GetStringValue(messageEvent.senderID);
            tgtSpec = messageEvent.targetSpecifier;
            tgt = StringField.GetStringValue(messageEvent.targetID);
            msg = StringField.GetStringValue(messageEvent.message);
            parm = StringField.GetStringValue(messageEvent.parameter);
            op = messageEvent.operation;
            val = messageEvent.literalValue;
        }

        public void CopyTo(QuestCounterMessageEvent messageEvent)
        {
            if (messageEvent == null)
            {
                Debug.LogWarning("Quest Machine: QuestCounterMessageEventProxy.CopyTo destination is null.");
                return;
            }
            messageEvent.senderSpecifier = sndSpec;
            messageEvent.senderID = new StringField(snd);
            messageEvent.targetSpecifier = tgtSpec;
            messageEvent.targetID = new StringField(tgt);
            messageEvent.message = new StringField(msg);
            messageEvent.parameter = new StringField(parm);
            messageEvent.operation = op;
            messageEvent.literalValue = val;
        }

        public static QuestCounterMessageEventProxy[] NewArray(List<QuestCounterMessageEvent> messageEvents)
        {
            if (messageEvents == null)
            {
                Debug.LogWarning("Quest Machine: QuestCounterMessageEventProxy.NewArray source is null.");
                return new QuestCounterMessageEventProxy[0];
            }
            var array = new QuestCounterMessageEventProxy[messageEvents.Count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new QuestCounterMessageEventProxy(messageEvents[i]);
            }
            return array;
        }

        public static List<QuestCounterMessageEvent> CreateList(QuestCounterMessageEventProxy[] messageEventsProxy)
        {
            var list = new List<QuestCounterMessageEvent>();
            if (messageEventsProxy == null)
            {
                Debug.LogWarning("Quest Machine: QuestCounterMessageEventProxy.CreateList source is null.");
                return list;
            }
            for (int i = 0; i < messageEventsProxy.Length; i++)
            {
                var messageEvent = new QuestCounterMessageEvent();
                messageEventsProxy[i].CopyTo(messageEvent);
                list.Add(messageEvent);
            }
            return list;
        }
    }

    #endregion

    #region QuestStateInfo Proxy

    [Serializable]
    public class QuestStateInfoProxy
    {

        public QuestActionProxy[] acn;
        public QuestContentProxy[] dlg;
        public QuestContentProxy[] jrl;
        public QuestContentProxy[] hud;

        public QuestStateInfoProxy() { }

        public QuestStateInfoProxy(QuestStateInfo stateInfo)
        {
            CopyFrom(stateInfo);
        }

        public void CopyFrom(QuestStateInfo stateInfo)
        {
            if (stateInfo == null)
            {
                Debug.LogWarning("Quest Machine: QuestStateInfoProxy.CopyFrom source is null.");
                return;
            }
            acn = QuestActionProxy.NewArray(stateInfo.actionList);
            dlg = GetNewArrayForCategory(stateInfo, QuestContentCategory.Dialogue);
            jrl = GetNewArrayForCategory(stateInfo, QuestContentCategory.Journal);
            hud = GetNewArrayForCategory(stateInfo, QuestContentCategory.HUD);
        }

        private QuestContentProxy[] GetNewArrayForCategory(QuestStateInfo stateInfo, QuestContentCategory category)
        {
            var index = (int)category;
            return (0 <= index && index < stateInfo.categorizedContentList.Count)
                ? QuestContentProxy.NewArray(stateInfo.categorizedContentList[index].contentList)
                : new QuestContentProxy[0];
        }

        public void CopyTo(QuestStateInfo stateInfo)
        {
            if (stateInfo == null)
            {
                Debug.LogWarning("Quest Machine: QuestStateInfoProxy.CopyTo destination is null.");
                return;
            }
            stateInfo.actionList = QuestActionProxy.CreateList(acn);
            stateInfo.categorizedContentList = new List<QuestContentSet>();
            for (int i = 0; i < QuestStateInfo.NumContentCategories ; i++)
            {
                var contentSet = QuestContentProxy.CreateContentSet(GetContentListProxy((QuestContentCategory)i));
                stateInfo.categorizedContentList.Add(contentSet);
            }
        }

        private QuestContentProxy[] GetContentListProxy(QuestContentCategory category)
        {
            switch (category)
            {
                case QuestContentCategory.Dialogue:
                    return dlg;
                case QuestContentCategory.Journal:
                    return jrl;
                case QuestContentCategory.HUD:
                    return hud;
                default:
                    if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: Unsupported content category '" + category + "' when deserializing quest. Please contact the developer.");
                    return null;
            }
        }

        public static QuestStateInfoProxy[] NewArray(List<QuestStateInfo> stateInfoList)
        {
            if (stateInfoList == null)
            {
                Debug.LogWarning("Quest Machine: QuestStateInfoProxy.NewArray source is null.");
                return new QuestStateInfoProxy[0];
            }
            var array = new QuestStateInfoProxy[stateInfoList.Count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new QuestStateInfoProxy(stateInfoList[i]);
            }
            return array;
        }

        public static List<QuestStateInfo> CreateList(QuestStateInfoProxy[] stateInfoListProxy)
        {
            var list = new List<QuestStateInfo>();
            if (stateInfoListProxy == null)
            {
                Debug.LogWarning("Quest Machine: QuestStateInfoProxy.CreateList source is null.");
                return list;
            }
            for (int i = 0; i < stateInfoListProxy.Length; i++)
            {
                var counter = new QuestStateInfo();
                stateInfoListProxy[i].CopyTo(counter);
                list.Add(counter);
            }
            return list;
        }
    }

    #endregion

    #region QuestAction Proxy

    [Serializable]
    public class QuestActionProxy
    {

        public string t;
        public string s;

        public QuestActionProxy() { }

        public QuestActionProxy(QuestAction action)
        {
            CopyFrom(action);
        }

        public void CopyFrom(QuestAction action)
        {
            if (action == null)
            {
                Debug.LogWarning("Quest Machine: QuestActionProxy.CopyFrom source is null.");
                return;
            }
            t = action.GetType().FullName;
            action.OnBeforeProxySerialization();
            s = JsonUtility.ToJson(action);
        }

        public static QuestActionProxy[] NewArray(List<QuestAction> actionList)
        {
            if (actionList == null)
            {
                Debug.LogWarning("Quest Machine: QuestActionProxy.NewArray source is null.");
                return new QuestActionProxy[0];
            }
            var array = new QuestActionProxy[actionList.Count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new QuestActionProxy(actionList[i]);
            }
            return array;
        }

        public static List<QuestAction> CreateList(QuestActionProxy[] actionListProxy)
        {
            var list = new List<QuestAction>();
            if (actionListProxy == null)
            {
                Debug.LogWarning("Quest Machine: QuestActionProxy.CreateList source is null.");
                return list;
            }
            for (int i = 0; i < actionListProxy.Length; i++)
            {
                var actionProxy = actionListProxy[i];
                if (actionProxy == null || string.IsNullOrEmpty(actionProxy.t) || string.IsNullOrEmpty(actionProxy.s)) continue;

                var baseType = RuntimeTypeUtility.GetTypeFromName(actionProxy.t);
                var type = RuntimeTypeUtility.GetWrapperType(baseType) ?? baseType;
                var action = ScriptableObject.CreateInstance(type) as QuestAction;
                if (action != null)
                {
                    JsonUtility.FromJsonOverwrite(actionProxy.s, action);
                    action.OnAfterProxyDeserialization();
                    list.Add(action);
                }
            }
            return list;
        }
    }

    [Serializable]
    public class QuestActionProxyContainer
    {
        public QuestActionProxy[] actions;

        public QuestActionProxyContainer() { }

        public QuestActionProxyContainer(List<QuestAction> actionList)
        {
            var count = (actionList == null) ? 0 : actionList.Count;
            var actions = new QuestActionProxy[count];
            for (int i = 0; i < count; i++)
            {
                actions[i] = new QuestActionProxy(actionList[i]);
            }
        }

        public static List<QuestAction> CreateList(QuestActionProxyContainer proxy)
        {
            if (proxy == null || proxy.actions == null) return new List<QuestAction>();
            return QuestActionProxy.CreateList(proxy.actions);
        }
    }

    #endregion

    #region QuestNode Proxy

    [Serializable]
    public class QuestNodeProxy
    {

        public string id;
        public string intName;
        public QuestNodeType type;
        public bool optional;
        public QuestNodeState state;
        public string speaker;
        public QuestStateInfoProxy[] states;
        public QuestConditionSetProxy conds;
        public TagDictionary tags;
        public int[] childIdx;
        public string r;

        public QuestNodeProxy() { }

        public QuestNodeProxy(QuestNode node)
        {
            CopyFrom(node);
        }

        public void CopyFrom(QuestNode node)
        {
            if (node == null)
            {
                Debug.LogWarning("Quest Machine: QuestNodeProxy.CopyFrom source is null.");
                return;
            }
            id = StringField.GetStringValue(node.id);
            intName = StringField.GetStringValue(node.internalName);
            type = node.nodeType;
            optional = node.isOptional;
            state = node.GetState();
            speaker = StringField.GetStringValue(node.speaker);
            states = QuestStateInfoProxy.NewArray(node.stateInfoList);
            conds = new QuestConditionSetProxy(node.conditionSet);
            tags =  new TagDictionary(node.tagDictionary);
            childIdx = node.childIndexList.ToArray();
            r = QuestProxy.includeCanvasRect ? ((int)node.canvasRect.x + ";" + (int)node.canvasRect.y + ";" + (int)node.canvasRect.width + ";" + (int)node.canvasRect.height) : string.Empty;
        }

        public void CopyTo(QuestNode node)
        {
            if (node == null)
            {
                Debug.LogWarning("Quest Machine: QuestNodeProxy.CopyTo destination is null.");
                return;
            }
            node.id = new StringField(id);
            node.internalName = new StringField(intName);
            node.nodeType = type;
            node.isOptional = optional;
            node.speaker = new StringField(speaker);
            node.stateInfoList = QuestStateInfoProxy.CreateList(states);
            node.conditionSet = conds.CreateConditionSet();
            node.tagDictionary = new TagDictionary(tags);
            node.childIndexList = new List<int>(childIdx);
            if (QuestProxy.includeCanvasRect && !string.IsNullOrEmpty(r))
            {
                var fields = r.Split(';');
                if (fields.Length == 4)
                {
                    node.canvasRect = new Rect(SafeConvert.ToFloat(fields[0]), SafeConvert.ToFloat(fields[1]), SafeConvert.ToFloat(fields[2]), SafeConvert.ToFloat(fields[3]));
                }
            }
        }

        public static QuestNodeProxy[] NewArray(List<QuestNode> nodes)
        {
            if (nodes == null)
            {
                Debug.LogWarning("Quest Machine: QuestNodeProxy.NewArray source is null.");
                return new QuestNodeProxy[0];
            }
            var array = new QuestNodeProxy[nodes.Count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new QuestNodeProxy(nodes[i]);
            }
            return array;
        }

        public static List<QuestNode> CreateList(QuestNodeProxy[] nodesProxy)
        {
            var list = new List<QuestNode>();
            if (nodesProxy == null)
            {
                Debug.LogWarning("Quest Machine: QuestNodeProxy.CreateList source is null.");
                return list;
            }
            for (int i = 0; i < nodesProxy.Length; i++)
            {
                var node = new QuestNode();
                nodesProxy[i].CopyTo(node);
                list.Add(node);
            }
            return list;
        }

        public static void CopyStatesTo(QuestNodeProxy[] nodesProxy, Quest quest)
        {
            if (nodesProxy == null || quest == null || quest.nodeList == null) return;
            for (int i = 0; i < Mathf.Min(nodesProxy.Length, quest.nodeList.Count); i++)
            {
                quest.nodeList[i].SetState(nodesProxy[i].state, false);
            }
        }
    }

    #endregion

    #region Quest Indicator State Records Proxy

    [Serializable]
    public class QuestIndicatorStateRecordProxy
    {

        public string id;
        public QuestIndicatorState indicator;

        public QuestIndicatorStateRecordProxy(string id, QuestIndicatorState indicator)
        {
            this.id = id;
            this.indicator = indicator;
        }

        public static QuestIndicatorStateRecordProxy[] NewArray(Dictionary<string, QuestIndicatorState> indicatorRecords)
        {
            if (indicatorRecords == null) return null;
            var array = new QuestIndicatorStateRecordProxy[indicatorRecords.Count];
            int i = 0;
            foreach (var kvp in indicatorRecords)
            {
                array[i] = new QuestIndicatorStateRecordProxy(kvp.Key, kvp.Value);
                i++;
            }
            return array;
        }

        public static Dictionary<string, QuestIndicatorState> ArrayToDictionary(QuestIndicatorStateRecordProxy[] array)
        {
            var dict = new Dictionary<string, QuestIndicatorState>();
            if (array == null) return dict;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == null) continue;
                dict.Add(array[i].id, array[i].indicator);
            }
            return dict;
        }
    }

    #endregion

}
