using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Linq;
public class AttackController : MonoBehaviour
{
    [SerializeField] FloatValue currentHealth;
    [SerializeField] FloatValue currentMagic;
    [SerializeField] float currentEnemyHealth;
    [SerializeField] float currentEnemyMagic;
    [SerializeField] Stats playerStats;
    [SerializeField] PlayerInventory playerInventory;
    [SerializeField] CurrentEnemy currentEnemy;
    [SerializeField] MySignal playerSignal;


    void Start()
    {
        currentEnemyHealth = currentEnemy.enemy.maxHealth;
        currentEnemyMagic = currentEnemy.enemy.maxMagic;
        playerSignal.Raise();
    }
    public float Attack(bool critical)
    {
        float rand = Random.Range(-0.2f, 0.2f);
        List<InventoryItem> weapons = playerInventory.equipment.myInventory.FindAll((item) => (item is Weapon));
        float weaponDamage = weapons.Sum((item) => (item as Weapon).baseDamage);
        float totalDamage = weaponDamage + playerStats.strength.value;
        float damage = Mathf.Ceil((rand * totalDamage + totalDamage) * (critical ? 2 : 1));
        currentEnemyHealth -= damage;
        return damage;
    }

    public float Spell(SpellObject spell)
    {
        if (currentMagic.value < spell.spellCost) return -1f;
        currentMagic.value -= spell.spellCost;
        currentEnemyHealth -= spell.spellPower;
        playerSignal.Raise();
        return spell.spellPower;
    }

    public BattleStateOld CheckBattleEnd()
    {
        if (currentEnemyHealth <= 0)
        {
            return BattleStateOld.PlayerWon;
        }
        else if (currentHealth.value <= 0)
        {
            return BattleStateOld.PlayerLost;
        }
        else return BattleStateOld.Busy;
    }

    public float EnemyAttack()
    {
        float damage = Mathf.Ceil(currentEnemy.enemy.strength);
        currentHealth.value -= damage;
        playerSignal.Raise();
        return damage;
    }
}
