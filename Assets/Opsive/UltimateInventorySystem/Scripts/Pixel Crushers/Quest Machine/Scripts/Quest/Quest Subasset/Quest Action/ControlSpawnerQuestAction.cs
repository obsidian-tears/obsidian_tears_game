// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Controls a spawner.
    /// </summary>
    public class ControlSpawnerQuestAction : QuestAction
    {

        public enum ControlState { Start, Stop, Despawn }

        [Tooltip("Name of spawner to control.")]
        [SerializeField]
        private StringField m_spawnerName;

        [Tooltip("New state.")]
        [SerializeField]
        private ControlState m_state = ControlState.Start;

        /// <summary>
        /// Name of spawner to control
        /// </summary>
        public StringField spawnerName
        {
            get { return m_spawnerName; }
            set { m_spawnerName = value; }
        }

        /// <summary>
        /// New state.
        /// </summary>
        public ControlState state
        {
            get { return m_state; }
            set { m_state = value; }
        }

        public override string GetEditorName()
        {
            if (StringField.IsNullOrEmpty(spawnerName)) return "Control Spawner";
            return "Control Spawner: " + state + " " + spawnerName;
        }

        public override void Execute()
        {
            switch (state)
            {
                case ControlState.Start:
                    QuestMachineMessages.StartSpawner(spawnerName);
                    break;
                case ControlState.Stop:
                    QuestMachineMessages.StopSpawner(spawnerName);
                    break;
                case ControlState.Despawn:
                    QuestMachineMessages.DespawnSpawner(spawnerName);
                    break;
            }
        }

    }

}
