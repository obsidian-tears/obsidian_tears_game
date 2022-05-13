// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// Defines an animation trigger that applies to a [min,max) range and 
    /// a set of temperaments.
    /// </summary>
    [Serializable]
    public class RangeAnimation
    {

        public const Temperament AllTemperaments =
            Temperament.Exuberant |
                Temperament.Bored |
                Temperament.Dependent |
                Temperament.Disdainful |
                Temperament.Relaxed |
                Temperament.Anxious |
                Temperament.Docile |
                Temperament.Hostile |
                Temperament.Neutral;

        public static string[] AllTemperamentNames =
        { "Exuberant", "Bored", "Dependent", "Disdainful", "Relaxed", "Anxious", "Docile", "Hostile", "Neutral" };

        public string triggerParameter;

        [Range(-100, 100)]
        public float min;

        [Range(-100, 100)]
        public float max;

        public Temperament temperament;

        public RangeAnimation(string triggerParameter, float min, float max, Temperament temperament)
        {
            this.triggerParameter = triggerParameter;
            this.min = min;
            this.max = max;
            this.temperament = temperament;
        }

    }

}
