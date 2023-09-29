// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{

    /// <summary>
    /// This script implements the sequencer command LoadingSceneTo(levelName, [loadingSceneIndex], [spawnpoint]).
    /// 
    /// It's kept around for compatibility with projects that use older versions of the Menu Framework.
    /// The preferred method is now to use the regular LoadLevel() sequencer command.
    /// 
    /// - levelName: The destination scene.
    /// - loadingSceneIndex: No longer used.
    /// - spawnpoint: Tells the player's PositionSaver where to spawn the player in the destination scene.
    /// </summary>
    public class SequencerCommandLoadingSceneTo : SequencerCommand
    {

        public void Start()
        {
            var levelName = GetParameter(0);
            var spawnpoint = GetParameter(2);
            if (string.IsNullOrEmpty(levelName))
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: Sequencer: LoadingSceneTo(" + GetParameters() + ") level name is an empty string. Loading by scene index is no longer supported.");
            }
            {
                if (DialogueDebug.LogInfo) Debug.Log("Dialogue System: Sequencer: LoadingSceneTo(" + levelName + ",(ignored), " + spawnpoint + ")");
                var s = string.IsNullOrEmpty(spawnpoint) ? levelName : (levelName + "@" + spawnpoint);
                PersistentDataManager.LevelWillBeUnloaded();
                SaveSystem.LoadScene(s);
            }
            Stop();
        }
    }
}
