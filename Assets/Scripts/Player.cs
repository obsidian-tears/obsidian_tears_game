using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Opsive.UltimateInventorySystem.Core;

public enum PlayerType { Fighter, Wizard, Rogue, Barbarian }
public class Player : MonoBehaviour
{

    [HideInInspector] [SerializeField] public float speed = 3f;
    Rigidbody2D myRigidbody;
    public Vector3 change;
    public Animator animator;
    public FloatValue maxHealth;
    public FloatValue currentHealth;
    public FloatValue maxMagic;
    public FloatValue currentMagic;
    [Space(5)]
    [Header("Global game context reference")]
    [SerializeField] GlobalGameContextSORS m_globalGameContext;
    [Space(5)]
    [Header("Signals")]
    [SerializeField] MySignal playerHealthSignal;
    [SerializeField] MySignal battleSignal;
    [SerializeField] MySignal dialogSignal;
    /*Note from Isaac: I've removed the playerPosition object from this script and all its references (I just commented it out) to replace its use with the save system I've implemented.
     * The player instead has a PositionSaver component that saves the player's position on scene changes and when the game is actually saved/exited. This avoids having to constantly
     * change player position in Update calls*/
    //[SerializeField] VectorValue playerPosition;
    [SerializeField] GameObject pauseMenu;
    //[SerializeField] PlayerType playerType = PlayerType.Fighter;
    [SerializeField] bool frozen;

    void Awake() {
        if (m_globalGameContext != null)
        {
            m_globalGameContext.RegisterPlayerObject(this);
        }
        else 
        {
            Debug.LogError("Cannot find attached global game context!", gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Register player as the inventory panel owner
        InventorySystemManager.GetDisplayPanelManager().SetPanelOwner(gameObject);

        frozen = false;
        myRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        //transform.position = new Vector3(playerPosition.initialValue.x, playerPosition.initialValue.y, transform.position.z);
        playerHealthSignal.Raise();
    }

    // Update is called once per frame
    void Update()
    {
        if (frozen) return;
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");
        change = change.normalized;
        //playerPosition.initialValue.x = transform.position.x;
        //playerPosition.initialValue.y = transform.position.y;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (pauseMenu.activeInHierarchy)
            {
                pauseMenu.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                Time.timeScale = 0;
                pauseMenu.SetActive(true);
            }
        }

    }

    void FixedUpdate()
    {
        if (frozen) {
            animator.SetBool("moving", false);
            return;
        }
        if (change != Vector3.zero)
        {
            myRigidbody.MovePosition(transform.position + change * speed * Time.deltaTime);
            animator.SetFloat("moveX", change.x);
            animator.SetFloat("moveY", change.y);
            animator.SetBool("moving", true);
        }
        else
        {
            animator.SetBool("moving", false);
        }
    }

    public void OnMonsterSignal()
    {
        if (animator.GetBool("moving"))
        {
            frozen = true;
            // check for potion effects, magic armor, etc. 
            battleSignal.Raise();
        }
    }

    public void OnDialog() {
        if (!frozen) {
            dialogSignal.Raise();
        }
    }

    public void Freeze() {
        frozen = true;
    }
    public void Unfreeze() {
        frozen = false;
    }
}
