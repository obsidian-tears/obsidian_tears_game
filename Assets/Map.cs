using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Map : MonoBehaviour
{
    public RectTransform playerInMap;
    public RectTransform map2dEnd;
    public Transform map3dParent;
    public Transform map3dEnd;

    private Vector3 normalized, mapped;
     

    private GameObject map3dP, map3dE, _playerGO;

    private void OnEnable()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Overworld")
        {
            map3dP = GameObject.Find("MapCenterCoordinates");
            map3dE = GameObject.Find("MapEndCoordinates");
            _playerGO = GameObject.FindWithTag("Player");
           
            map3dParent = map3dP?.transform;
            map3dEnd = map3dE?.transform;


            if (map3dParent != null || map3dEnd != null)
            {
                normalized = Divide(map3dParent.InverseTransformPoint(_playerGO.transform.position), map3dEnd.position - map3dParent.position);
                //normalized.y = normalized.z;
                mapped = Multiply(normalized, map2dEnd.localPosition);
                mapped.z = 0;
                playerInMap.localPosition = mapped;

            }

        }
        else
        {


            float boxColliderPositionX = PlayerPrefs.GetFloat("BoxColliderPositionX");
            float boxColliderPositionY = PlayerPrefs.GetFloat("BoxColliderPositionY");
            float boxColliderPositionZ = PlayerPrefs.GetFloat("BoxColliderPositionZ");

            Vector3 boxcolliderpositionnew = new Vector3(boxColliderPositionX, boxColliderPositionY, boxColliderPositionZ);
            map3dParent = new GameObject("MapCenterCoordinates").transform;
            map3dParent.position =  new Vector3(257.7f, 206.6f, 0f);

            map3dEnd = new GameObject("MapEndCoordinates").transform;
            map3dEnd.position = new Vector3(-197.508f, 163.817f, 0f);

            Debug.Log(boxcolliderpositionnew + " boxcollider position " + " (player)");
            Debug.Log(map3dEnd.transform.position + " mapEnd ");
            Debug.Log(map3dParent.transform.position + " Map3dParent ");

            // Actualiza la posición del mapa con la posición del Box Collider guardada
            normalized = Divide(map3dParent.InverseTransformPoint(boxcolliderpositionnew.x, boxcolliderpositionnew.y, boxcolliderpositionnew.z), map3dEnd.position - map3dParent.position);
             
            //normalized.y = normalized.z;
            mapped = Multiply(normalized, map2dEnd.localPosition);
            mapped.z = 0;
            playerInMap.localPosition = mapped;


        }

    }





    private static Vector3 Divide(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    }

    private static Vector3 Multiply(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }
}
