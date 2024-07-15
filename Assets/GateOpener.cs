using System.Collections;
using System.Collections.Generic;
using PixelCrushers.QuestMachine;
using PixelCrushers;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class GateOpener : Saver, IMessageHandler
{
    [SerializeField] private GameObject _openGate, _closedGate;

    private bool isOpen = false;

    private void Start()
    {
        MessageSystem.AddListener(this, "Quest State Changed", "");
    }

    // Unregister when this GameObject is destroyed.
    private void OnDestroy()
    {
        MessageSystem.RemoveListener(this);
    }

    public override string RecordData()
    {
        return JsonConvert.SerializeObject(isOpen);
    }

    public override void ApplyData(string s)
    {
        isOpen = JsonConvert.DeserializeObject<bool>(s);

        if (isOpen)
        {
            Open();
        }
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

    private void Open() 
    {
        _closedGate.SetActive(false);
        _openGate.SetActive(true);
    }

}
