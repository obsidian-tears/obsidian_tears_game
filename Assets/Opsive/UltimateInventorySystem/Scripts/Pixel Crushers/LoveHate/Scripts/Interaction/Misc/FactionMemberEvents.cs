// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.Events;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// This component provides UnityEvent blocks for faction member events.
    /// Add it to a faction member, and then assign methods to the events
    /// that you want to handle.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    [RequireComponent(typeof(FactionMember))]
    public class FactionMemberEvents : MonoBehaviour, IForgetDeedEventHandler, IGossipEventHandler,
        IGreetEventHandler, IModifyPadDeedEventHandler, IRememberDeedEventHandler,
        IShareRumorsEventHandler, IWitnessDeedEventHandler
    {

        [System.Serializable]
        public class RumorEvent : UnityEvent<Rumor> { }

        [System.Serializable]
        public class FactionMemberEvent : UnityEvent<FactionMember> { }

        [System.Serializable]
        public class PadEvent : UnityEvent<float, float, float, float> { }

        public PadEvent onModifyPad = new PadEvent();
        public RumorEvent onWitnessDeed = new RumorEvent();
        public RumorEvent onRememberDeed = new RumorEvent();
        public RumorEvent onForgetDeed = new RumorEvent();
        public FactionMemberEvent onShareRumors = new FactionMemberEvent();
        public FactionMemberEvent onGossip = new FactionMemberEvent();
        public FactionMemberEvent onGreet = new FactionMemberEvent();

        public void OnForgetDeed(Rumor rumor)
        {
            onForgetDeed.Invoke(rumor);
        }

        public void OnGossip(FactionMember other)
        {
            onGossip.Invoke(other);
        }

        public void OnGreet(FactionMember other)
        {
            onGreet.Invoke(other);
        }

        public void OnModifyPad(float happinessChange, float pleasureChange, float arousalChange, float dominanceChange)
        {
            onModifyPad.Invoke(happinessChange, pleasureChange, arousalChange, dominanceChange);
        }

        public void OnRememberDeed(Rumor rumor)
        {
            onRememberDeed.Invoke(rumor);
        }

        public void OnShareRumors(FactionMember other)
        {
            onShareRumors.Invoke(other);
        }

        public void OnWitnessDeed(Rumor rumor)
        {
            onWitnessDeed.Invoke(rumor);
        }

    }

}
