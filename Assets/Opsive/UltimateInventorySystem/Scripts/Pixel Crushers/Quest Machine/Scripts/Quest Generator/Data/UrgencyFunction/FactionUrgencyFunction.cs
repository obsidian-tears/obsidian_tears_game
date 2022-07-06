// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Returns the faction affinity.
    /// </summary>
    //--- Only used internally: [CreateAssetMenu(menuName = "Quest Machine/Entities/Urgency Functions/Urgency By Faction")]
    public class FactionUrgencyFunction : UrgencyFunction
    {

        [Tooltip("Multiply the urgency by the value of this curve, where the keys indicate the number of entities the observer is aware of.")]
        [SerializeField]
        private AnimationCurve m_entityCountMultiplier = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1), new Keyframe(10, 10), new Keyframe(50, 20));

        /// <summary>
        /// The urgency value is multiplied by this curve, where the keys indicate the number of entities the observer is aware of.
        /// </summary>
        public AnimationCurve entityCountMultiplier
        {
            get { return m_entityCountMultiplier; }
            set { m_entityCountMultiplier = value; }
        }

        public override string typeName { get { return "By Faction"; } }

        public override float Compute(WorldModel worldModel)
        {
            if (worldModel == null)
            {
                Debug.LogError("Quest Machine: Internal error - world model is null.");
            }
            else if (worldModel.observer == null)
            {
                Debug.LogError("Quest Machine: Internal error - world model observer is null.");
            }
            else if (worldModel.observer.entityType == null)
            {
                Debug.LogError("Quest Machine: Observer's entity type is null. Do you need to assign an entity type to it?");
            }
            else if (worldModel.observed == null)
            {
                Debug.LogError("Quest Machine: Internal error - world model observed entity is null.");
            }
            else if (worldModel.observed.entityType == null)
            {
                Debug.LogError("Quest Machine: Observed entity's entity type is null. Do you need to assign an entity type to it?");
            }
            else
            {
                var observerFaction = worldModel.observer.entityType.GetFaction();
                var observedFaction = worldModel.observed.entityType.GetFaction();
                if (observerFaction == null)
                {
                    Debug.LogError("Quest Machine: " + worldModel.observer.entityType.name + " faction is null. Do you need to assign a faction?");
                }
                else if (observedFaction == null)
                {
                    Debug.LogError("Quest Machine: " + worldModel.observed.entityType.name + " faction is null. Do you need to assign a faction?");
                }
                else
                {
                    return entityCountMultiplier.Evaluate(worldModel.observed.count) * observerFaction.GetAffinity(observedFaction);
                }
            }
            return 0;
        }

    }
}
