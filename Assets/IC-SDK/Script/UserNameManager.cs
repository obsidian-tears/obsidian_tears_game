using UnityEngine;
using UnityEngine.UI;

namespace IC_SDK
{
    public class UserNameManager : MonoBehaviour
    {
        [SerializeField] private Text _userNameText;
        [SerializeField] private Text _inputFieldText;

        private void Start()
        {
            LoginManager.Instance.e_SuccessfulLogin += ChangeUserName;
        }

        public void SetNewName()
        {
            if (_inputFieldText.text.Length<3)
            {
                Debug.LogError("The name is to short");
                return;
            }
            
            if (_inputFieldText.text.Length>15)
            {
                Debug.LogError("The name is to long");
                return;
            }
            
            ReacFunctions.SetUserName(_inputFieldText.text);
            _userNameText.text = _inputFieldText.text;
        }

        private void ChangeUserName(User user)
        {
            _userNameText.text = user.userName;
            _inputFieldText.text = user.userName;
        }
    }
}