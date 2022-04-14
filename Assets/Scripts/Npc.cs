using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : MonoBehaviour
{

    [SerializeField] private TextAsset textAsset;
    [SerializeField] private DialogAsset currentDialog;
    [SerializeField] private MySignal dialogSignal;
    [SerializeField] private bool playerInRange;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        playerInRange = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange &&
       Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("hit space in range");
            currentDialog.value = textAsset;
            dialogSignal.Raise();
        }
    }
}
