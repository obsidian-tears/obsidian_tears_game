// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// This special entity type is essentially a singleton reserved for the player entity.
    /// </summary>
    //--- Only used internally: [CreateAssetMenu(menuName = "Quest Machine/Entities/Specifiers/Player Entity")]
    public class PlayerEntityType : EntityType
    {

        public static PlayerEntityType instance;

#if UNITY_2019_3_OR_NEWER && UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitStaticVariables()
        {
            instance = null;
        }
#endif

        private void OnEnable()
        {
            instance = this;
        }

    }

}