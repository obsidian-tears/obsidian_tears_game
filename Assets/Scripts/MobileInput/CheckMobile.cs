using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckMobile : MonoBehaviour
{
#if UNITY_ANDROID
    public static bool IsMobile { get; private set; } = true;

#elif UNITY_EDITOR_WIN
    public static bool IsMobile { get; private set; } = false;
#endif

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }


    public void CheckMobilePlatform(int isMobile)
    {
        if (isMobile == 1) IsMobile = true;

        else IsMobile = false;
    }
}
