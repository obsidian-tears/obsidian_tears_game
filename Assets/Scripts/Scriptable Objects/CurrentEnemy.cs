using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="CurrentEnemy", menuName ="Enemies/Current Enemy")]
public class CurrentEnemy : ScriptableObject
{
    public Enemy enemy;
}
