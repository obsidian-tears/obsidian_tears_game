using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using PixelCrushers;

[System.Serializable]
public class OnBattleStart : UnityEvent { }


public class MonsterArea : MonoBehaviour
{
    public float initProb = 0.1f;
    public List<EnemiesList> enemies;

    private float probability;
    private bool active;
    private Battle currentBattle;
    private ScenePortal scenePortal;

    public OnBattleStart onBattleStart;

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            active = true;
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            active = false;
        }
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (active)
        {
            float randVal = Random.value;
            if (randVal < probability * Time.deltaTime)
            {
                OnBattleSignal();
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
            StartBattle();
        }
    }

    public void StartBattle()
    {
        Debug.Log("Battle starting");
        onBattleStart.Invoke();
        CharStats player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharStats>();

        currentBattle.playerBaseHealth = player.healthBase;
        currentBattle.playerCurrentHealth = player.healthTotal;
        currentBattle.playerBaseMagic = player.magicBase;
        currentBattle.playerCurrentMagic = player.magicTotal;
        currentBattle.playerAttackBase = player.attackBase;
        currentBattle.playerDefenseBase = player.defenseBase;
        currentBattle.playerSpeedBase = player.speedBase;

        scenePortal.UsePortal();
    }


    void Start()
    {
        probability = initProb;
        currentBattle = GameObject.Find("SceneManager").GetComponent<ObjectHolder>().currentBattle;

        scenePortal = gameObject.AddComponent<ScenePortal>();
        scenePortal.requiredTag = "Player";
        scenePortal.destinationSceneName = "Battle";
    }


    void BattleStarted()
    {
        Debug.Log("Battle started");
    }

}


[System.Serializable]
public struct EnemiesList
{
    public Enemy enemy;
    [Range(0f,1.0f)]
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