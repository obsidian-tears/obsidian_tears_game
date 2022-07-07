// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Abstract base class for quest subassets (ScriptableObjects) such as QuestCondition, 
    /// QuestAction, and QuestContent. Adds references to a Quest and QuestNode, and 
    /// handles serialization and instance management.
    /// </summary>
    public abstract class QuestSubasset : ScriptableObject, IProxySerializationCallbackReceiver
    {

        /// <summary>
        /// (Runtime) The quest that this condition belongs to.
        /// </summary>
        public Quest quest { get; protected set; }

        /// <summary>
        /// (Runtime) The quest node that this condition belongs to.
        /// </summary>
        public QuestNode questNode { get; protected set; }

        /// <summary>
        /// (Runtime) The quest's tag dictionary.
        /// </summary>
        protected TagDictionary tagDictionary { get { return (quest != null) ? quest.tagDictionary : null; } }

        /// <summary>
        /// Returns the name to show in the editor for this subasset.
        /// </summary>
        public virtual string GetEditorName()
        {
            return GetType().Name;
        }

        /// <summary>
        /// Sets quest references, as some subassets might need to refer to their containing
        /// quest and/or quest node.
        /// </summary>
        public virtual void SetRuntimeReferences(Quest quest, QuestNode questNode)
        {
            this.quest = quest;
            this.questNode = questNode;
            AddTagsToDictionary();
        }

        /// <summary>
        /// Records the static tags used in this asset's text content into the quest's 
        /// staticTags property.
        /// </summary>
        public virtual void AddTagsToDictionary()
        {
        }

        /// <summary>
        /// Adds any tags in the StringField to the tags dictionary.
        /// </summary>
        /// <param name="stringField"></param>
        protected virtual void AddTagsToDictionary(StringField stringField)
        {
            AddTagsToDictionary(StringField.GetStringValue(stringField));
        }

        /// <summary>
        /// Adds any tags in the string to the tags dictionary.
        /// </summary>
        /// <param name="s"></param>
        protected virtual void AddTagsToDictionary(string s)
        {
            QuestMachineTags.AddTagsToDictionary(tagDictionary, s);
        }

        /// <summary>
        /// Override to return any images that this content references.
        /// </summary>
        /// <returns>Array of images referenced by this content.</returns>
        public virtual Sprite[] GetImages()
        {
            return null;
        }

        /// <summary>
        /// Overide to return any audio clips that this content references.
        /// </summary>
        /// <returns>Array of audio clips referenced by this content.</returns>
        public virtual AudioClip[] GetAudioClips()
        {
            return null;
        }

        /// <summary>
        /// Allows a subasset to save information in a serializable format prior to being
        /// serialized to a proxy object for saving. The base method doesn't do anything,
        /// but subclasses may need to.
        /// </summary>
        public virtual void OnBeforeProxySerialization()
        {
        }

        /// <summary>
        /// Applies saved information from a proxy object. The base method doesn't do anything,
        /// but subclasses may need to.
        /// </summary>
        public virtual void OnAfterProxyDeserialization()
        {
        }

        /// <summary>
        /// Allows subclasses to deep copy their own subassets by instantiating copies.
        /// </summary>
        /// <param name="copy">The copy to instantiate subasset copies into. Assumes the
        /// copy has already been instantiated and contains an accurate copy of everything
        /// except subassets.</param>
        public virtual void CloneSubassetsInto(QuestSubasset copy)
        {
        }

        /// <summary>
        /// Allows subclasses to destroy their subassets, usually when a runtime instance
        /// of a quest is being destroyed.
        /// </summary>
        public virtual void DestroySubassets()
        {
        }

        /// <summary>
        /// Returns a deep copy of a QuestSubasset list.
        /// </summary>
        public static List<T> CloneList<T>(List<T> original) where T : QuestSubasset
        {
            if (original == null) return null;
            var copy = ScriptableObjectUtility.CloneList<T>(original);
            for (int i = 0; i < original.Count; i++)
            {
                if (original[i] != null)
                {
                    original[i].CloneSubassetsInto(copy[i]);
                }
                else
                { 
                    if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: QuestSubasset.CloneList<" + typeof(T).Name + ">: Element " + i + " is null.");
                }
            }
            return copy;
        }

        public static void DestroyList<T>(List<T> list) where T : QuestSubasset
        {
            if (list == null) return;
            for (int i = 0; i < list.Count; i++)
            {
                var subasset = list[i];
                if (subasset == null) continue;
                subasset.DestroySubassets();
                Destroy(subasset);
            }
        }

    }

}
