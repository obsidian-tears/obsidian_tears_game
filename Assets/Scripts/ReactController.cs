
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class ReactController : MonoBehaviour
{
    [SerializeField] MySignal freezeSignal;
    [SerializeField] MySignal unfreezeSignal;
    [SerializeField] GameObject loadingIndicator;
    [SerializeField] GameObject treasureChest;
    [DllImport("__Internal")] private static extern void OpenChest(String chestId);

    public void GetTreasure()
    {
        // TODO send request to react
        // TODO use proper id for treasure chest
        OpenChest(treasureChest.id);
        freezeSignal.Raise();
        loadingIndicator.SetActive(true);
    }

    // React will call this function after awaiting server action if success
    public void SetTreasure(string treasureIdentifier, int numGold)
    {
        // TODO set the treasure that is inside the treasure chest
        unfreezeSignal.Raise();
    }

    // React will call this function after awaiting server action if failure
    public void DisplayError(string error) {
        // TODO display an error on the screen
        loadingIndicator.SetActive(false);
        unfreezeSignal.Raise();
    }
}
