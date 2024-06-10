using Opsive.Shared.Input.VirtualControls;
using UnityEngine;
using static Opsive.UltimateInventorySystem.DatabaseNames.DemoInventoryDatabaseNames;


//namespace Opsive.Shared.Input
//{

public class MobileInput : MonoBehaviour
{
    [SerializeField] private VirtualJoystick m_VirtualJoystick;
    //[SerializeField] private VirtualButton m_InteractButton;
    [SerializeField] private VirtualButton m_RunButton;
    [SerializeField] GameObject _MobileUI;

    [SerializeField] private VirtualControlsManager controlsManager;
    public Vector3 joystickInput;

    public bool running;

    [SerializeField] private Player _player;


    void Start()
    {
        setPlayer();

        if (CheckMobile.IsMobile || PlayerPrefs.GetString("IsMobile") == "true")
        {
            m_VirtualJoystick.enabled = true;
            _MobileUI.SetActive(true);           

        }

    }

    private void Update()
    {
        if (!CheckMobile.IsMobile)
        {
            return;
        }
        
        _player.animator.SetFloat("moveX", m_VirtualJoystick.GetAxis("Horizontal"));
        _player.animator.SetFloat("moveY", m_VirtualJoystick.GetAxis("Vertical"));
        _player.animator.SetBool("moving", m_VirtualJoystick.GetAxis("Horizontal") != 0 || m_VirtualJoystick.GetAxis("Vertical") != 0);
                
        
    }

    void FixedUpdate()
    {
        joystickInput.x = m_VirtualJoystick.GetAxis("Horizontal");
        joystickInput.y = m_VirtualJoystick.GetAxis("Vertical");
        joystickInput = joystickInput.magnitude > .75f ? joystickInput.normalized : joystickInput;
        //Move(joystickInput);          

        // changeMobile = joystickInput;
        // changeMobile.x = Input.GetAxisRaw("Horizontal");
        // changeMobile.y = Input.GetAxisRaw("Vertical");

        if (_player == null)
        {
            setPlayer();
        }


        if (joystickInput != Vector3.zero)
        {
            Move(joystickInput);
        }
    }


    private void Move(Vector3 direction)
    {
        _player.myRigidbody.MovePosition(_player.transform.position + direction * (_player.speed + 1.5f) * Time.deltaTime);
    }


    public void OnRunButtonPressed()
    {
        _player.HandleRunButtonPressed();
    }


    public void OnRunButtonReleased()
    {
        _player.HandleRunButtonReleased();
    }

    private void setPlayer()
    {

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        _player = playerObject.GetComponent<Player>();

    }


}

