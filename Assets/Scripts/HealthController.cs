using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// NOTE: not used anymore, health and mana are controlled through CharStats class
public class HealthController : MonoBehaviour
{
    [SerializeField] FloatValue currentHealth;
    [SerializeField] FloatValue currentMagic;
    [SerializeField] Stats playerStats;

    [SerializeField] Slider healthSlider;
    [SerializeField] Slider magicSlider;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI goldText;

    void Start()
    {
       //UpdateHealthAndMagic();
    }

    public void UpdateHealthAndMagic() {
      //  healthSlider.value = currentHealth.value / playerStats.maxHealth.value;
      //  magicSlider.value = currentMagic.value / playerStats.maxMagic.value;
      //  goldText.text = "$" + Mathf.FloorToInt(playerStats.gold.value).ToString();
      //  levelText.text = playerStats.level.value.ToString();
    }
}
