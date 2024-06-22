using System.Collections;
using System.Collections.Generic;
using PixelCrushers.QuestMachine;
using PixelCrushers;
using UnityEngine;
using System;

public class Weylen : MonoBehaviour, IMessageHandler
{
    [Tooltip("The position that Weylen will be teleported to.")]
    [SerializeField] Vector3 newPosition;
    [Tooltip("If true, Weylen will be teleported to a position relative to his current position. If false, Weylen will be teleported to the position specified in newPosition.")]
    [SerializeField] bool useRelativePosition = false;
    [Tooltip("The BoxCollider2D that will be disabled when Weylen teleports.")]
    [SerializeField] BoxCollider2D questCollider;
    [Tooltip("How long the transition will last.")]
    [SerializeField] float transitionTime = 1.0f;
    [Tooltip("The canvas that will be faded in and out during the transition.")]
    [SerializeField] GameObject sceneFaderCanvas;
    private int ignoreMessageCount = 0;
    private void Start()
    {
        MessageSystem.AddListener(this, "Quest State Changed", "Weylen's Requirements");
        if (gameObject.GetComponent<WeylenSaver>().didWeylenMove) {
            DisableQuestCollider();
            SwapWalkingScripts();
            Teleport();
        }
    }

    // Unregister when this GameObject is destroyed.
    private void OnDestroy()
    {
        MessageSystem.RemoveListener(this);
    }

    // Handle messages from Quest Machine.
    public void OnMessage(MessageArgs messageArgs)
    {
        OnMessage(messageArgs.message, messageArgs.parameter, messageArgs.sender);
    }
    
    private void OnMessage(string message, string parameter, object s)
    {
        if (QuestMachine.GetQuestNodeState("Weylen's Requirements", "Dialogue") 
            == QuestNodeState.True)  {
            ignoreMessageCount++;
            Debug.Log("Ding! " + ignoreMessageCount);
            if (ignoreMessageCount != 2) return;
            if (!gameObject.GetComponent<WeylenSaver>().didWeylenMove) {
                gameObject.GetComponent<WeylenSaver>().didWeylenMove = true;
                StartCoroutine(Go());
            } else {
                DisableQuestCollider();
                SwapWalkingScripts();
                Teleport();
            }
        }
    }

    private IEnumerator Go() {
        FreezePlayer(true);
        FadeOut();
        yield return new WaitForSecondsRealtime(transitionTime); // Game is paused, RealTime needed
        DisableQuestCollider();
        SwapWalkingScripts();
        Teleport();
        FadeIn();
        FreezePlayer(false);
    }

    private void SwapWalkingScripts() {
       GetComponents<NonPlayableCharacter>()[0].enabled = false;
       GetComponents<NonPlayableCharacter>()[1].enabled = true;
    }

    private void Teleport() {
        transform.position = useRelativePosition ? transform.position + newPosition : newPosition;
    }

    private void DisableQuestCollider() {
        questCollider.enabled = false;
    }

    private void FadeOut() {
        Debug.Log("out");
        if (sceneFaderCanvas != null) {
            Animator animator = sceneFaderCanvas.GetComponent<Animator>();
            if (animator != null) {
                animator.SetTrigger("Show");
            }
        }
    }

    private void FadeIn() {
        Debug.Log("in");
        if (sceneFaderCanvas != null) {
            Animator animator = sceneFaderCanvas.GetComponent<Animator>();
            if (animator != null) {
                animator.SetTrigger("Hide");
            }
        }
    }

    private void FreezePlayer(bool freeze) {
        GameObject player = GameObject.Find("Player");
        if (player != null) {
            Player playerScript = player.GetComponent<Player>();
            if (playerScript != null) {
                if (freeze)
                    playerScript.Freeze();
                else
                    playerScript.Unfreeze();
            }
        }
    }

}