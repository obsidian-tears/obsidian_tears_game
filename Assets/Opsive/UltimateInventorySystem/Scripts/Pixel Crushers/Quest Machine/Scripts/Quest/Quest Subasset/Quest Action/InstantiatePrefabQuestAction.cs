// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Instantiates a prefab.
    /// </summary>
    public class InstantiatePrefabQuestAction : QuestAction
    {

        [Tooltip("Prefab to instantiate.")]
        [SerializeField]
        private GameObject m_prefab;

        [Tooltip("Name of GameObject (usually an empty GameObject) where prefab should be instantiated.")]
        [SerializeField]
        private StringField m_locationTransform;

        /// <summary>
        /// Prefab to instantiate.
        /// </summary>
        public GameObject prefab
        {
            get { return m_prefab; }
            set { m_prefab = value; }
        }

        /// <summary>
        /// Name of GameObject (usually an empty GameObject) where prefab should be instantiated.
        /// </summary>
        public StringField locationTransform
        {
            get { return m_locationTransform; }
            set { m_locationTransform = value; }
        }

        public override string GetEditorName()
        {
            if (prefab == null) return "Instantiate";
            if (locationTransform == null) return "Instantiate: " + prefab.name;
            return "Instantiate: " + prefab.name + " at " + locationTransform;
        }

        public override void Execute()
        {
            if (prefab == null) return;
            var location = StringField.IsNullOrEmpty(locationTransform) ? null : GameObjectUtility.GameObjectHardFind(StringField.GetStringValue(locationTransform));
            if (location == null)
            {
                if (QuestMachine.debug) Debug.Log("Quest Machine: Instantiating prefab '" + prefab + "'.", prefab);
                Instantiate(prefab);
            }
            else
            {
                if (QuestMachine.debug) Debug.Log("Quest Machine: Instantiating prefab '" + prefab + "' at " + locationTransform + ".", prefab);
                Instantiate(prefab, location.transform.position, location.transform.rotation);
            }
        }

    }

}
