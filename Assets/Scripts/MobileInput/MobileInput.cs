using Opsive.Shared.Input.VirtualControls;
using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
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

        [SerializeField] private Player _player;



        void Start()
        {
            if (CheckMobile.IsMobile)
            {
                m_VirtualJoystick.enabled = true;
                _MobileUI.SetActive(true);


                if (m_InteractButton != null) m_InteractButton.onClick.AddListener(OnInteractButtonClicked);
                if (m_RunButton != null)
                {
                    m_RunButton.onClick.AddListener(OnRunButtonPressed);
                    if (InputDeviceManager.IsButtonUp("RunButton"))
                    {
                        OnRunButtonReleased();
                    }

                }

            }

        }


        void Update()
        {
            Vector3 joystickInput = new Vector3(m_VirtualJoystick.GetAxis("Horizontal"), m_VirtualJoystick.GetAxis("Vertical"), 0f).normalized;
            Move(joystickInput);
            Sprint(running);

        }



        private void Move(Vector3 direction)
        {            
            _player.myRigidbody.MovePosition(transform.position + direction * _player.speed * Time.deltaTime);
            _player.animator.SetFloat("moveX", direction.x);
            _player.animator.SetFloat("moveY", direction.y);
            _player.animator.SetBool("moving", direction.magnitude > 0);

        }

      
        private void Sprint(bool isSprinting)
        {
            
        }

        private void OnInteractButtonClicked()
        {
            
        }


        private void OnRunButtonPressed()
        {
            running = true;
        }

        private void OnRunButtonReleased()
        {
            running = false;
        }




    }
}
