using System.Collections;
using System.Collections.Generic;
using PixelCrushers.QuestMachine;
using PixelCrushers;
using UnityEngine;
using System;

public class GateOpener : MonoBehaviour, IMessageHandler
{
    [SerializeField] Sprite openGateSprite;
    [SerializeField] BoxCollider2D gateCollider;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
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
        if (QuestMachine.GetQuestNodeState("TanasArtifacts", "VillageKey2") == QuestNodeState.Active)
        {
            Open();
        }
    }

    public void Open() {
        OnDestroy(this);
    }

}
