using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthController : MonoBehaviour
{
    [SerializeField] FloatValue currentHealth;
    [SerializeField] FloatValue currentMagic;
    [SerializeField] Stats playerStats;

    [SerializeField] Slider healthSlider;
    [SerializeField] Slider magicSlider;
    [SerializeField] TextMeshProUGUI levelText;

    void Start()
    {
       UpdateHealthAndMagic();
       levelText.text = playerStats.level.value.ToString();
    }

    public void UpdateHealthAndMagic() {
       healthSlider.value = currentHealth.value / playerStats.maxHealth.value;
       magicSlider.value = currentMagic.value / playerStats.maxMagic.value;
    }
}
