using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST}
public class BattleSystem : MonoBehaviour
{
    public BattleState state;

    public Battle currentBattle;

    public BattleHUD hud;

    public GameObject enemy;
    public GameObject player;

    public TextMeshProUGUI dialogueText;


    public GameObject buttonsContainer;
    public GameObject statsUI;
    public Button attackButton;

    private SpriteRenderer enemyRenderer;
    private CharStats playerStats;
    private CharStats enemyStats;

    private ScenePortal portal;

    public Animator playerAnimator;
    public Animator enemyAnimator;

    void Start()
    {
        state = BattleState.START;

        statsUI.SetActive(false);
        buttonsContainer.SetActive(false);

        enemyRenderer = enemy.GetComponent<SpriteRenderer>();
        playerStats = player.GetComponent<CharStats>();
        enemyStats = enemy.GetComponent<CharStats>();

        portal = gameObject.GetComponent<ScenePortal>();

        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {

        //Player setup
        playerStats.level = currentBattle.level;
        playerStats.healthBase = currentBattle.playerBaseHealth;
        playerStats.healthTotal = currentBattle.playerCurrentHealth;
        playerStats.magicBase = currentBattle.playerBaseMagic;
        playerStats.magicTotal = currentBattle.playerCurrentMagic;
        playerStats.attackBase = currentBattle.playerAttackBase;
        playerStats.defenseBase = currentBattle.playerDefenseBase;
        playerStats.speedBase = currentBattle.playerSpeedBase;
        playerStats.UpdateStats();

        //Enemy setup
        enemyRenderer.sprite = currentBattle.enemy.sprite;
        enemyStats.characterName = currentBattle.enemy.enemyName;
        enemyStats.healthTotal = currentBattle.enemy.hp;
        enemyStats.magicTotal = currentBattle.enemy.mp;
        enemyStats.attackTotal = currentBattle.enemy.attackDamage;
        enemyStats.defenseTotal = currentBattle.enemy.defense;
        enemyStats.speedTotal = currentBattle.enemy.speed;

        //Start battle
        dialogueText.text = enemyStats.characterName + " approaches.";
        hud.SetHUD(playerStats);

        yield return new WaitForSeconds(2f);
        hud.SetHUD(playerStats);

        statsUI.SetActive(true);
        buttonsContainer.SetActive(true);

        state = BattleState.PLAYERTURN;
        PlayerTurn();

    }

    IEnumerator PlayerAttack()
    {

        playerAnimator.SetTrigger("Attack");

        bool isDead = enemyStats.TakeDamage(playerStats.attackTotal);

        dialogueText.text = "Phendrin attacks " + enemyStats.characterName;

        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = BattleState.WON;
            StartCoroutine(EndBattle());
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        dialogueText.text = enemyStats.characterName + " attacks!";

        enemyAnimator.SetTrigger("Attack");

        yield return new WaitForSeconds(1f);

        bool isDead = playerStats.TakeDamage(enemyStats.attackTotal);

        hud.SetHUD(playerStats);

        yield return new WaitForSeconds(1f);

        if (isDead)
        {
            state = BattleState.LOST;
            StartCoroutine(EndBattle());
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    IEnumerator EndBattle()
    {
        if(state == BattleState.WON)
        {
            dialogueText.text = enemyStats.characterName + " has been defeated.";

            yield return new WaitForSeconds(2f);

            portal.UsePortal();
        }
        else if (state == BattleState.LOST)
        {
            dialogueText.text = "Phendrin has been defeated.";
            yield return new WaitForSeconds(2f);
        }
    }

    void PlayerTurn()
    {
        attackButton.enabled = true;
        dialogueText.text = "Choose an action...";
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }

        attackButton.enabled = false;
        StartCoroutine(PlayerAttack());
    }
}
