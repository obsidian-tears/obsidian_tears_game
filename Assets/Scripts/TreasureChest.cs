using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : Interactable
{
    Animator animator;
    [SerializeField] InventoryItem item;
    [SerializeField] PlayerInventory playerInventory;
    [SerializeField] Stats playerStats;
    [SerializeField] int gold;
    [SerializeField] bool isOpen;
    void Start()
    {
        active = true;
        animator = GetComponent<Animator>();
        isOpen = false;
    }

    void Update() {
        if (active && playerInRange) {
            if (Input.GetKeyDown(KeyCode.Space) && !isOpen) {
                OpenChest();
            }
        }
    }

    public void OpenChest()
    {
        active = false;
        if (item != null) {
            playerInventory.AddItem(item);
        }
        if (gold > 0) {
            playerStats.gold.value += gold;
        }
        animator.SetBool("open", true);
    }
}
