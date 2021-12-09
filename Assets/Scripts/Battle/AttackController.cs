using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    [SerializeField] FloatValue currentHealth;
    [SerializeField] FloatValue currentMagic;
    [SerializeField] float currentEnemyHealth;
    [SerializeField] float currentEnemyMagic;
    [SerializeField] Stats playerStats;
    [SerializeField] CurrentEnemy currentEnemy;
    [SerializeField] MySignal playerSignal;
    void Start()
    {
        currentEnemyHealth = currentEnemy.enemy.maxHealth;
        currentEnemyMagic = currentEnemy.enemy.maxMagic;
        playerSignal.Raise();
    }
    public float Attack()
    {
        currentEnemyHealth -= playerStats.strength.value;
        return playerStats.strength.value;
    }

    public float Spell(Spell spell)
    {
        currentEnemyHealth -= spell.spellPower;
        return spell.spellPower;
    }

    public BattleState CheckBattleEnd() {
        if (currentEnemyHealth <= 0) {
            return BattleState.PlayerWon;
        } else if (currentHealth.value <= 0) {
            return BattleState.PlayerLost;
        } else return BattleState.Busy;
    }

    public float EnemyAttack()
    {
        currentHealth.value -= currentEnemy.enemy.strength;
        playerSignal.Raise();
        return currentEnemy.enemy.strength;
    }
}
