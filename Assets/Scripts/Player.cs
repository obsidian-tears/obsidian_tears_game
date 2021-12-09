using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum PlayerType {Fighter, Wizard, Rogue, Barbarian}
public class Player : MonoBehaviour
{

    [SerializeField] float speed = 3f;
    Rigidbody2D myRigidbody;
    Vector3 change;
    public Animator animator;
    public FloatValue maxHealth;
    public FloatValue currentHealth;
    public FloatValue maxMagic;
    public FloatValue currentMagic;
    [SerializeField] MySignal playerHealthSignal;
    [SerializeField] MySignal battleSignal;
    [SerializeField] VectorValue playerPosition;
    [SerializeField] PlayerType playerType = PlayerType.Fighter;
    

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        transform.position = new Vector3(playerPosition.initialValue.x, playerPosition.initialValue.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");
        change = change.normalized;
        playerPosition.initialValue.x = transform.position.x;
        playerPosition.initialValue.y = transform.position.y;
    }

    void FixedUpdate()
    {
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

    public void OnMonsterSignal() {
        if (animator.GetBool("moving")) {
            // check for potion effects, magic armor, etc. 
            battleSignal.Raise();
        }
    }
}
