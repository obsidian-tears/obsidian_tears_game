using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New RegionalMonster", menuName = "Enemies/Regional Monster")]
public class RegionalMonster : ScriptableObject
{
    public EnemyOld enemy;
    public float probability;
}
