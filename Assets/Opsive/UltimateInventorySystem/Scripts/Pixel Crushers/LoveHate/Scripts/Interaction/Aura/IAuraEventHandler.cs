// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.EventSystems;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// Unity Event system event handler interface for aura Affect(FactionMember).
    /// This event occurs on the aura. See IEnterAuraEventHandler for the event
    /// that occurs on the faction member entering the aura.
    /// </summary>
    public interface IAuraEventHandler : IEventSystemHandler
    {

        void OnAura(FactionMember other);

    }

}
