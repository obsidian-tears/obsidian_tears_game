using System.Collections;
using System.Collections.Generic;
using Opsive.UltimateInventorySystem.DropsAndPickups;
using Opsive.UltimateInventorySystem.Interactions;
using UnityEngine;

public class CustomCurrencyPickup : CurrencyPickup
{
    [Tooltip("Dumb treasure chest id")]
    [SerializeField] protected string m_TreasureIndex;

    protected override void OnInteractInternal(IInteractor interactor)
    {
        base.OnInteractInternal(interactor);
        NotifyPickupSuccess();
    }
}
