using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCollider : MonoBehaviour
{
    [SerializeField] Transform colliderTransform;
    

    private void OnTriggerEnter2D(Collider2D collider)
    {
        PlayerPrefs.SetFloat("BoxColliderPositionX", colliderTransform.transform.position.x);
        PlayerPrefs.SetFloat("BoxColliderPositionY", colliderTransform.transform.position.y);
        PlayerPrefs.SetFloat("BoxColliderPositionZ", colliderTransform.transform.position.z);
    }
}
