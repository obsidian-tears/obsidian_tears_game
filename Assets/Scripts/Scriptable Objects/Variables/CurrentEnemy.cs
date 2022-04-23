using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: change to EnemyVariable
[CreateAssetMenu(fileName ="CurrentEnemy", menuName ="Enemies/Current Enemy")]
public class CurrentEnemy : ScriptableObject
{
    public Enemy enemy;
}
