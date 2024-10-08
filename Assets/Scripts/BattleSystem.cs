using GameManagers;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.AttributeSystem;
using Opsive.UltimateInventorySystem.Core.DataStructures;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using Opsive.UltimateInventorySystem.Exchange;
using Opsive.UltimateInventorySystem.ItemActions;
using Opsive.UltimateInventorySystem.UI.Panels;
using Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers;
using PixelCrushers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.LowLevel;
using UnityEngine.UI;
using static Opsive.UltimateInventorySystem.DatabaseNames.DemoInventoryDatabaseNames;
using EventHandler = Opsive.Shared.Events.EventHandler;
using PixelCrushers.DialogueSystem;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST, FLEEING }
public class BattleSystem : MonoBehaviour
{
    public BattleState state;

    public Battle currentBattle;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public GameObject enemy;
    public GameObject player;
    public GameObject Arrow;

    public TextMeshProUGUI dialogueText;


    //public ItemViewSlotsContainerPanelBinding charPanelBinding;
    //public ItemViewSlotsContainerPanelBinding invPanelBinding;


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

    // public DisplayPanel mainMenuPanel;
    // public DisplayPanel spellMenuPanel;

    public SpriteRenderer backgroundImage;

    public Transform damageObjectPrefab;

    public Transform enemyNumberSpawnTransform;
    public Transform playerNumberSpawnTransform;

    public AudioSource musicSource;
    public AudioClip deathSound;

    public UnityEvent onWin;
    public UnityEvent onLose;

    private bool dobleAttack;

    void Start()
    {
        backgroundImage.sprite = currentBattle.backgroundImage;
        if (currentBattle.music != null)
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

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // PREVIOUS ITEM CALLBACKS
        // ItemCollection equipmentCollection = inventory.GetItemCollection("Equipped");
        // EventHandler.RegisterEvent(equipmentCollection, EventNames.c_ItemCollection_OnUpdate, () => OnItemChange());

        // NOTE: we only monitor main collection, as we are not interested to having updates regarding un/equipped stuff
        // Also, un/equipping is already 
        ItemCollection mainCollection = inventory.GetItemCollection("MainItemCollection");
        EventHandler.RegisterEvent(mainCollection, EventNames.c_ItemCollection_OnUpdate, () => OnItemUse());

        // NEW ITEM CALLBACKS
        //EventHandler.RegisterEvent(inventory, EventNames.c_Inventory_OnUpdate, () => OnItemChange());

        //Register player as the inventory panel owner
        InventorySystemManager.GetDisplayPanelManager().SetPanelOwner(player);

        // Notify UI that the battle is happening and bind on signal
        GameUIManager.Instance.SetUIMode(UIMode.BATTLE);
        GameUIManager.Instance.OnSpellWindowClosed += OnSpellWindowClosed;

        StartCoroutine(SetupBattle());
    }

    private void OnDestroy()
    {
        if (GameUIManager.Exist)
        {
            CloseInventory();
            GameUIManager.Instance.SetUIMode(UIMode.STANDARD);
            GameUIManager.Instance.OnSpellWindowClosed -= OnSpellWindowClosed;
        }
    }

    private void OnItemUse()
    {
        if (GameUIManager.Instance.InventoryShowing)
        {
            if (state != BattleState.PLAYERTURN)
            {
                return;
            }
            buttonsContainer.SetActive(false);
            // GameUIManager.Instance.InventoryPanel.OnOpen();
            // GameUIManager.Instance.EquipmentPanel.OnOpen();
            CloseInventory();
            StartCoroutine(ItemUse());
        }
    }

    IEnumerator ItemUse()
    {
        // wait one frame until item is applied to the stats
        yield return null;

        playerHUD.SetHUD(playerStats);
        // playerStats.UpdateUI();
        dialogueText.text = "Phendrin uses an item";

        yield return new WaitForSeconds(2f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
        playerHUD.SetHUD(playerStats);
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
        // playerStats.UpdateStats();

        if (playerStats.characterClass == "FIGHTER")
        {
            if (VerifySword())
            {
                playerStats.attackTotal = (playerStats.attackBase + Mathf.RoundToInt(1.2f * ObtainDamageWeaponEquipped())); //Redondea automaticamente
                Debug.Log(playerStats.attackBase + "attackbase");
                Debug.Log(playerStats.attackTotal + "attacktotal");
                Debug.Log(ObtainDamageWeaponEquipped() + "weapon damage");
            }
        }

        //Enemy setup
        enemyRenderer.sprite = currentBattle.enemy.sprite;
        enemyStats.characterName = currentBattle.enemy.enemyName;
        enemyStats.healthMax = enemyStats.healthTotal = currentBattle.enemy.hp;
        enemyStats.magicTotal = currentBattle.enemy.mp;
        enemyStats.attackTotal = currentBattle.enemy.attackDamage;
        enemyStats.defenseTotal = currentBattle.enemy.defense;
        enemyStats.speedTotal = currentBattle.enemy.speed;
        enemyStats.criticalHitProbability = currentBattle.enemy.criticalHitProbability;

        //Start battle
        dialogueText.text = enemyStats.characterName + " approaches.";

        yield return new WaitForSeconds(1.5f);

        playerHUD.SetHUD(playerStats);
        enemyHUD.SetHUD(enemyStats);

        statsUI.SetActive(true);
        buttonsContainer.SetActive(true);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack()
    {
        Debug.Log("Enumerator Attacl");

        playerAnimator.SetTrigger("Attack");

        VerifyBowEquipped();

        DamageValue damageValue = CalculateDamage(playerStats.attackTotal, enemyStats.defenseTotal, playerStats.criticalHitProbability);
        bool isDead = enemyStats.TakeDamage(damageValue.damageAmount);

        yield return new WaitForSeconds(1f);

        enemyAnimator.SetTrigger("Hurt");
        flashImage.StartFlash(0.25f, 0.5f, enemyHurtFlashColor);
        spawnDamageDisplay(enemyNumberSpawnTransform.position, damageValue.damageAmount, damageValue.isCritical);

        dialogueText.text = "Phendrin attacks " + enemyStats.characterName;

        enemyHUD.SetHUD(enemyStats);

        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = BattleState.WON;
            StartCoroutine(EndBattle());
        }
        else if (dobleAttack)
        {
            dobleAttack = false;
            StartCoroutine(PlayerAttack());
            yield break;
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        // playerHUD.SetHUD(playerStats);
        yield return new WaitForSeconds(0.25f);
        // playerHUD.SetHUD(playerStats);

        dialogueText.text = enemyStats.characterName + " attacks!";

        enemyAnimator.SetTrigger("Attack");

        yield return new WaitForSeconds(1f);

        DamageValue damageValue = CalculateDamage(enemyStats.attackTotal, playerStats.defenseTotal, enemyStats.criticalHitProbability);
        bool isDead = playerStats.TakeDamage(damageValue.damageAmount);

        playerAnimator.SetTrigger("Hurt");
        flashImage.StartFlash(0.25f, 0.5f, playerHurtFlashColor);
        spawnDamageDisplay(playerNumberSpawnTransform.position, damageValue.damageAmount, damageValue.isCritical);

        playerHUD.SetHUD(playerStats);

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
        //ItemCollection equipmentCollection = inventory.GetItemCollection("Equipped");
        ItemCollection mainCollection = inventory.GetItemCollection("MainItemCollection");
        //EventHandler.UnregisterEvent(equipmentCollection, EventNames.c_ItemCollection_OnUpdate, () => OnItemUse());
        EventHandler.UnregisterEvent(mainCollection, EventNames.c_ItemCollection_OnUpdate, () => OnItemUse());

        if (state == BattleState.WON)
        {
            currentBattle.wonBattle = true;
            dialogueText.text = enemyStats.characterName + " has been defeated.";
            ReactController.Instance.SignalDefeatMonster(currentBattle.enemy.enemyServerId.ToString());

            float disappearTimer = 1f;
            SpriteRenderer renderer = enemy.GetComponent<SpriteRenderer>();
            Color rendererColor = renderer.color;
            while (disappearTimer > 0)
            {
                rendererColor.a -= Time.deltaTime * 3f;
                renderer.color = rendererColor;
                disappearTimer -= Time.deltaTime;
            }

            playerStats.xp += currentBattle.enemy.xpDrop;

            var tempPlayer = GameObject.FindWithTag("Player");
            var inventory = tempPlayer.GetComponent<Inventory>();
            var currencyOwner = inventory.GetCurrencyComponent<CurrencyCollection>() as CurrencyOwner;
            var ownerCurrencyCollection = currencyOwner.CurrencyAmount;
            var gold = InventorySystemManager.GetCurrency("Gold");
            ownerCurrencyCollection.AddCurrency(gold, currentBattle.enemy.goldDrop);

            if (currentBattle.enemy.itemDrops.Length > 0)
            {
                GameUIManager.Instance.PlayerUI.transform.GetChild(0).gameObject.SetActive(false);
                GameUIManager.Instance.PlayerUI.transform.GetChild(1).gameObject.SetActive(false);
                GameUIManager.Instance.PlayerUI.gameObject.SetActive(true);

                var rewards = new List<ItemInfo>();
                var randomRewards = new List<ItemInfo>();

                foreach (var item in currentBattle.enemy.itemDrops)
                {
                    if (item.probability >= 100)
                        rewards.Add(item);
                    else
                        randomRewards.Add(item);
                }

                if (randomRewards.Count > 0)
                {
                    var totalProbability = 0;
                    foreach (var item in randomRewards)
                        totalProbability += item.probability;

                    var randomValue = UnityEngine.Random.Range(0, totalProbability);

                    var cumulativeProbability = 0;
                    foreach (var item in randomRewards)
                    {
                        cumulativeProbability += item.probability;
                        if (randomValue <= cumulativeProbability)
                        {
                            rewards.Add(item);
                            break;
                        }
                    }
                }

                foreach (ItemInfo itemInfo in rewards)
                {
                    var item = InventorySystemManager.CreateItem(itemInfo.Item);
                    inventory.AddItem(item, itemInfo.Amount);
                }

                yield return new WaitForSeconds(rewards.Count * 0.5f + 2);

                GameUIManager.Instance.PlayerUI.gameObject.SetActive(false);
                GameUIManager.Instance.PlayerUI.transform.GetChild(0).gameObject.SetActive(true);
                GameUIManager.Instance.PlayerUI.transform.GetChild(1).gameObject.SetActive(true);
            }

            if (currentBattle.enemy.goldDrop > 0)
            {
                DialogueManager.ShowAlert("Collected " + currentBattle.enemy.goldDrop + " Gold", 2f);
                yield return new WaitForSeconds(2.5f);
            }

            if (currentBattle.enemy.xpDrop > 0)
            {
                DialogueManager.ShowAlert("Gained " + currentBattle.enemy.xpDrop + " XP", 2f);
                yield return new WaitForSeconds(2.5f);
            }

            onWin.Invoke();
        }
        else if (state == BattleState.LOST)
        {
            musicSource.clip = deathSound;
            dialogueText.text = "Phendrin has been defeated.";
            portal.destinationSceneName = "Main Menu";
            musicSource.PlayOneShot(deathSound);

            onLose.Invoke();
            yield return new WaitForSeconds(5f);

            portal.UsePortal();
        }
        else if (state == BattleState.FLEEING)
        {
            currentBattle.ranBattle = true;
            dialogueText.text = "Success! Phendrin escapes " + currentBattle.enemy.enemyName;

            yield return new WaitForSeconds(2f);

            portal.setDestinationToPreviousScene();
            portal.UsePortal();
        }
    }


    private bool VerifySword()
    {
        ItemInfo[] pooledArray = new ItemInfo[10];
        var swordCategory = InventorySystemManager.GetItemCategory("Sword");
        var equippedContainer = inventory.GetItemCollection("Equipped");

        var itemInfoListSlice = equippedContainer.GetItemInfos(ref pooledArray, swordCategory,
             (candidateItemInfo, category) => category.InherentlyContains(candidateItemInfo.Item));

        if (itemInfoListSlice.Count > 0)
            return true;
        else
            return false;
    }

    private int ObtainDamageWeaponEquipped()
    {
        ItemInfo[] pooledArray = new ItemInfo[10];

        var bowcategory = InventorySystemManager.GetItemCategory("Sword");
        var equippedInventory = inventory.GetItemCollection("Equipped");

        var itemInfoListSlice = equippedInventory.GetItemInfos(ref pooledArray, bowcategory,
            (candidateItemInfo, category) => category.InherentlyContains(candidateItemInfo.Item));

        if (itemInfoListSlice.Count > 0)
        {
            var firstItemInfo = itemInfoListSlice[0];
            var attackWeapon = firstItemInfo.Item.GetAttribute<Attribute<int>>("Attack").GetValue();
            return attackWeapon;
        }
        else return 0;
    }


    private void VerifyBowEquipped()  // verifico si tengo equipado un bow para hacer su animacion
    {
        ItemInfo[] pooledArray = new ItemInfo[10];

        var bowcategory = InventorySystemManager.GetItemCategory("Bow");
        var equippedInventory = inventory.GetItemCollection("Equipped");

        var itemInfoListSlice = equippedInventory.GetItemInfos(ref pooledArray, bowcategory,
            (candidateItemInfo, category) => category.InherentlyContains(candidateItemInfo.Item));

        if (itemInfoListSlice.Count > 0)
        {
            var firstItemInfo = itemInfoListSlice[0];
            Debug.Log(firstItemInfo);

            if (firstItemInfo != null)
            {
                Arrow.SetActive(true);
                var animarrow = Arrow.GetComponent<Animator>();
                animarrow.SetTrigger("StartArrow");
            }
        }
        else
        {
            return;
        }
    }

    private bool VerifyBow()  // envia true si tengo equipado un bow
    {
        ItemInfo[] pooledArray = new ItemInfo[10];

        var bowcategory = InventorySystemManager.GetItemCategory("Bow");
        var equippedInventory = inventory.GetItemCollection("Equipped");

        var itemInfoListSlice = equippedInventory.GetItemInfos(ref pooledArray, bowcategory,
            (candidateItemInfo, category) => category.InherentlyContains(candidateItemInfo.Item));

        if (itemInfoListSlice.Count > 0)
            return true;
        else
            return false;
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

        CloseInventory();
        buttonsContainer.SetActive(false);

        if (playerStats.characterClass == "RANGER")
        {
            int chance = UnityEngine.Random.Range(0, 10);

            if (VerifyBow() && chance < 4)
            {
                dobleAttack = true;
            }
        }

        StartCoroutine(PlayerAttack());
    }

    public void OnRunButton()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }

        CloseInventory();
        StartCoroutine(OnRun());
    }

    public void OnSpellButton()
    {
        CloseInventory();
        GameUIManager.Instance.SpellMenu.SmartOpen();
    }

    public void OnItemButton()
    {
        GameUIManager.Instance.ToggleBattleInventoryPanel();
    }

    public void OnSpellUse(int manaNeeded, int magicPowerAdded, float magicPowerMultiplier, int healAmount, bool frostDamage, bool lightningDamage, bool fireDamage, string spellName, GameObject spellAnimation)
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }

        CloseInventory();
        StartCoroutine(UseSpell(manaNeeded, magicPowerAdded, magicPowerMultiplier, healAmount, frostDamage, lightningDamage, fireDamage, spellName, spellAnimation));
    }

    IEnumerator UseSpell(int manaNeeded, int magicPowerAdded, float magicPowerMultiplier, int healAmount, bool frostDamage, bool lightningDamage, bool fireDamage, string spellName, GameObject spellAnimation)
    {
        GameUIManager.Instance.SpellMenu.Close(true);
        GameUIManager.Instance.CloseSpellWindow();

        dialogueText.text = "Phendrin casts " + spellName;

        playerAnimator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.8f);
        Instantiate(spellAnimation, player.transform, true);
        yield return new WaitForSeconds(0.5f);

        playerStats.AddMana(-manaNeeded);
        playerStats.Heal(healAmount);
        playerHUD.SetHUD(playerStats);

        DamageValue damageValue = CalculateSpellDamage(magicPowerAdded, magicPowerMultiplier, enemyStats.defenseTotal, playerStats.criticalHitProbability, fireDamage, frostDamage, lightningDamage);
        if (damageValue.damageAmount != 0)
        {
            bool isDead = enemyStats.TakeDamage(damageValue.damageAmount);

            enemyAnimator.SetTrigger("Hurt");
            flashImage.StartFlash(0.25f, 0.5f, enemyHurtFlashColor);
            spawnDamageDisplay(enemyNumberSpawnTransform.position, damageValue.damageAmount, damageValue.isCritical);

            enemyHUD.SetHUD(enemyStats);

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
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    private void OnSpellWindowClosed()
    {
        buttonsContainer.SetActive(true);
    }

    IEnumerator OnRun()
    {
        buttonsContainer.SetActive(false);

        dialogueText.text = "Attempting to flee from " + currentBattle.enemy.enemyName + "...";

        yield return new WaitForSeconds(1.5f);

        float chance = UnityEngine.Random.Range(0f, 1f);
        if (chance > currentBattle.enemy.runSuccessProbability)
        {

            state = BattleState.FLEEING;
            StartCoroutine(EndBattle());
        }
        else
        {
            dialogueText.text = "Phendrin could not escape " + currentBattle.enemy.enemyName;
            yield return new WaitForSeconds(1.5f);
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    public void CloseInventory()
    {
        GameUIManager.Instance.ToggleBattleInventoryPanel(false);
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

        float randomizedValue = UnityEngine.Random.Range(initalValue - stdDev, initalValue + stdDev);

        int damageAmount = (int)System.Math.Ceiling(randomizedValue);

        bool isCritical = false;
        //Double if critical hit
        if (UnityEngine.Random.Range(0f, 1.0f) < criticalHitChance)
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

    public DamageValue CalculateSpellDamage(int magicPowerAdded, float magicPowerMultipler, int defense, float criticalHitChance, bool fireDamage, bool frostDamage, bool shockDamage)
    {
        //Calculate damage amount for spell
        int damageAmount = 0;
        bool isCritical = false;

        if (magicPowerAdded > 0)
        {
            damageAmount = playerStats.magicPowerTotal;
            damageAmount += magicPowerAdded;
        }
        if (magicPowerMultipler != 1 && magicPowerMultipler > 0)
        {
            if (damageAmount == 0)
            {
                damageAmount = playerStats.magicPowerTotal;
            }
            damageAmount = (int)Math.Round(damageAmount * magicPowerMultipler);
        }
        if (damageAmount > 0)
        {
            //Get value of damage
            float denominator = damageAmount + defense;
            float factor = damageAmount / denominator;
            float initalValue = damageAmount * factor;

            //Randomize
            float stdDev = 1.5f;
            if (playerStats.level > 3)
            {
                stdDev = playerStats.level / 2f;
            }

            float randomizedValue = UnityEngine.Random.Range(initalValue - stdDev, initalValue + stdDev);
            damageAmount = (int)System.Math.Ceiling(randomizedValue);

            //Calculate susceptibility to spells
            int currentDamageAmount = damageAmount;
            int fireDamageAmount = 0;
            int frostDamageAmount = 0;
            int shockDamageAmount = 0;

            if (currentBattle.enemy.fireSusceptibility > 0 && fireDamage)
            {
                fireDamageAmount = (int)Math.Round(currentDamageAmount * currentBattle.enemy.fireSusceptibility);
            }
            if (currentBattle.enemy.frostSusceptibility > 0 && frostDamage)
            {
                frostDamageAmount = (int)Math.Round(currentDamageAmount * currentBattle.enemy.frostSusceptibility);
            }
            if (currentBattle.enemy.shockSusceptibility > 0 && shockDamage)
            {
                shockDamageAmount = (int)Math.Round(currentDamageAmount * currentBattle.enemy.shockSusceptibility);
            }
            damageAmount = currentDamageAmount + fireDamageAmount + frostDamageAmount + shockDamageAmount;

            //Double if critical hit
            isCritical = false;
            if (UnityEngine.Random.Range(0f, 1.0f) < criticalHitChance)
            {
                damageAmount = damageAmount * 2;
                isCritical = true;
            }
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