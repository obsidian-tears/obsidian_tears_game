using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySlot : MonoBehaviour
{
    public Enemy enemy;
    public Image image;
    public void Setup(Enemy newEnemy) {
        enemy = newEnemy;
        image.preserveAspect = true;
        image.sprite = newEnemy.sprite;
        image.SetNativeSize();
    }
}