using UnityEngine;

namespace IC_SDK
{
    public class CanvasManager : MonoBehaviour
    {
        [SerializeField] private GameObject _loginPanel;
        [SerializeField] private GameObject _afterLoginPanel;
        private void Start()
        {
            LoginManager.Instance.e_SuccessfulLogin += (user) =>
            {
                _loginPanel.SetActive(false);
                _afterLoginPanel.SetActive(true);
            };
        }
    }
}