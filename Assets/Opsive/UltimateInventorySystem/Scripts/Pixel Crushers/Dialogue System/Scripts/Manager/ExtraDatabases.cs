﻿// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// Adds and removes extra dialogue databases.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class ExtraDatabases : MonoBehaviour
    {

        /// <summary>
        /// Add the databases when this trigger occurs.
        /// </summary>
        public DialogueTriggerEvent addTrigger = DialogueTriggerEvent.OnStart;

        /// <summary>
        /// Remove the databases when this trigger occurs.
        /// </summary>
        public DialogueTriggerEvent removeTrigger = DialogueTriggerEvent.None;

        /// <summary>
        /// The databases to add/remove.
        /// </summary>
        public DialogueDatabase[] databases = new DialogueDatabase[0];

        /// <summary>
        /// The condition that must be true for the trigger to fire.
        /// </summary>
        public Condition condition = new Condition();

        /// <summary>
        /// As soon as one event (add or remove) has occurred, destroy this component.
        /// </summary>
        [Tooltip("As soon as one event (add or remove) has occurred, destroy this component.")]
        public bool once = false;

        /// <summary>
        /// Add/remove one database per frame instead of adding them all at the same time.
        /// Useful to avoid stutter when adding several databases.
        /// </summary>
        [Tooltip("Add/remove one database per frame instead of adding them all at the same time. Useful to avoid stutter when adding several databases.")]
        public bool onePerFrame = false;

        /// <summary>
        /// This event is called after ExtraDatabases has finished adding its list of databases
        /// to the DialogueManager's MasterDatabase.
        /// </summary>
        public static event System.Action addedDatabases = delegate { };

        /// <summary>
        /// This event is called after ExtraDatabases has finished removing its list of databases
        /// from the DialogueManager's MasterDatabase.
        /// </summary>
        public static event System.Action removedDatabases = delegate { };

        private bool m_trying = false;
        private Coroutine m_destroyCoroutine = null;
        private int m_numActiveCoroutines = 0;

        private void TryAddDatabases(Transform interactor, bool onePerFrame)
        {
            if (!m_trying)
            {
                m_trying = true;
                try
                {
                    if ((condition == null) || condition.IsTrue(interactor))
                    {
                        AddDatabases(onePerFrame);
                    }
                }
                finally
                {
                    m_trying = false;
                }
            }
        }

        public void AddDatabases(bool onePerFrame)
        {
            if (onePerFrame)
            {
                StartCoroutine(AddDatabasesCoroutine());
            }
            else if (gameObject.activeInHierarchy && enabled)
            {
                AddDatabasesImmediate();
            }
        }

        private void AddDatabasesImmediate()
        {
            foreach (var database in databases)
            {
                AddDatabase(database);
            }
            addedDatabases();
            if (once) Destroy(this);
        }

        private IEnumerator AddDatabasesCoroutine()
        {
            m_numActiveCoroutines++;
            if (once && m_destroyCoroutine == null) m_destroyCoroutine = StartCoroutine(DestroyCoroutine());
            foreach (var database in databases)
            {
                AddDatabase(database);
                yield return null;
            }
            addedDatabases();
            m_numActiveCoroutines--;
        }

        private void AddDatabase(DialogueDatabase database)
        {
            if (database != null)
            {
                if (DialogueDebug.logInfo) Debug.Log("Dialogue System: Adding database " + database.name, this);
                DialogueManager.AddDatabase(database);
            }
        }

        private void TryRemoveDatabases(Transform interactor, bool onePerFrame)
        {
            if (!m_trying)
            {
                m_trying = true;
                try
                {
                    if ((condition == null) || condition.IsTrue(interactor))
                    {
                        RemoveDatabases(onePerFrame);
                    }
                }
                finally
                {
                    m_trying = false;
                }
            }
        }

        public void RemoveDatabases(bool onePerFrame)
        {
            if (onePerFrame)
            {
                StartCoroutine(RemoveDatabasesCoroutine());
            }
            else if (gameObject.activeInHierarchy && enabled)
            {
                RemoveDatabasesImmediate();
            }
        }

        private void RemoveDatabasesImmediate()
        {
            foreach (var database in databases)
            {
                RemoveDatabase(database);
            }
            removedDatabases();
            if (once) Destroy(this);
        }

        private IEnumerator RemoveDatabasesCoroutine()
        {
            m_numActiveCoroutines++;
            if (once && m_destroyCoroutine == null) m_destroyCoroutine = StartCoroutine(DestroyCoroutine());
            foreach (var database in databases)
            {
                RemoveDatabase(database);
                yield return null;
            }
            removedDatabases();
            m_numActiveCoroutines--;
        }

        private void RemoveDatabase(DialogueDatabase database)
        {
            if (database != null)
            {
                if (DialogueDebug.logInfo) Debug.Log("Dialogue System: Removing database " + database.name, this);
                DialogueManager.RemoveDatabase(database);
            }
        }

        private IEnumerator DestroyCoroutine()
        {
            // If Once is ticked and component both adds & removes databases, we need to wait for both to finish:
            while (m_numActiveCoroutines > 0)
            {
                yield return null;
            }
            m_destroyCoroutine = null;
            Destroy(this);
        }

        public void Start()
        {
            if (addTrigger == DialogueTriggerEvent.OnStart || removeTrigger == DialogueTriggerEvent.OnStart)
            {
                StartCoroutine(StartEndOfFrame());
            }
        }

        private IEnumerator StartEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            if (addTrigger == DialogueTriggerEvent.OnStart) TryAddDatabases(null, onePerFrame);
            if (removeTrigger == DialogueTriggerEvent.OnStart) TryRemoveDatabases(null, onePerFrame);
        }

        public void OnEnable()
        {
            if (addTrigger == DialogueTriggerEvent.OnEnable) TryAddDatabases(null, onePerFrame);
            if (removeTrigger == DialogueTriggerEvent.OnEnable) TryRemoveDatabases(null, onePerFrame);
        }

        public void OnDisable()
        {
            if (addTrigger == DialogueTriggerEvent.OnDisable) TryAddDatabases(null, false); // Can't run coroutine when disabled.
            if (removeTrigger == DialogueTriggerEvent.OnDisable) TryRemoveDatabases(null, false);
        }

        public void OnDestroy()
        {
            if (addTrigger == DialogueTriggerEvent.OnDestroy) TryAddDatabases(null, false); // Can't run coroutine when destroyed.
            if (removeTrigger == DialogueTriggerEvent.OnDestroy) TryRemoveDatabases(null, false);
        }

        public void OnUse(Transform actor)
        {
            if (!enabled) return;
            if (addTrigger == DialogueTriggerEvent.OnUse) TryAddDatabases(actor, onePerFrame);
            if (removeTrigger == DialogueTriggerEvent.OnUse) TryRemoveDatabases(actor, onePerFrame);
        }

        public void OnUse(string message)
        {
            if (!enabled) return;
            if (addTrigger == DialogueTriggerEvent.OnUse) TryAddDatabases(null, onePerFrame);
            if (removeTrigger == DialogueTriggerEvent.OnUse) TryRemoveDatabases(null, onePerFrame);
        }

        public void OnUse()
        {
            if (!enabled) return;
            if (addTrigger == DialogueTriggerEvent.OnUse) TryAddDatabases(null, onePerFrame);
            if (removeTrigger == DialogueTriggerEvent.OnUse) TryRemoveDatabases(null, onePerFrame);
        }

        public void OnTriggerEnter(Collider other)
        {
            if (!enabled) return;
            if (addTrigger == DialogueTriggerEvent.OnTriggerEnter) TryAddDatabases(other.transform, onePerFrame);
            if (removeTrigger == DialogueTriggerEvent.OnTriggerEnter) TryRemoveDatabases(other.transform, onePerFrame);
        }

        public void OnTriggerExit(Collider other)
        {
            if (!enabled) return;
            if (addTrigger == DialogueTriggerEvent.OnTriggerExit) TryAddDatabases(other.transform, onePerFrame);
            if (removeTrigger == DialogueTriggerEvent.OnTriggerExit) TryRemoveDatabases(other.transform, onePerFrame);
        }

#if USE_PHYSICS2D || !UNITY_2018_1_OR_NEWER

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (!enabled) return;
            if (addTrigger == DialogueTriggerEvent.OnTriggerEnter) TryAddDatabases(other.transform, onePerFrame);
            if (removeTrigger == DialogueTriggerEvent.OnTriggerEnter) TryRemoveDatabases(other.transform, onePerFrame);
        }

        public void OnTriggerExit2D(Collider2D other)
        {
            if (!enabled) return;
            if (addTrigger == DialogueTriggerEvent.OnTriggerExit) TryAddDatabases(other.transform, onePerFrame);
            if (removeTrigger == DialogueTriggerEvent.OnTriggerExit) TryRemoveDatabases(other.transform, onePerFrame);
        }

#endif

    }

}