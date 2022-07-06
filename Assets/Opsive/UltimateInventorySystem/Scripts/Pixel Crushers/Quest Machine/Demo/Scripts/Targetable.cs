// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.Events;

namespace PixelCrushers.QuestMachine.Demo
{

    /// <summary>
    /// Add to a GameObject to make it targetable by PlayerController2D. It can invoke events
    /// when targeted/untargeted, attacked, interacted, or when an item is applied to it.
    /// This script is fairly specific to the demo scene, although you may be able to 
    /// adapt it for other uses, too.
    /// </summary>
    public class Targetable : MonoBehaviour
    {

        public GameObject[] deathPrefabs;
        public GameObject polymorphInto;
        public AudioClip polymorphAudioClip;

        public UnityEvent onTarget = new UnityEvent();
        public UnityEvent onUntarget = new UnityEvent();
        public UnityEvent onAttack = new UnityEvent();
        public UnityEvent onInteract = new UnityEvent();
        public IntUnityEvent onApply = new IntUnityEvent();

        public void Target()
        {
            onTarget.Invoke();
        }

        public void Untarget()
        {
            onUntarget.Invoke();
        }

        public void Attack()
        {
            onAttack.Invoke();
        }

        public void Die()
        {
            foreach (var prefab in deathPrefabs)
            {
                Instantiate(prefab, transform.position, transform.rotation);
            }
            Destroy(gameObject);
        }

        public void Polymorph()
        {
            if (!HaveWand()) return;
            var entity = GetComponent<QuestEntity>();
            MessageSystem.SendMessage(this, "Polymorph", ((entity != null) ? entity.entityType.name : name));
            AudioSource.PlayClipAtPoint(polymorphAudioClip, Camera.main.transform.position);
            Instantiate(polymorphInto, transform.position, transform.rotation);
            Die();
        }

        private bool HaveWand()
        {
            var demoInventory = FindObjectOfType<DemoInventory>();
            if (demoInventory == null) return false;
            return demoInventory.GetItemCount(DemoInventory.WandSlot) > 0;
        }

        public void PlayAudio(AudioClip audioClip)
        {
            AudioSource.PlayClipAtPoint(audioClip, Camera.main.transform.position);
        }

        public void Interact()
        {
            onInteract.Invoke();
        }

        public void Apply(int itemIndex)
        {
            onApply.Invoke(itemIndex);
        }
    }
}