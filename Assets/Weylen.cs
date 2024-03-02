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
    private void Start()
    {
        MessageSystem.AddListener(this, "Quest State Changed", "Weylen's Requirements");
        // DontDestroyOnLoad(this.GetComponent<Weylen>());
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
            == QuestNodeState.True) 
            StartCoroutine(Go());
    }

    private IEnumerator Go() {
        FreezePlayer(true);
        FadeOut();
        yield return new WaitForSeconds(transitionTime);
        SwapBoxCollider();
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

    private void SwapBoxCollider() {
        questCollider.enabled = false;
    }

    private void FadeOut() {
        GameObject sceneFaderCanvas = GameObject.Find("SceneFaderCanvas");
        if (sceneFaderCanvas != null) {
            Animator animator = sceneFaderCanvas.GetComponent<Animator>();
            if (animator != null) {
                animator.SetTrigger("Show");
            }
        }
    }

    private void FadeIn() {
        GameObject sceneFaderCanvas = GameObject.Find("SceneFaderCanvas");
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
