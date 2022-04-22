// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine.Demo
{

    /// <summary>
    /// Simple 2D player controller. Can interact with Targetable GameObjects.
    /// This script is fairly specific to the demo scene. However, it does
    /// demonstrate how to use the Message System to listen for Pause Player
    /// and Unpause Player messages.
    /// </summary>
    public class PlayerController2D : MonoBehaviour, IMessageHandler
    {

        public string attackButton = "Fire1";
        public string interactButton = "Fire2";
        public string horizontalAxis = "Horizontal";
        public string verticalAxis = "Vertical";

        [Tooltip("The fastest the player can travel left and right.")]
        public float maxHorizontalSpeed = 8f;

        [Tooltip("The fastest the player can travel up and down.")]
        public float maxVerticalSpeed = 5f;

        [Tooltip("Tracks which direction the player is facing.")]
        public bool facingLeft = false;

        [Tooltip("Hide this GameObject when first moving.")]
        public GameObject hideOnMove;

        public AudioClip attackSound;

        private Rigidbody2D m_rigidbody2D;
        private Animator m_animator;
        private SortByY m_sortByY;
        private bool needToHide = true;
        public List<Targetable> m_targets = new List<Targetable>();

        public const string RunParameter = "Run";
        private const string AttackParameter = "Attack";

        private void Awake()
        {
            m_rigidbody2D = GetComponent<Rigidbody2D>();
            m_animator = GetComponent<Animator>();
            m_sortByY = GetComponent<SortByY>();
            if (m_rigidbody2D == null) Debug.LogError("No Rigidbody2D found on " + name, this);
            if (m_animator == null) Debug.LogError("No Animator found on " + name, this);
            if (m_sortByY == null) m_sortByY = gameObject.AddComponent<SortByY>();
            MessageSystem.AddListener(this, "Pause Player", string.Empty);
            MessageSystem.AddListener(this, "Unpause Player", string.Empty);
        }

        private void OnDestroy()
        {
            MessageSystem.RemoveListener(this, "Pause Player", string.Empty);
            MessageSystem.RemoveListener(this, "Unpause Player", string.Empty);
        }

        public void OnMessage(MessageArgs messageArgs)
        {
            switch (messageArgs.message)
            {
                case "Pause Player":
                    enabled = false;
                    m_rigidbody2D.velocity = Vector2.zero;
                    m_animator.SetBool(RunParameter, false);
                    break;
                case "Unpause Player":
                    enabled = true;
                    break;
            }
        }

        private void Update()
        {
            if (needToHide && (Mathf.Abs(InputDeviceManager.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(InputDeviceManager.GetAxis("Vertical")) > 0.1f))
            {
                needToHide = false;
                if (hideOnMove != null) hideOnMove.SetActive(false);
            }
            if ((InputDeviceManager.DefaultGetMouseButtonDown(0) || InputDeviceManager.DefaultGetMouseButtonDown(1)) && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
            if (InputDeviceManager.IsButtonDown(attackButton))
            {

                StartCoroutine(Attack());
            }
            else if (InputDeviceManager.IsButtonDown(interactButton))
            {
                Interact();
            }
        }

        private void FixedUpdate()
        {
            // Move the character:
            var move = new Vector2(InputDeviceManager.GetAxis(horizontalAxis) * maxHorizontalSpeed, InputDeviceManager.GetAxis(verticalAxis) * maxVerticalSpeed);
            m_rigidbody2D.velocity = move;

            // Update the animator:
            m_animator.SetBool(RunParameter, move.magnitude > 0.1f);

            // Flip the character if necessary:
            var needToFlip = ((move.x < 0 && !facingLeft) || (move.x > 0 && facingLeft));
            if (needToFlip)
            {
                facingLeft = !facingLeft;
                Vector3 scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
            }
        }

#if USE_PHYSICS2D || !UNITY_2018_1_OR_NEWER

        private void OnTriggerEnter2D(Collider2D other)
        {
            var newTarget = other.GetComponent<Targetable>();
            if (newTarget == null) return;
            m_targets.Add(newTarget);
            newTarget.Target();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var oldTarget = other.GetComponent<Targetable>();
            if (oldTarget == null) return;
            m_targets.Remove(oldTarget);
            oldTarget.Untarget();
        }

#endif

        private void CleanTargetList()
        {
            m_targets.RemoveAll(t => (t == null || !t.gameObject.activeInHierarchy));
        }

        private IEnumerator Attack()
        {
            AudioSource.PlayClipAtPoint(attackSound, Camera.main.transform.position);
            CleanTargetList();
            m_animator.SetTrigger(AttackParameter);
            yield return new WaitForSeconds(0.4f);
            for (int i = 0; i < m_targets.Count; i++)
            {
                m_targets[i].Attack();
            }
        }

        private void Interact()
        {
            CleanTargetList();
            var inventory = FindObjectOfType<DemoInventory>();
            for (int i = 0; i < m_targets.Count; i++)
            {
                if (inventory.usingIndex != -1)
                {
                    MessageSystem.SendMessageWithTarget(this, m_targets[i], "Use", "Wand");
                }
                else
                {
                    m_targets[i].Interact();
                }
            }
        }

    }
}