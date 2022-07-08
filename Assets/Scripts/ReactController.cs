
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactController : MonoBehaviour
{
    [SerializeField] MySignal freezeSignal;
    [SerializeField] MySignal unfreezeSignal;
    [SerializeField] GameObject loadingIndicator;
    [SerializeField] GameObject treasureChest;

    public void GetTreasure()
    {
        // TODO send request to react
        unfreezeSignal.Raise();
        loadingIndicator.SetActive(true);
    }

    // React will call this function after awaiting server action
    public void SetTreasure()
    {
        unfreezeSignal.Raise();
    }

    public void DisplayError(string error) {
        // TODO display an error on the screen
        loadingIndicator.SetActive(false);
        unfreezeSignal.Raise();
    }
}
