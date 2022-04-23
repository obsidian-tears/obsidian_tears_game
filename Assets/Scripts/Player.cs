using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerType
{
    Fighter,
    Wizard,
    Rogue,
    Barbarian
}

public class Player : MonoBehaviour
{
    [SerializeField]
    float speed = 3f;

    Rigidbody2D myRigidbody;

    Vector3 change;

    public Animator animator;

    /* 
    TODO: Move to Player Stats
    */
    public FloatValue maxHealth;

    public FloatValue maxMagic;

    public FloatValue currentHealth;

    public FloatValue currentMagic;

    /* 
    End TODO 
    */
    [SerializeField]
    MySignal playerHealthSignal;

    [SerializeField]
    MySignal battleSignal;

    [SerializeField]
    VectorValue playerPosition;

    /*
    TODO: move this to a menu/dialog controller
    */
    [SerializeField]
    GameObject pauseMenu;

    /*
    End TODO 
    */
    [SerializeField]
    MySignal dialogSignal;

    [SerializeField]
    BoolValue frozen;

    // Start is called before the first frame update
    void Start()
    {
        frozen.value = false;
        myRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        transform.position =
            new Vector3(playerPosition.initialValue.x,
                playerPosition.initialValue.y,
                transform.position.z);
        playerHealthSignal.Raise();
    }

    // Update is called once per frame
    void Update()
    {
        if (frozen.value) return;
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");
        change = change.normalized;
        playerPosition.initialValue.x = transform.position.x;
        playerPosition.initialValue.y = transform.position.y;

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
        if (frozen.value)
        {
            animator.SetBool("moving", false);
            return;
        }
        if (change != Vector3.zero)
        {
            myRigidbody
                .MovePosition(transform.position +
                change * speed * Time.deltaTime);
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
            frozen.value = true;

            // check for potion effects, magic armor, etc.
            battleSignal.Raise();
        }
    }

    public void OnDialog()
    {
        if (!frozen.value)
        {
            dialogSignal.Raise();
        }
    }

    public void Freeze()
    {
        frozen.value = true;
    }

    public void Unfreeze()
    {
        frozen.value = false;
    }
}
