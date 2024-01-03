using Opsive.Shared.Input.VirtualControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Opsive.Shared.Input
{

    public class MobileInput : MonoBehaviour
    {
        [SerializeField] private VirtualJoystick m_VirtualJoystick;
        [SerializeField] private Button m_InteractButton;
        [SerializeField] private Button m_RunButton;
        [SerializeField] GameObject _MobileUI;


        public bool running;

        void Start()
        {
            if (CheckMobile.IsMobile)
            {
                m_VirtualJoystick.enabled = true;
                _MobileUI.SetActive(true);
            }



        }

        void Update()
        {
        
        }
    }
}
