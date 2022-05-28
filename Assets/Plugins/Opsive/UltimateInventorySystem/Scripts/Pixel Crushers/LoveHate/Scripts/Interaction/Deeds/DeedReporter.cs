// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// Provides a higher-level way to report deeds to the FactionManager
    /// using a deed template library.
    /// 
    /// Add this to the faction member that performs the actions (usually the player).
    /// </summary>
    [RequireComponent(typeof(FactionMember))]
    [AddComponentMenu("")] // Use wrapper.
    public class DeedReporter : MonoBehaviour
    {

        /// <summary>
        /// The dimension in which to report deeds to FactionManager.
        /// </summary>
        [Tooltip("The dimension in which to report deeds to FactionManager.")]
        public Dimension dimension = Dimension.Is3D;

        /// <summary>
        /// The predefined deed templates.
        /// </summary>
        [Tooltip("The predefined deed templates.")]
        public DeedTemplateLibrary deedTemplateLibrary;

        private FactionMember m_member = null;

        protected virtual void Awake()
        {
            m_member = GetComponent<FactionMember>();
        }

        /// <summary>
        /// Reports that the faction member committed a deed.
        /// </summary>
        /// <param name="tag">Tag of the deed in the deed template library.</param>
        /// <param name="target">Target of the deed.</param>
        public virtual void ReportDeed(string tag, FactionMember target)
        {
            ReportDeedByActor(m_member, tag, target);
        }

        /// <summary>
        /// Reports that the faction member committed a deed.
        /// </summary>
        /// <param name="tag">Tag of the deed in the deed template library.</param>
        /// <param name="target">Target of the deed.</param>
        public virtual void ReportDeedByActor(FactionMember actor, string tag, FactionMember target)
        {
            if (actor == null)
            {
                Debug.LogWarning("Love/Hate: ReportDeed(" + tag + ") actor is null", this);
                return;
            }
            if (target == null)
            {
                Debug.LogWarning("Love/Hate: ReportDeed(" + tag + ") target is null", this);
                return;
            }
            if (deedTemplateLibrary == null)
            {
                Debug.LogWarning("Love/Hate: ReportDeed(" + tag + ") no Deed Template Library is assigned", this);
                return;
            }
            DeedTemplate deedTemplate;
            if (FindDeedTemplate(tag, out deedTemplate))
            {
                var actorPowerLevel = (actor == null) ? 0 : actor.GetPowerLevel();
                var deed = Deed.GetNew(deedTemplate.tag, actor.factionID, target.factionID, deedTemplate.impact,
                                       deedTemplate.aggression, actorPowerLevel, deedTemplate.traits, deedTemplate.permittedEvaluators);
                actor.factionManager.CommitDeed(actor, deed, deedTemplate.requiresSight, dimension, deedTemplate.radius);
                Deed.Release(deed);
            }
        }

        public virtual bool FindDeedTemplate(string tag, out DeedTemplate deedTemplate)
        {
            var index = deedTemplateLibrary.deedTemplates.FindIndex(t => string.Equals(t.tag, tag));
            if (index >= 0)
            {
                deedTemplate = deedTemplateLibrary.deedTemplates[index];
                return true;
            }
            else
            {
                Debug.LogWarning("Love/Hate: DeedReporter can't find deed template for: '" + tag + "'", this);
                deedTemplate = null;
                return false;
            }
        }

        /// <summary>
        /// For optional UtopiaWorx Zone Controller integration.
        /// </summary>
        /// <returns>The properties that Zone Controller can control.</returns>
        public static List<string> ZonePluginActivator()
        {
            List<string> controllable = new List<string>();
            return controllable;
        }

    }

}
