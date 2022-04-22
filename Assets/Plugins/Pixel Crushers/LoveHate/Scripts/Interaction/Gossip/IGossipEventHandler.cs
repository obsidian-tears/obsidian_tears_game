// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.EventSystems;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// Unity Event system event handler interface for OnGossip(FactionMember).
    /// </summary>
    public interface IGossipEventHandler : IEventSystemHandler
    {

        void OnGossip(FactionMember other);

    }

}
