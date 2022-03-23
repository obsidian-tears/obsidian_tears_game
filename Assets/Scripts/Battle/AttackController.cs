using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
public class AttackController : MonoBehaviour
{
    [SerializeField] FloatValue currentHealth;
    [SerializeField] FloatValue currentMagic;
    [SerializeField] float currentEnemyHealth;
    [SerializeField] float currentEnemyMagic;
    [SerializeField] Stats playerStats;
    [SerializeField] CurrentEnemy currentEnemy;
    [SerializeField] MySignal playerSignal;

    [DllImport("__Internal")] private static extern void GameOver();

    void Start()
    {
        currentEnemyHealth = currentEnemy.enemy.maxHealth;
        currentEnemyMagic = currentEnemy.enemy.maxMagic;
        playerSignal.Raise();
    }
    public float Attack(bool critical)
    {
        float rand = Random.Range(-0.2f, 0.2f);
        float damage = Mathf.Ceil(rand*playerStats.strength.value + playerStats.strength.value * (critical ? 2 : 1));
        currentEnemyHealth -= damage;
        return damage;
    }

    public float Spell(Spell spell)
    {
        currentEnemyHealth -= spell.spellPower;
        return spell.spellPower;
    }

    public BattleState CheckBattleEnd()
    {
        if (currentEnemyHealth <= 0)
        {
            return BattleState.PlayerWon;
        }
        else if (currentHealth.value <= 0)
        {
            #if UNITY_WEBGL == true && UNITY_EDITOR == false
                GameOver();
            #endif
            return BattleState.PlayerLost;
        }
        else return BattleState.Busy;
    }

    public float EnemyAttack()
    {
        float damage = Mathf.Ceil(currentEnemy.enemy.strength);
        currentHealth.value -= damage;
        playerSignal.Raise();
        return damage;
    }
}
