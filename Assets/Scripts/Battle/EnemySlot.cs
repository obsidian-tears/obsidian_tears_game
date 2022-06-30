using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySlot : MonoBehaviour
{
    public EnemyOld enemy;
    public Image image;
    public void Setup(EnemyOld newEnemy) {
        enemy = newEnemy;
        image.preserveAspect = true;
        image.sprite = newEnemy.sprite;
        image.SetNativeSize();
    }
}