using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public RectTransform playerInMap;
    public RectTransform map2dEnd;
    public Transform map3dParent;
    public Transform map3dEnd;

    private Vector3 normalized, mapped;
     

    private GameObject map3dP, map3dE, _playerGO;

    private void Awake()
    {

        map3dP = GameObject.Find("MapCenterCoordinates");
        map3dE = GameObject.Find("MapEndCoordinates");
        _playerGO = GameObject.FindWithTag("Player");

        map3dParent = map3dP.transform;
        map3dEnd = map3dE.transform;        
        
    }

    private void Update()
    {
        if (map3dParent != null || map3dEnd != null)
        {
            normalized = Divide (map3dParent.InverseTransformPoint(_playerGO.transform.position),map3dEnd.position - map3dParent.position);
            //normalized.y = normalized.z;
            mapped = Multiply(normalized, map2dEnd.localPosition);
            mapped.z = 0;
            playerInMap.localPosition = mapped;

        }
        else
        {
            //button disable.
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
