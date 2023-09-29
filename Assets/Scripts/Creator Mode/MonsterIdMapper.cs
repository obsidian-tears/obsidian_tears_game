using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterIdMapper : MonoBehaviour
{
    public List<Enemy> enemies = new List<Enemy>();

    public Enemy GetMonsterFromId(int id)
    {
        return enemies[id];
    }
}
