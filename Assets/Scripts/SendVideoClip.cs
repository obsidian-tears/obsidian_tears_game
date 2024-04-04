using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SendVideoClip : MonoBehaviour
{
    

    public void OnInteract(string url)
    {
        PlayerPrefs.SetString("urlParaReproducir", url);
        //actualscene =  SceneManager.GetActiveScene();
        
        //PlayerPrefs.SetString("backscene", actualscene.ToString());
       // Debug.Log(actualscene);
        //SceneManager.LoadScene("GranGranFirst");
    }


}
