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
    using UnityEngine.UI;
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
        [Tooltip("The magic power text.")]
        [SerializeField] protected Text m_MagicPowerText;


        [Tooltip("The health bar text.")]
        [SerializeField] protected Text m_HealthBarText;
        [Tooltip("The mana bar text.")]
        [SerializeField] protected Text m_ManaBarText;
        [Tooltip("The xp bar text.")]
        [SerializeField] protected Text m_XpBarText;


        public Slider healthSlider;
        public Slider manaSlider;
        public Slider xpSlider;




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
        public void Draw(int healthMax, int magicMax, int attackTotal, int defenseTotal, int speedTotal, int healthTotal, int magicTotal, int magicPowerTotal, int healthBase, int magicBase, int attackBase, int defenseBase, int speedBase, int magicPowerBase, int xpTotal, int xpMax)
        {

            m_MaxHpValueText.text = "Total(Base) " + healthMax.ToString() + "(" + healthBase.ToString() + ")";
            m_MaxMpValueText.text = "Total(Base) " + magicMax.ToString() + "(" + magicBase.ToString() + ")";
            m_AttackValueText.text = "Total(Base) " + attackTotal.ToString() + "(" + attackBase.ToString() + ")";
            m_DefenseValueText.text = "Total(Base) " + defenseTotal.ToString() + "(" + defenseBase.ToString() + ")";
            m_SpeedValueText.text = "Total(Base) " + speedTotal.ToString() + "(" + speedBase.ToString() + ")";
            m_MagicPowerText.text = "Total(Base) " + magicPowerTotal.ToString() + "(" + magicPowerBase.ToString() + ")";

            m_HealthBarText.text = healthTotal.ToString() + "/" + healthMax.ToString();
            m_ManaBarText.text = magicTotal.ToString() + "/" + magicMax.ToString();
            m_XpBarText.text = xpTotal.ToString() + "/" + xpMax.ToString();

            healthSlider.maxValue = healthMax;
            healthSlider.value = healthTotal;

            manaSlider.maxValue = magicMax;
            manaSlider.value = magicTotal;

            xpSlider.maxValue = xpMax;
            xpSlider.value = xpTotal;
        }
    }
}
