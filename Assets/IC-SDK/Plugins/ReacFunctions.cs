using System.Runtime.InteropServices;

namespace IC_SDK
{
    public static class ReacFunctionsInternal
    {
        [DllImport("__Internal")]
        public static extern void LoginIc();

        [DllImport("__Internal")]
        public static extern void GetNFT();

        [DllImport("__Internal")]
        public static extern void SetUserName(string json);
    }

    public static class ReacFunctions
    {
        public static void GetNFT()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
        ReacFunctionsInternal.GetNFT();
#endif
        }

        public static void LoginIc()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
        ReacFunctionsInternal.LoginIc();
#endif
        }

        public static void SetUserName(string json)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
        ReacFunctionsInternal.SetUserName(json);
#endif
        }
    }
}