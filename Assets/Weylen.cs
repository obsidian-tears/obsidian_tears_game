using System.Collections;
using System.Collections.Generic;
using PixelCrushers.QuestMachine;
using PixelCrushers;
using UnityEngine;
using System;

public class Weylen : MonoBehaviour, IMessageHandler
{
    [SerializeField] Vector3 newPosition;
    [SerializeField] BoxCollider2D questCollider;
    private void Start()
    {
        MessageSystem.AddListener(this, "Quest State Changed", "");
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
        if (QuestMachine.GetQuestNodeState("Weylen's Requirements", "Dialogue") == QuestNodeState.True)
        {
            FreezePlayer(true);
            FadeOut();
            SwapBoxCollider();
            Teleport();
            FadeIn();
            FreezePlayer(false);
        }
    }

    private void Teleport() {
        transform.position = newPosition;
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
