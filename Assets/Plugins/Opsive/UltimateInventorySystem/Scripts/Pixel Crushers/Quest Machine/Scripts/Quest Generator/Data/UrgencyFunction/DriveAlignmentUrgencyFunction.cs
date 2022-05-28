// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Urgency based on how well the observed entity's drives align with the observer's,
    /// where 0 is no alignment and 1 is perfect alignment.
    /// </summary>
    public class DriveAlignmentUrgencyFunction : UrgencyFunction
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

        public override string typeName { get { return "By Drive Alignment"; } }

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
                return GetDriveAlignment(worldModel.observer.entityType.driveValues, worldModel.observed.entityType.driveValues);
            }
            return 0;
        }

        private float GetDriveAlignment(List<DriveValue> observerDriveValues, List<DriveValue> observedDriveValues)
        {
            if (observerDriveValues == null || observedDriveValues == null) return 0;
            float totalAlignment = 0;
            int count = 0;
            for (int i = 0; i < observedDriveValues.Count; i++)
            {
                var observedDriveValue = observedDriveValues[i];
                if (observedDriveValue == null || observedDriveValue.drive == null) continue;
                var observerDriveValue = LookupDriveValue(observerDriveValues, observedDriveValue.drive);
                if (observerDriveValue == null) continue;
                float difference = Mathf.Abs(observedDriveValue.value - observerDriveValue.value);
                var alignment = (200f - difference) / 200f;
                totalAlignment += alignment;
                count++;
            }
            return (count == 0) ? 0 : (totalAlignment / (float)count);
        }

        private DriveValue LookupDriveValue(List<DriveValue> driveValues, Drive drive)
        {
            if (drive == null || driveValues == null) return null;
            for (int i = 0; i < driveValues.Count; i++)
            {
                var driveValue = driveValues[i];
                if (driveValue == null) continue;
                if (driveValue.drive == drive) return driveValue;
            }
            return null;
        }

    }
}
