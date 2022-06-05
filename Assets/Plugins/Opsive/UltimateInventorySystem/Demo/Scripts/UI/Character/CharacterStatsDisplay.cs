/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Demo.UI.Menus.Main.Inventory
{
    using CharacterControl;
    using CharacterControl.Player;
    using Opsive.UltimateInventorySystem.Core;
    using Opsive.UltimateInventorySystem.Demo.Events;
    using Opsive.UltimateInventorySystem.Equipping;
    using Opsive.UltimateInventorySystem.UI.CompoundElements;
    using UnityEngine;
    using EventHandler = Opsive.Shared.Events.EventHandler;
    using Text = Opsive.Shared.UI.Text;

    /// <summary>
    /// The character stats UI.
    /// </summary>
    public class CharacterStatsDisplay : MonoBehaviour
    {
        public Equipper equipper;

        [Tooltip("The max Hp text.")]
        [SerializeField] protected Text m_MaxHpValueText;
        [Tooltip("The max Mp text.")]
        [SerializeField] protected Text m_MaxMpValueText;
        [Tooltip("The attack text.")]
        [SerializeField] protected Text m_AttackValueText;
        [Tooltip("The defense text.")]
        [SerializeField] protected Text m_DefenseValueText;
        [Tooltip("The speed text.")]
        [SerializeField] protected Text m_SpeedValueText;

        private void Start()
        {
            Initialize();
        }

        /// <summary>
        /// Initialize the listener.
        /// </summary>
        private void Initialize()
        {
            //Draw();
        }

        /// <summary>
        /// Draw the stats.
        /// </summary>
        public void Draw(int healthMax, int magicMax, int attackTotal, int defenseTotal, int speedTotal)
        {

            m_MaxHpValueText.text = healthMax.ToString();
            m_MaxMpValueText.text = magicMax.ToString();
            m_AttackValueText.text = attackTotal.ToString();
            m_DefenseValueText.text = defenseTotal.ToString();
            m_SpeedValueText.text = speedTotal.ToString();

        }
    }
}
