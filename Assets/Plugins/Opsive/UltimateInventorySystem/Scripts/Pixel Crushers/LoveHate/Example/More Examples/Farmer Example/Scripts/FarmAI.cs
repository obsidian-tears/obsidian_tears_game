using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers.LoveHate.Example
{

	/// <summary>
	/// Basic AI script for the Farmer Example scene.
	/// </summary>
	public class FarmAI : Mover2D
	{

		public float powerLevel = 1;

		public float selfPerceivedPowerLevel = 1;

		public float speed = 0.2f;

		public float pursueSpeed = 0.5f;

		public float evadeSpeed = 0.4f;

		public float endurance = 10;

		public float dislikeThreshold = -10;

		public bool aggressive = false;

		private FactionMember m_member;

        private DeedReporter m_deedReporter;

        public List<FactionMember> m_inRange = new List<FactionMember>();

		private enum State { Wander, Pursue, Evade }

		private State m_state = State.Wander;

		private Transform m_target = null;

		private Vector3 m_destination;

		private float m_giveUpTime;
		
		private UnityEngine.UI.Text m_text = null;

		// Start() does basic setup.
		protected override void Start()
		{
			base.Start();
			m_deedReporter = GetComponent<DeedReporter>();
            if (m_deedReporter != null) m_deedReporter.enabled = true; // Dummy line to quiet Unity 2018+ if USE_PHYSICS2D isn't set yet.
            m_member = GetComponent<FactionMember>();
			m_member.GetPowerLevel = GetPowerLevel;
			m_member.GetSelfPerceivedPowerLevel = GetSelfPerceivedPowerLevel;
			m_destination = GetRandomDestination();
			m_text = GetComponentInChildren<UnityEngine.UI.Text>();
			ChangeState(State.Wander, null);
		}

		private float GetPowerLevel()
		{
			return powerLevel;
		}

		private float GetSelfPerceivedPowerLevel()
		{
			return selfPerceivedPowerLevel;
		}

#if USE_PHYSICS2D || !UNITY_2018_1_OR_NEWER

        private void OnTriggerEnter2D(Collider2D collider)
		{
			var otherMember = collider.GetComponentInChildren<FactionMember>();
			if (otherMember == null || m_inRange.Contains(otherMember)) return;
			if (m_target == null)
			{
				HandleTarget(otherMember);
			}
			else
			{
				m_inRange.Add(otherMember);
			}
		}

		private void OnTriggerExit2D(Collider2D collider)
		{
			var otherMember = collider.GetComponentInChildren<FactionMember>();
			if (otherMember == null) return;
			m_inRange.Remove(otherMember);
			if (m_target == otherMember)
			{
				CheckForNewTarget();
			}
		}

        private void OnCollisionEnter2D(Collision2D coll)
        {
            var otherMember = coll.collider.GetComponentInChildren<FactionMember>();
            if (otherMember == null || !aggressive) return;
            if (m_member.GetAffinity(otherMember) < 0)
            {
                if (m_deedReporter != null)
                {
                    m_deedReporter.ReportDeed("attack", otherMember);
                }
                if (!string.Equals(otherMember.faction.name, "Farmer"))
                {
                    Destroy(otherMember.gameObject);
                    HandleTarget(null);
                    m_inRange.RemoveAll(x => x == null);
                    CheckForNewTarget();
                }
            }
        }

#endif

        private void HandleTarget(FactionMember newTarget)
		{
			m_target = (newTarget == null) ? null : newTarget.transform;
			if (m_target != null && m_member.GetAffinity(newTarget) <= dislikeThreshold)
			{
				ChangeState(aggressive ? State.Pursue : State.Evade, newTarget.transform);
			}
			else
			{
				ChangeState(State.Wander, null);
			}
		}
		
		private void CheckForNewTarget()
		{
			if (m_inRange.Count > 0)
			{
				var newTarget = m_inRange[0];
				m_inRange.Remove(newTarget);
				HandleTarget(newTarget);
			}
			else
			{
				HandleTarget(null);
			}
		}

		private void ChangeState(State newState, Transform newTarget)
		{
			m_target = newTarget;
			m_state = newState;
			m_giveUpTime = GameTime.time + endurance;
			if (m_text != null) m_text.text = GetStateText(newState);
		}

		private string GetStateText(State state)
		{
			switch (state)
			{
			default:
			case State.Wander: 
				return "<color=lightgray>Wander</color>";
			case State.Pursue:
				return "<color=red>Pursue</color>";
			case State.Evade:
				return "<color=blue>Evade</color>";
			}
		}

		private void Update()
		{
			if (m_state != State.Wander && ((m_target == null) || (GameTime.time > m_giveUpTime)))
			{
				ChangeState(State.Wander, null);
			}
			switch (m_state)
			{
			case State.Wander:
				if (Vector3.Distance(transform.position, m_destination) < 1)
				{
					m_destination = GetRandomDestination();
				}
				MoveTowards(m_destination, speed);
				break;
			case State.Pursue:
				MoveTowards(m_target.position, 2 * speed);
				break;
			case State.Evade:
				MoveTowards(m_target.position, -speed);
				break;
			}
		}

		private Vector3 GetRandomDestination()
		{
			var x = Random.Range(cameraRect.xMin, cameraRect.xMax);
			var y = Random.Range(cameraRect.yMin, cameraRect.yMax);
			return new Vector3(x, y, transform.position.z);
		}

		private void MoveTowards(Vector3 destination, float speed)
		{
			moveToPosition = Vector2.MoveTowards(
				new Vector2(transform.position.x, transform.position.y),
				destination, speed * GameTime.deltaTime);
		}

	}

}
