using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.DataStructures;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using Opsive.UltimateInventorySystem.Exchange;
using Opsive.UltimateInventorySystem.UI.Panels;
using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using EventHandler = Opsive.Shared.Events.EventHandler;


public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST, FLEEING}
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

    private Inventory inventory;

    public Animator playerAnimator;
    public Animator enemyAnimator;

    public FlashImage flashImage;
    public Color enemyHurtFlashColor;
    public Color playerHurtFlashColor;

    public DisplayPanel mainMenuPanel;

    public Image backgroundImage;

    public Transform damageObjectPrefab;

    public Transform enemyNumberSpawnTransform;
    public Transform playerNumberSpawnTransform;

    public AudioSource musicSource;
    public AudioClip deathSound;

    public UnityEvent onWin;
    public UnityEvent onLose;


    void Start()
    {

        
        backgroundImage.sprite = currentBattle.backgroundImage;

        if(currentBattle.music != null)
        {
            musicSource.clip = currentBattle.music;
            musicSource.Play();
            
        }


        if (onWin == null)
            onWin = new UnityEvent();
        if (onLose == null)
            onLose = new UnityEvent();

        state = BattleState.START;

        statsUI.SetActive(false);
        buttonsContainer.SetActive(false);

        enemyRenderer = enemy.GetComponent<SpriteRenderer>();
        playerStats = player.GetComponent<CharStats>();
        enemyStats = enemy.GetComponent<CharStats>();

        portal = gameObject.GetComponent<ScenePortal>();

        inventory = player.GetComponent<Inventory>();

        if (playerStats.equipper != null)
        {
            EventHandler.RegisterEvent(playerStats.equipper, EventNames.c_Equipper_OnChange, ItemEquipped);
        }
        EventManager.StartListening("PlayerHeal", ItemUsed);

        Cursor.lockState = CursorLockMode.None;

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
        enemyStats.criticalHitProbability = currentBattle.enemy.criticalHitProbability;

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

        DamageValue damageValue = CalculateDamage(playerStats.attackTotal, enemyStats.defenseTotal, playerStats.criticalHitProbability);
        bool isDead = enemyStats.TakeDamage(damageValue.damageAmount);
        

        yield return new WaitForSeconds(1f);

        enemyAnimator.SetTrigger("Hurt");
        flashImage.StartFlash(0.25f, 0.5f, enemyHurtFlashColor);
        spawnDamageDisplay(enemyNumberSpawnTransform.position, damageValue.damageAmount, damageValue.isCritical);

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
        hud.SetHUD(playerStats);
        yield return new WaitForSeconds(0.25f);
        hud.SetHUD(playerStats);

        dialogueText.text = enemyStats.characterName + " attacks!";

        enemyAnimator.SetTrigger("Attack");

        yield return new WaitForSeconds(1f);

        DamageValue damageValue = CalculateDamage(enemyStats.attackTotal, playerStats.defenseTotal, enemyStats.criticalHitProbability);
        bool isDead = playerStats.TakeDamage(damageValue.damageAmount);
        

        playerAnimator.SetTrigger("Hurt");
        flashImage.StartFlash(0.25f, 0.5f, playerHurtFlashColor);
        spawnDamageDisplay(playerNumberSpawnTransform.position, damageValue.damageAmount, damageValue.isCritical);

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
        EventHandler.UnregisterEvent(playerStats.equipper, EventNames.c_Equipper_OnChange, ItemEquipped);
        EventManager.StopListening("PlayerHeal", ItemUsed);

        if (state == BattleState.WON)
        {
            currentBattle.wonBattle = true;
            dialogueText.text = enemyStats.characterName + " has been defeated.";

            float disappearTimer = 1f;
            SpriteRenderer renderer = enemy.GetComponent<SpriteRenderer>();
            Color rendererColor = renderer.color;
            while(disappearTimer > 0)
            {
                rendererColor.a -= Time.deltaTime * 3f;
                renderer.color = rendererColor;
                disappearTimer -= Time.deltaTime;
            }

            onWin.Invoke();

            playerStats.xp += currentBattle.enemy.xpDrop;
            var currencyOwner = inventory.GetCurrencyComponent<CurrencyCollection>() as CurrencyOwner;
            var ownerCurrencyCollection = currencyOwner.CurrencyAmount;
            var gold = InventorySystemManager.GetCurrency("Gold");
            ownerCurrencyCollection.AddCurrency(gold, currentBattle.enemy.goldDrop);

            yield return new WaitForSeconds(2f);
        }
        else if (state == BattleState.LOST)
        {
            musicSource.clip = deathSound;
            dialogueText.text = "Phendrin has been defeated.";
            portal.destinationSceneName = "Main Menu";
            musicSource.PlayOneShot(deathSound);

            onLose.Invoke();
            yield return new WaitForSeconds(9f);
            
            portal.UsePortal();
        }
        else if (state == BattleState.FLEEING)
        {
            dialogueText.text = "Success! Phendrin escapes " + currentBattle.enemy.enemyName;

            yield return new WaitForSeconds(2f);

            portal.setDestinationToPreviousScene();
            portal.UsePortal();
        }
    }

    void PlayerTurn()
    {
        buttonsContainer.SetActive(true);
        dialogueText.text = "Choose an action...";
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }

        buttonsContainer.SetActive(false);
        StartCoroutine(PlayerAttack());
    }

    public void OnRunButton()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }

        StartCoroutine(OnRun());
    }

    IEnumerator OnRun()
    {
        buttonsContainer.SetActive(false);

        dialogueText.text = "Attempting to flee from " + currentBattle.enemy.enemyName + "...";

        yield return new WaitForSeconds(1.5f);

        float chance = Random.Range(0f, 1f);
        if (chance > currentBattle.enemy.runSuccessProbability)
        {

            state = BattleState.FLEEING;
            StartCoroutine(EndBattle());
        }
        else
        {
            dialogueText.text = "Phendrin could not escape " + currentBattle.enemy.enemyName;
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    public void ItemEquipped()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }
        CloseInventory();
        buttonsContainer.SetActive(false);
        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
        hud.SetHUD(playerStats);
    }

    public void ItemUsed()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }
        
        CloseInventory();
        buttonsContainer.SetActive(false);
        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
        hud.SetHUD(playerStats);
    }

    public void CloseInventory()
    {
        mainMenuPanel.SmartClose();
    }

    public DamageValue CalculateDamage(int attack, int defense, float criticalHitChance)
    {
        //Get value of damage
        float denominator = attack + defense;
        float factor = attack / denominator;
        float initalValue = attack * factor;

        //Randomize
        float stdDev = 1.5f;
        if (playerStats.level > 3)
        {
            stdDev = playerStats.level / 2f;
        }

        float randomizedValue = Random.Range(initalValue - stdDev, initalValue + stdDev);

        int damageAmount = (int)System.Math.Ceiling(randomizedValue);

        bool isCritical = false;
        //Double if critical hit
        if(Random.Range(0f, 1.0f) < criticalHitChance)
        {
            damageAmount = damageAmount * 2;
            isCritical = true;
        }

        //Return value
        DamageValue returnValue;
        returnValue.damageAmount = damageAmount;
        returnValue.isCritical = isCritical;
        return returnValue;
    }

    void spawnDamageDisplay(Vector3 location, int damageAmount, bool isCriticalHit)
    {
        Transform damagePopupTransform = Instantiate(damageObjectPrefab, location, Quaternion.identity);
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damageAmount, isCriticalHit);
    }
}

public struct DamageValue
{
    public int damageAmount;
    public bool isCritical;
}