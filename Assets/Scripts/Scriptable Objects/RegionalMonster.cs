using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New RegionalMonster", menuName = "Enemies/Regional Monster")]
public class RegionalMonster : ScriptableObject
{
    public Enemy enemy;
    public float probability;
}
