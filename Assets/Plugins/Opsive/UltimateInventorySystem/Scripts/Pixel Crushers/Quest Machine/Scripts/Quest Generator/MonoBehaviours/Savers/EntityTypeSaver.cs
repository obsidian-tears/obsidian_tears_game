// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Saves entity types' runtime-changeable values.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class EntityTypeSaver : Saver
    {
        [Serializable]
        public class DriveValueData
        {
            public List<float> values = new List<float>();
        }

        [Serializable]
        public class Data
        {
            public List<DriveValueData> entities = new List<DriveValueData>();
        }

        [Tooltip("Save runtime-changeable values of these entity types.")]
        [SerializeField]
        private EntityType[] m_entityTypes = new EntityType[0];

        public EntityType[] entityTypes
        {
            get { return m_entityTypes; }
            set { m_entityTypes = value; }
        }

        public override string RecordData()
        {
            var data = new Data();
            for (int i = 0; i < entityTypes.Length; i++)
            {
                var entity = entityTypes[i];
                var driveValueData = new DriveValueData();
                if (entity != null)
                {
                    for (int j = 0; j < entity.driveValues.Count; j++)
                    {
                        driveValueData.values.Add(entity.driveValues[j].value);
                    }
                }
                data.entities.Add(driveValueData);
            }
            return SaveSystem.Serialize(data);
        }

        public override void ApplyData(string s)
        {
            if (string.IsNullOrEmpty(s)) return;
            var data = SaveSystem.Deserialize<Data>(s);
            if (data == null) return;
            for (int i = 0; i < entityTypes.Length; i++)
            {
                var entity = entityTypes[i];
                if (entity == null) continue;
                for (int j = 0; j < entity.driveValues.Count; j++)
                {
                    entity.driveValues[j].value = data.entities[i].values[j];
                }
            }
        }

    }
}
