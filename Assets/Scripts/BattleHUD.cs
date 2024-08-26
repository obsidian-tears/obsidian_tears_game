using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public TextMeshProUGUI nameText;
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
        if (nameText)
            nameText.text = stats.characterName;

        if (levelText)
            levelText.text = "LVL " + stats.level;

        if (mpNumber)
            mpNumber.text = stats.magicTotal + "/" + stats.magicMax;

        hpNumber.text = stats.healthTotal + "/" + stats.healthMax;

        attackNumber.text = stats.attackTotal.ToString();
        defenseNumber.text = stats.defenseTotal.ToString();
        // speedNumber.text = stats.speedTotal.ToString();

        hpSlider.maxValue = stats.healthMax;
        hpSlider.value = stats.healthTotal;

        if (mpSlider)
        {
            mpSlider.maxValue = stats.magicMax;
            mpSlider.value = stats.magicTotal;
        }
    }
}
