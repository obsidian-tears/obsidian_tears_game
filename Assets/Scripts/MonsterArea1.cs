using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterArea1 : MonoBehaviour
{
    public float initProb = 0.1f;
    public List<RegionalMonster> monsters;
    public MySignal signal;
    [SerializeField] bool active;
    [SerializeField] float probability;
    [SerializeField] GameObject battleTransition;
    [SerializeField] CurrentEnemy currentEnemy;
    //float transitionWait = 1.0f;

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
                signal.Raise();
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
            foreach (RegionalMonster regionalMonster in monsters)
            {
                tempProb += regionalMonster.probability;
                currentEnemy.enemy = regionalMonster.enemy;
                if (randVal < tempProb)
                {
                    break;
                }
            }
            // navigate to battle scene
            ////StartCoroutine(StartBattleTransitionCo());
        }
    }

    /*Note from Isaac: I created a scriptable object called "SceneManager" that can be called from events to load scenes, including load previous scenes. Doing 
         * so from events is better practice than naming scenes through strings within code, so I commented out the places where scene transitions have been taking place
         * and changed them to be handled in the respective UnityEvents. The scenes still load asynchronously so the transitions still work for now*/
    /*public IEnumerator StartBattleTransitionCo()
    {
        Debug.Log("Wrong place to load scene");
        Time.timeScale = 0;
        if (battleTransition)
        {
            Instantiate(battleTransition, Vector3.zero, Quaternion.identity);
        }
        yield return new WaitForSecondsRealtime(transitionWait);
        Time.timeScale = 1;
        
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Battle");
        while (!asyncOperation.isDone) yield return null;
    }*/
    void Start()
    {
        probability = initProb;
    }

}
