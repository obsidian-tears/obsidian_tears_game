using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using PixelCrushers;
using Opsive.UltimateInventorySystem.Core.DataStructures;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using Opsive.UltimateInventorySystem.Exchange;
using System.Text;

[System.Serializable]
public class OnBattleStart : UnityEvent { }

[System.Serializable]
public class OnBattleWin : UnityEvent { }

[System.Serializable]
public class OnBattleRan : UnityEvent { }

[RequireComponent(typeof(DestructibleSaver))]
public class MonsterArea : MonoBehaviour
{
    public string monsterAreaUniqueID;

    public int minSecondsToBattleStart;
    public int maxSecondsToBattleStart;
    public Sprite backgroundImage;
    public AudioClip musicClip;
    public bool isOneTimeBattle;
    public List<EnemiesList> enemies;

    private float probability;
    private bool active;
    private Battle currentBattle;
    private ScenePortal scenePortal;

    private float t;

    public OnBattleStart onBattleStart;
    public OnBattleWin onBattleWin;
    public OnBattleRan onBattleRan;

    private Player player;

    private MobileInput mobileInput;


    public void OnTriggerEnter2D(Collider2D collider)
    {

        if (collider.CompareTag("Player"))
        {
            player = collider.gameObject.GetComponent<Player>();
            active = true;
            t = 0;
            probability = Random.Range(minSecondsToBattleStart, maxSecondsToBattleStart);
            mobileInput = FindObjectOfType<MobileInput>();

        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            active = false;
        }
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            if (active && player.change != Vector3.zero || (mobileInput && mobileInput.joystickInput != Vector3.zero))
            {
                if (t > probability)
                {
                    OnBattleSignal();
                }
                else
                {
                    t += (player.isRunning ? 1.5f : 1f) * Time.deltaTime;
                }
            }
        }
    }

    public void OnBattleSignal()
    {
        if (active)
        {
            float randVal = Random.value;
            float tempProb = 0.0f;
            // choose the monster to battle
            foreach (EnemiesList enemy in enemies)
            {
                tempProb += enemy.enemyProbability;
                currentBattle.enemy = enemy.enemy;
                if (randVal < tempProb)
                {
                    break;
                }
            }
            // navigate to battle scene
            StartCoroutine(StartBattle());
        }
    }

    IEnumerator StartBattle()
    {
        if (monsterAreaUniqueID == null)
            monsterAreaUniqueID = gameObject.name;
        currentBattle.monsterAreaObject = monsterAreaUniqueID;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        playerObj.GetComponent<Player>().Freeze();

        probability = 0f;
        onBattleStart.Invoke();
        FlashImage flashImage = (FlashImage)FindObjectOfType(typeof(FlashImage));
        if (flashImage != null)
        {
            flashImage.StartFlash(1f, 0.99f, Color.white);
        }


        CharStats player = playerObj.GetComponent<CharStats>();

        currentBattle.playerBaseHealth = player.healthBase;
        currentBattle.playerCurrentHealth = player.healthTotal;
        currentBattle.playerBaseMagic = player.magicBase;
        currentBattle.playerCurrentMagic = player.magicTotal;
        currentBattle.playerAttackBase = player.attackBase;
        currentBattle.playerDefenseBase = player.defenseBase;
        currentBattle.playerSpeedBase = player.speedBase;
        currentBattle.music = musicClip;

        currentBattle.level = player.level;
        currentBattle.backgroundImage = backgroundImage;

        yield return new WaitForSeconds(1f);

        scenePortal.UsePortal();
    }

    void Awake()
    {
        probability = 0;
        currentBattle = GameObject.Find("SceneManager").GetComponent<ObjectHolder>().currentBattle;

        if (currentBattle.monsterAreaObject == monsterAreaUniqueID)
        {
            if (currentBattle.wonBattle)
            {
                var tempPlayer = GameObject.FindWithTag("Player");
                var inventory = tempPlayer.GetComponent<Inventory>();
                ItemInfo[] rewards = currentBattle.enemy.itemDrops;

                foreach (ItemInfo itemInfo in rewards)
                {
                    var item = InventorySystemManager.CreateItem(itemInfo.Item);
                    inventory.AddItem(item, itemInfo.Amount);
                }

                ReactController.Instance.SignalDefeatMonster(currentBattle.enemy.enemyServerId.ToString());
                onBattleWin.Invoke();

                if (isOneTimeBattle)
                    Destroy(this.gameObject);
            }
            else if (currentBattle.ranBattle)
                StartCoroutine(DelayRun());
        }
    }

    IEnumerator DelayRun()
    {
        // Allow SaveSystem script to ApplySavedGameData() before transitioning to next scene; Otherwise empty data is saved and all inventory and quest data is lost.
        yield return new WaitForSeconds(0.05f);
        onBattleRan.Invoke();
    }

    void Start()
    {
        scenePortal = gameObject.AddComponent<ScenePortal>();
        scenePortal.requiredTag = "Player";
        scenePortal.destinationSceneName = "Battle";

        currentBattle.wonBattle = false;
        currentBattle.ranBattle = false;
        currentBattle.monsterAreaObject = null;
    }

    void BattleStarted()
    {
        Debug.Log("Battle started");
    }

}


[System.Serializable]
public struct EnemiesList
{
    public EnemiesList(Enemy enemy, float enemyProbability)
    {
        this.enemy = enemy;
        this.enemyProbability = enemyProbability;
    }
    public Enemy enemy;
    [Range(0f, 1.0f)]
    public float enemyProbability;
}



/*----------------------------- OVERVIEW -------------------------------//
 *   <<< NAME >>>
 *       -- Ranged Float.
 *       
 *   <<< DESCRIPTION >>>
 *       -- Class intended to be an alternative to Vector2 for inspector usage of ranged values.
 *
 *   <<< DEPENDENCIES >>>
 *       -- Plugins: None.
 *       -- Module: 
 *          -- None.
//----------------------------------------------------------------------*/


[System.Serializable]
public class RangedInt
{
    //------------------------------------------------------------------------------------//
    //----------------------------------- FIELDS -----------------------------------------//
    //------------------------------------------------------------------------------------//

    public int min;
    public int max;

    //------------------------------------------------------------------------------------//
    //---------------------------------- METHODS -----------------------------------------//
    //------------------------------------------------------------------------------------//

    public void Init()
    {
        min = 0;
        max = 1;
    }//End of Init()


    public RangedInt()
    {
        Init();
    }//End of MinMaxFloat()


    public RangedInt(int min, int max)
    {
        this.min = min;
        this.max = max;
    }//End of MinMaxFloat(float min, float max)


    public int GetRandomValue()
    {
        return Random.Range(min, max);
    }//End of getRandomValue


    public override string ToString()
    {
        return string.Format("[Class: {0}, Min: {1}, Max: {2}]", typeof(RangedInt).Name, min, max);
    }//End of ToString()


    public static implicit operator int(RangedInt someRangedInt)
    {
        return someRangedInt.GetRandomValue();
    }//End of implicit operator float

}//End of MinMaxFloat

[System.Serializable]
public class RangedIntAttribute : PropertyAttribute
{

    //------------------------------------------------------------------------------------//
    //----------------------------- ENUM DECLARATIONS ------------------------------------//
    //------------------------------------------------------------------------------------//

    public enum RangeDisplayType // [1]
    {
        LockedRanges,
        EditableRanges,
        HideRanges
    }

    //------------------------------------------------------------------------------------//
    //----------------------------------- FIELDS -----------------------------------------//
    //------------------------------------------------------------------------------------//

    public int max;
    public int min;
    public RangeDisplayType rangeDisplayType;

    //------------------------------------------------------------------------------------//
    //---------------------------------- METHODS -----------------------------------------//
    //------------------------------------------------------------------------------------//

    public RangedIntAttribute(int min, int max, RangeDisplayType rangeDisplayType = RangeDisplayType.LockedRanges)
    {
        this.min = min;
        this.max = max;
        this.rangeDisplayType = rangeDisplayType;
    }//End of RangedIntAttribute()

}//End of class