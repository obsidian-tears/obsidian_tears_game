using UnityEngine;
using System.Runtime.InteropServices;

public static class ReactFunctions 
{
    public static void GetNFT()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        ReactFunctionsInternal.GetNFT();
#endif
    }
    public static void LoginIc()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        ReactFunctionsInternal.LoginIc();
#endif
    }
    public static void SetUserAlias(string wallet, string newAlias)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
    ReactFunctionsInternal.SetUserAlias(wallet,newAlias);
#endif
    }

}
public static class ReactFunctionsInternal
{
    [DllImport("__Internal")]
    public static extern void SetUserAlias(string wallet, string newAlias);

    [DllImport("__Internal")]
    public static extern void GetNFT();
    [DllImport("__Internal")]
    public static extern void LoginIc();
}
