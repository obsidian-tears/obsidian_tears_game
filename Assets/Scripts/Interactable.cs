using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool active;
    public bool playerInRange;
    [SerializeField] MySignal enterSignal;
    [SerializeField] MySignal exitSignal;

    void Start()
    {
        active = true;
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && active)
        {
            enterSignal.Raise();
            playerInRange = true;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && (active || playerInRange))
            exitSignal.Raise();
        playerInRange = false;
    }
}
