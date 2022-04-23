using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: delete this object. Too redundant. 
// Replace with list of monsters and probabilites in monster area
[CreateAssetMenu(fileName = "New RegionalMonster", menuName = "Enemies/Regional Monster")]
public class RegionalMonster : ScriptableObject
{
    public Enemy enemy;
    public float probability;
}
