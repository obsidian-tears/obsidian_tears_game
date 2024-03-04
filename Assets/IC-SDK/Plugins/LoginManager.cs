using UnityEngine;
using System;

namespace IC_SDK
{
    public class LoginManager : MonoBehaviour
    {
        public static LoginManager Instance;
    
        public string principal;
        public bool successfulLogin = false;
        public User user;
        public event Action<User> e_SuccessfulLogin = delegate {  };

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("There is an existing Login Manager, destroying...");
                Destroy(this);
                return;
            }
            
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        public void GetPrincipal(string json)
        {
            Debug.Log("Execute get principal");
            if (json != null)
            {
                user = JsonUtility.FromJson<User>(json);

                Debug.Log($"el principal que recibo es estse:  {user.principal}");

                if (user != null)
                {
                    this.successfulLogin = true;
                    this.principal = user.principal;
                    e_SuccessfulLogin(user);
                    NFTManager.RequestNFTs();
                }
                else
                {
                    Debug.Log("Error en getPrincipal2");
                }
            }
            else
            {
                Debug.LogError("Error in getPrincipal");
            }
        }

        public void LoginIC()
        {
#if UNITY_EDITOR
            Debug.Log("Logueandose...");
#endif

            ReacFunctions.LoginIc();
        }
    }
}