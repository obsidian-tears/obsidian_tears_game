// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.EventSystems;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// Unity Event system event handler interface for OnGreet(FactionMember).
    /// </summary>
    public interface IGreetEventHandler : IEventSystemHandler
    {

        void OnGreet(FactionMember other);

    }

}
