// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using PixelCrushers.QuestMachine;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{

    /// <summary>
    /// Sequencer command: Spawner(subject, operation)
    /// 
    /// - subject: GameObject with a Spawner component. Default: speaker.
    /// - operation: start | stop | despawn
    /// </summary>
    public class SequencerCommandSpawner : SequencerCommand
    {

        public void Start()
        {
            var subject = GetSubject(0, Sequencer.Speaker);
            var operation = GetParameter(1);
            if (subject == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: Sequencer: (Quest Machine) Spawner(" + GetParameters() + "): Can't find subject " + GetParameter(0));
            }
            else
            {
                var spawner = subject.GetComponentInChildren<Spawner>();
                if (spawner == null)
                {
                    if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: Sequencer: (Quest Machine) Spawner(" + GetParameters() + "): Subject doesn't have a Spawner.", subject);
                }
                else
                {
                    var failed = false;
                    if (string.Equals(operation, "start", System.StringComparison.OrdinalIgnoreCase))
                    {
                        spawner.StartSpawning();
                    }
                    else if (string.Equals(operation, "stop", System.StringComparison.OrdinalIgnoreCase))
                    {
                        spawner.StopSpawning();
                    }
                    else if (string.Equals(operation, "despawn", System.StringComparison.OrdinalIgnoreCase))
                    {
                        spawner.DespawnAll();
                    }
                    else
                    {
                        failed = true;
                    }
                    if (failed)
                    {
                        if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: Sequencer: (Quest Machine) Spawner(" + GetParameters() + "): Unrecognized operation '" + operation + "'.", subject);
                    }
                    else if (DialogueDebug.LogInfo)
                    {
                        Debug.Log("Dialogue System: Sequencer: (Quest Machine) Spawner(" + GetParameters() + ")", subject);
                    }
                }
            }
            Stop();
        }

    }

}
