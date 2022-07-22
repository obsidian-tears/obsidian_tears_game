using Opsive.UltimateInventorySystem.UI.CompoundElements;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelUpMenu : MonoBehaviour
{

    public CharStats playerStats;
    public TextMeshProUGUI currentLevelText;
    public TextMeshProUGUI nextLevelText;
    public TextMeshProUGUI currentXPText;
    public TextMeshProUGUI nextXPText;
    public TextMeshProUGUI xpNeededText;
    public TextMeshProUGUI pointsRemainingText;
    public TextMeshProUGUI currentHPText;
    public TextMeshProUGUI nextHPText;
    public TextMeshProUGUI currentMPText;
    public TextMeshProUGUI nextMPText;
    public TextMeshProUGUI currentAttackText;
    public TextMeshProUGUI nextAttackText;
    public TextMeshProUGUI currentDefenseText;
    public TextMeshProUGUI nextDefenseText;
    public TextMeshProUGUI currentSpeedText;
    public TextMeshProUGUI nextSpeedText;
    public TextMeshProUGUI currentMagicPowerText;
    public TextMeshProUGUI nextMagicPowerText;

    public QuantityPicker hpQP;
    public QuantityPicker mpQP;
    public QuantityPicker attackQP;
    public QuantityPicker defenseQP;
    public QuantityPicker speedQP;
    public QuantityPicker magicPowerQP;

    [HideInInspector] public bool isLevelUp = false;


    // Start is called before the first frame update
    private void OnEnable()
    {
        //Determine if it's a levelup or just using saved points
        if (playerStats.xp < playerStats.xpToLevelUp)
            isLevelUp = false;
        else
            isLevelUp = true;

        //If leveling up, determine how many points you get
        int pointsFromLevelUp = playerStats.pointsRemaining;
        if (isLevelUp)
            pointsFromLevelUp += (playerStats.level + 6);

        //Set each number on the screen to the correct value
        currentLevelText.text = playerStats.level.ToString();

        if (isLevelUp)
            nextLevelText.text = (playerStats.level + 1).ToString();
        else
            nextLevelText.text = playerStats.level.ToString();

        currentXPText.text = playerStats.xp.ToString();

        if (isLevelUp)
            nextXPText.text = (playerStats.xp - playerStats.xpToLevelUp).ToString();
        else
            nextXPText.text = playerStats.xp.ToString();

        xpNeededText.text = playerStats.xpToLevelUp.ToString();

        pointsRemainingText.text = pointsFromLevelUp.ToString();

        currentHPText.text = playerStats.healthBase.ToString();
        nextHPText.text = playerStats.healthBase.ToString();

        currentMPText.text = playerStats.magicBase.ToString();
        nextMPText.text = playerStats.magicBase.ToString();

        currentAttackText.text = playerStats.attackBase.ToString();
        nextAttackText.text = playerStats.attackBase.ToString();

        currentDefenseText.text = playerStats.defenseBase.ToString();
        nextDefenseText.text = playerStats.defenseBase.ToString();

        currentSpeedText.text = playerStats.speedBase.ToString();
        nextSpeedText.text = playerStats.speedBase.ToString();

        currentMagicPowerText.text = playerStats.magicPowerBase.ToString();
        nextMagicPowerText.text = playerStats.magicPowerBase.ToString();


        //Set max of each quantity picker
        hpQP.MaxQuantity = Int32.Parse(pointsRemainingText.text);
        mpQP.MaxQuantity = Int32.Parse(pointsRemainingText.text);
        defenseQP.MaxQuantity = Int32.Parse(pointsRemainingText.text);
        attackQP.MaxQuantity = Int32.Parse(pointsRemainingText.text);
        speedQP.MaxQuantity = Int32.Parse(pointsRemainingText.text);
        magicPowerQP.MaxQuantity = Int32.Parse(pointsRemainingText.text);

    }

    private void OnDisable()
    {
        Debug.Log("Level up menu disabled");
    }
}
