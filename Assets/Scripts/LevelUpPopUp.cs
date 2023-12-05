using GameManagers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpPopUp : MonoBehaviour
{
    public GameObject _canvasLevelUp;



    private void Start()
    {
       // StartCoroutine(Tiempo());
    }



    public void OpenLevelUp()
    {
        var kk = FindObjectOfType<GameUIManager>().LevelUpMenu;
        kk.SetActive(true);
        
        Debug.Log(kk.name + "canvas");
       //_canvasLevelUp.SetActive(true);

    }

    IEnumerator Tiempo()
    {

        yield return new WaitForSeconds(0.1f);
        _canvasLevelUp = GameObject.FindWithTag("LevelUp");
        Debug.Log(_canvasLevelUp.name + "canvasEnumerator");
    }

}
