// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// This helper component invokes a delegate method when disabled.
    /// Spawners work with SpawnedEntity instead of the Pixel Crushers
    /// common library SpawnedObject because they handle handle spawning 
    /// differently from SpawnedObjectManager.
    /// 
    /// If the object also has a SpawnedObject component, when restored it will
    /// also register itself with its original Spawner if present.
    /// </summary>
    [AddComponentMenu("")]
    public class SpawnedEntity : Saver
    {
        public string spawnerName { get; set; }

        public delegate void SpawnedObjectDelegate(SpawnedEntity spawnedEntity);

        public event SpawnedObjectDelegate disabled = delegate { };

        public override void OnDisable()
        {
            base.OnDisable();
            disabled(this);
        }

        public override string RecordData()
        {
            return spawnerName;
        }

        public override void ApplyData(string s)
        {
            if (string.IsNullOrEmpty(s)) return;
            spawnerName = s;
            var spawner = Spawner.FindSpawner(spawnerName);
            if (spawner != null) spawner.AddRestoredEntity(this);
        }
    }
}