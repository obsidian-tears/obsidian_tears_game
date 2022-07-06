// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Entity types are abstract definitions of entities. Every entity in the game world has an entity type that defines
    /// its attributes such as its faction with other entities and actions that can be performed on it. The quest generator
    /// needs to work with abstract entity types because actual instances of the entities may not exist in the world at
    /// the time a quest is being generated.
    /// </summary>
    public class EntityType : ScriptableObject
    {

        [Tooltip("Description of this entity type.")]
        [TextArea]
        [SerializeField]
        private string m_description;

        [Tooltip("There is only one entity of this type.")]
        [SerializeField]
        private bool m_isUnique;

        [Tooltip("The display name of this entity type.")]
        [SerializeField]
        private StringField m_displayName;

        [Tooltip("The plural display name for multiples of this entity type.")]
        [SerializeField]
        private StringField m_pluralDisplayName;

        [Tooltip("The entity type's image may be shown in UIs.")]
        [SerializeField]
        private Sprite m_image;

        [Tooltip("The entity type's level, used to determine quest difficulty and rewards.")]
        [SerializeField]
        private int m_level = 1;

        [Tooltip("The faction that this entity type belongs to.")]
        [SerializeField]
        private Faction m_faction;

        [Tooltip("The entity type's parent types.")]
        [SerializeField]
        private List<EntityType> m_parents;

        [Tooltip("Functions that quest generators use to determine how urgently they must generate a quest about this entity.")]
        [SerializeField]
        private List<UrgencyFunction> m_urgencyFunctions;

        [Tooltip("Actions that can be performed on this entity type.")]
        [SerializeField]
        private List<Action> m_actions;

        [Tooltip("When planning an action, limit to this many targets. Key is total number known; value is max number to require in action. Example: If NPC knows about 20 orcs, she might limit the action to defeat only 5 of them.")]
        [SerializeField]
        private AnimationCurve m_maxCountInAction = new AnimationCurve(new Keyframe(0, 0), new Keyframe(10, 10), new Keyframe(20, 15));

        [Tooltip("This entity type's drives. Used by quest generators to decide on targets and actions.")]
        [SerializeField]
        private List<DriveValue> m_driveValues = new List<DriveValue>();

        [Tooltip("Multiplier values for reward systems.")]
        [SerializeField]
        private float[] m_rewardMultipliers = new float[8] { 1, 1, 1, 1, 1, 1, 1, 1 }; // Must match RewardMultiplier count.

        [NonSerialized]
        private StringField m_internalAssetName = null;

        [NonSerialized]
        private StringField m_internalPluralDisplayName = null;

        /// <summary>
        /// Description of this entity type.
        /// </summary>
        public string description
        {
            get { return m_description; }
            set { m_description = value; }
        }

        /// <summary>
        /// True if there is only one entity of this type.
        /// </summary>
        public bool isUnique
        {
            get { return m_isUnique; }
            set { m_isUnique = value; }
        }

        /// <summary>
        /// The display name of this entity type.
        /// </summary>
        public StringField displayName
        {
            get
            {
                if (!StringField.IsNullOrEmpty(m_displayName))
                {
                    return m_displayName;
                }
                else
                {
                    if (StringField.IsNullOrEmpty(m_internalAssetName)) m_internalAssetName = new StringField(name);
                    return m_internalAssetName;
                }
            }
            set { m_displayName = value; }
        }

        /// <summary>
        /// The plural display name for multiples of this entity type.
        /// </summary>
        public StringField pluralDisplayName
        {
            get
            {
                if (!StringField.IsNullOrEmpty(m_pluralDisplayName))
                {
                    return m_pluralDisplayName;
                }
                else
                {
                    if (StringField.IsNullOrEmpty(m_internalPluralDisplayName))
                    {
                        var plural = displayName.ToString();
                        plural += plural.EndsWith("s") ? "es" : "s";
                        m_internalPluralDisplayName = new StringField(plural);
                    }
                    return m_internalPluralDisplayName;
                }
            }
            set { m_pluralDisplayName = value; }
        }

        /// <summary>
        /// The entity type's image may be shown in UIs.
        /// </summary>
        public Sprite image
        {
            get { return m_image; }
            set { m_image = value; }
        }

        /// <summary>
        /// The entity type's level, used to determine quest difficulty and rewards.
        /// </summary>
        public int level
        {
            get { return m_level; }
            set { m_level = value; }
        }

        /// <summary>
        /// The faction that this entity type belongs to.
        /// </summary>
        public Faction faction
        {
            get { return m_faction; }
            set { m_faction = value; }
        }

        /// <summary>
        /// The entity type's parent types.
        /// </summary>
        public List<EntityType> parents
        {
            get { return m_parents; }
            set { m_parents = value; }
        }

        /// <summary>
        /// Functions that quest generators use to determine how urgently they must generate a quest about this entity.
        /// </summary>
        public List<UrgencyFunction> urgencyFunctions
        {
            get { return m_urgencyFunctions; }
            set { m_urgencyFunctions = value; }
        }

        /// <summary>
        /// Actions that can be performed on this entity type.
        /// </summary>
        public List<Action> actions
        {
            get { return m_actions; }
            set { m_actions = value; }
        }

        /// <summary>
        /// When planning an action, limit to this many targets. Key is total number known; value is max number to require in action.
        /// </summary>
        public AnimationCurve maxCountInAction
        {
            get { return m_maxCountInAction; }
            set { m_maxCountInAction = value; }
        }

        /// <summary>
        /// This entity type's drives. Used by quest generators to decide on targets and actions.
        /// </summary>
        public List<DriveValue> driveValues
        {
            get
            {
                ValidateRuntimeDriveValues();
                return m_runtimeDriveValues;
            }
            set
            {
                ValidateRuntimeDriveValues();
                m_runtimeDriveValues = value;
            }
        }

        private List<DriveValue> m_runtimeDriveValues;
        private bool m_validatedRuntimeDriveValues = false;

        private void ValidateRuntimeDriveValues()
        {
            if (!m_validatedRuntimeDriveValues)
            {
                m_validatedRuntimeDriveValues = true;
                m_runtimeDriveValues = new List<DriveValue>();
                for (int i = 0; i < m_driveValues.Count; i++)
                {
                    m_runtimeDriveValues.Add(new DriveValue(m_driveValues[i]));
                }
            }
        }

        private void ResetRuntimeValues()
        {
            m_validatedRuntimeDriveValues = false;
        }

        /// <summary>
        /// Returns a text descriptor for a specified number of this entity type.
        /// </summary>
        /// <param name="count">The number of entities.</param>
        public string GetDescriptor(int count)
        {
            if (isUnique || count == 1) return StringField.GetStringValue(displayName);
            return count + " " + pluralDisplayName;
        }

        /// <summary>
        /// Looks up this entity type's faction.
        /// </summary>
        /// <returns>This entity type's faction, or its parent's faction if unassigned.</returns>
        public Faction GetFaction()
        {
            if (faction != null) return faction;
            foreach (var parent in parents)
            {
                var result = parent.GetFaction();
                if (result != null) return result;
            }
            return null;
        }

        /// <summary>
        /// Returns a list of urgency functions, including those inherited from parents.
        /// </summary>
        public List<UrgencyFunction> GetUrgencyFunctions()
        {
            var list = new List<UrgencyFunction>(urgencyFunctions);
            if (parents.Count > 0)
            {
                var parentsChecked = new HashSet<EntityType>();
                for (int i = 0; i < parents.Count; i++)
                {
                    AddParentUrgencyFunctions(parents[i], parentsChecked, list);
                }
            }
            return list;
        }

        // Recurse through parents:
        private void AddParentUrgencyFunctions(EntityType parent, HashSet<EntityType> parentsChecked, List<UrgencyFunction> list)
        {
            if (parentsChecked.Contains(parent)) return;
            parentsChecked.Add(parent);
            for (int i = 0; i < parent.urgencyFunctions.Count; i++)
            {
                var urgencyFunction = parent.urgencyFunctions[i];
                if (!list.Contains(urgencyFunction))
                {
                    list.Add(urgencyFunction);
                }
            }
            for (int i = 0; i < parent.parents.Count; i++)
            {
                AddParentUrgencyFunctions(parent.parents[i], parentsChecked, list);
            }
        }

        /// <summary>
        /// Looks up a drive value, first in the entity; failing that, checks parents.
        /// </summary>
        public DriveValue LookupDriveValue(Drive drive)
        {
            return RecursivelyLookupDriveValue(drive, null);
        }

        private DriveValue RecursivelyLookupDriveValue(Drive drive, HashSet<EntityType> entitiesChecked)
        {
            if (drive == null || driveValues == null) return null;
            if (entitiesChecked != null && entitiesChecked.Contains(this)) return null;
            for (int i = 0; i < driveValues.Count; i++)
            {
                var driveValue = driveValues[i];
                if (driveValue == null) continue;
                if (driveValue.drive == drive) return driveValue;
            }
            if (parents.Count > 0)
            {
                if (entitiesChecked == null) entitiesChecked = new HashSet<EntityType>();
                entitiesChecked.Add(this);
                for (int i = 0; i < parents.Count; i++)
                {
                    var parent = parents[i];
                    if (parent == null) continue;
                    var driveValue = parent.RecursivelyLookupDriveValue(drive, entitiesChecked);
                    if (driveValue != null) return driveValue;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the reward multiplier value for a specific category for this entity type.
        /// </summary>
        /// <param name="category">The reward multiplier category.</param>
        /// <returns>The multiplier value.</returns>
        public float GetRewardMultiplier(RewardMultiplier category)
        {
            var i = (int)category;
            return (0 <= i && i < m_rewardMultipliers.Length) ? m_rewardMultipliers[i] : 1;
        }

        private void OnEnable()
        {
            QuestMachineConfiguration.quitting -= ResetRuntimeValues;
            QuestMachineConfiguration.quitting += ResetRuntimeValues;
        }

        private void OnDisable()
        {
            QuestMachineConfiguration.quitting -= ResetRuntimeValues;
        }

    }
}
