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

#if UNITY_ANDROID
        IsMobile = true;
#else
        IsMobile = false;
#endif
    }


    public void CheckMobilePlatform(int isMobile)
    {
        if (isMobile == 1) IsMobile = true;

        else IsMobile = false;
    }
}
