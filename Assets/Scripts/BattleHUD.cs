using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI hpNumber;
    public TextMeshProUGUI mpNumber;
    public TextMeshProUGUI attackNumber;
    public TextMeshProUGUI defenseNumber;
    public TextMeshProUGUI speedNumber;

    public Slider hpSlider;
    public Slider mpSlider;

    public void SetHUD(CharStats stats)
    {
        levelText.text = "LVL " + stats.level;
        hpNumber.text = stats.healthTotal + "/" + stats.healthMax;
        mpNumber.text = stats.magicTotal + "/" + stats.magicMax;
        /*attackNumber.text = stats.attackTotal.ToString();
        defenseNumber.text = stats.defenseTotal.ToString();
        speedNumber.text = stats.speedTotal.ToString();*/

        hpSlider.maxValue = stats.healthMax;
        hpSlider.value = stats.healthTotal;

        mpSlider.maxValue = stats.magicMax;
        mpSlider.value = stats.magicTotal;
    }
}
