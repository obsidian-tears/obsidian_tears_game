// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.EventSystems;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// Unity Event system event handler interface for faction members who
    /// enter aura. This event occurs on the faction member that enters the
    /// aura. For the event on the aura itself, see IAuraEventHandler.
    /// </summary>
    public interface IEnterAuraEventHandler : IEventSystemHandler
    {

        void OnEnterAura(AbstractAuraTrigger aura);

    }

}
