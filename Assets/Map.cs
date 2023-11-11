using PixelCrushers;
using PixelCrushers.Wrappers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Map : MonoBehaviour
{
    public RectTransform playerInMap;
    public RectTransform map2dEnd;
    public Transform map3dParent;
    public Transform map3dEnd; // Game Object that is in overworld, that why i set the coordinates below when you change scene.

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
            //get the floats that saved when i change scene.

            float boxColliderPositionX = PlayerPrefs.GetFloat("BoxColliderPositionX");
            float boxColliderPositionY = PlayerPrefs.GetFloat("BoxColliderPositionY");
            float boxColliderPositionZ = PlayerPrefs.GetFloat("BoxColliderPositionZ");

            if (map3dEnd == null && map3dParent == null )
            {
                _playerGO = new GameObject("PlayerGoNew");
                _playerGO.transform.position = new Vector3(boxColliderPositionX, boxColliderPositionY, boxColliderPositionZ);
               // create the game objects that aren't in the others scenes and i set the transform doing this.
                map3dParent = new GameObject("MapCenterCoordinates").transform;                
                map3dParent.transform.position = new Vector3(0, 0, 0);
                map3dEnd = new GameObject("MapEndCoordinates").transform;
                map3dEnd.transform.position = new Vector3(0, 0, 0);
                map3dEnd.SetParent(map3dParent);
                //set the transform of the original map, if there are more that one, this will need to be changed so it can be used on another maps.
                map3dEnd.transform.position = new Vector3(257.7f, 206.6f, 0f);
                map3dParent.transform.position =  new Vector3(-197.508f, 163.817f, 0f);
                
                //changed the original values to use the new ones getting the player prefs floats.
                normalized = Divide(map3dParent.InverseTransformPoint(_playerGO.transform.position), map3dEnd.position - map3dParent.position);                
                mapped = Multiply(normalized, map2dEnd.localPosition);
                mapped.z = 0;
                playerInMap.localPosition = mapped;
               
            }
            else
            {
                //first time will create the game objects that aren't in scene, then it will be doing this part of the code.
                normalized = Divide(map3dParent.InverseTransformPoint(_playerGO.transform.position), map3dEnd.position - map3dParent.position);               
                mapped = Multiply(normalized, map2dEnd.localPosition);                        
                mapped.z = 0;
                playerInMap.localPosition = mapped;    

            }


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
