using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckMobile : MonoBehaviour
{
    public static bool IsMobile { get; private set; }
    // public static bool IsMobile { get; private set; } = false;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

    }


    public void CheckMobilePlatform(int isMobile)
    {
        if (isMobile == 1) 
        {
            IsMobile = true;
            PlayerPrefs.SetString("IsMobile","true");
        
        }
        else IsMobile = false;
    }
}
