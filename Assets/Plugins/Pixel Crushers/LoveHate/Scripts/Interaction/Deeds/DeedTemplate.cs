// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// A deed template defines the attributes of a deed that the DeedReporter can
    /// report to the FactionManager.
    /// </summary>
    [Serializable]
    public class DeedTemplate
    {

        /// <summary>
        /// Optional category this deed template belongs to.
        /// </summary>
        public DeedCategory category;

        /// <summary>
        /// The tag unique to the type of deed (e.g., "attack").
        /// </summary>
        public string tag = string.Empty;

        /// <summary>
        /// An optional description for the designer's use.
        /// </summary>
        public string description = string.Empty;

        /// <summary>
        /// The impact of the deed to targets (e.g., attack might be -3, heal +1).
        /// </summary>
        [Range(-100, 100)]
        public float impact = 0;

        /// <summary>
        /// How aggressive or submissive the deed is, where -100 is the most 
        /// submissive and +100 is the most aggressive.
        /// </summary>
        [Range(-100, 100)]
        public float aggression;

        /// <summary>
        /// The objective traits of the deed.
        /// </summary>
        public float[] traits = new float[0];

        /// <summary>
        /// Set `true` if witnesses must be able to see the deed being committed.
        /// </summary>
        public bool requiresSight = false;

        /// <summary>
        /// The maximum radius at which the deed can be witnessed.
        /// </summary>
        public float radius = 10;

        public PermittedEvaluators permittedEvaluators = PermittedEvaluators.Everyone;

        /// <summary>
        /// Initializes a new instance of the <see cref="PixelCrushers.LoveHate.DeedTemplate"/> struct.
        /// </summary>
        /// <param name="tag">Tag unique to the type of deed (e.g., "attack").</param>
        /// <param name="impact">Impact of the deed to targets (e.g., attack might be -3, heal +1).</param>
        /// <param name="aggression">Aggressiveness of the deed [-100,+100].</param>
        /// <param name="traits">Objective traits of the deed.</param>
        /// <param name="mustSee">If set to <c>true</c> must be seen to be witnessed.</param>
        /// <param name="radius">Maximum radius at which the deed can be witnessed.</param>
        /// <param name="permittedEvaluators">Who is allowed to evaluate the deed.</param>
        public DeedTemplate(string tag, string description, float impact, float aggression,
                            float[] traits, bool requiresSight, float radius, PermittedEvaluators permittedEvaluators = PermittedEvaluators.Everyone)
        {
            this.tag = tag;
            this.description = description;
            this.impact = impact;
            this.aggression = aggression;
            this.traits = traits;
            this.requiresSight = requiresSight;
            this.radius = radius;
            this.permittedEvaluators = permittedEvaluators;
        }

    }

}
