using UnityEngine;
using System.Collections;

namespace PixelCrushers.LoveHate.Example
{

	/// <summary>
	/// This is a rudimentary 2D player controller for the example scene.
	/// </summary>
	public class RudimentaryPlayerController2D : Mover2D
	{

		public float speedFactor = 0.5f;

		public float uiUpdateFrequency = 0.5f;

		public NPC currentTarget { get; private set; }

		public InteractionUI interactionUI { get; private set; }

		private DeedReporter m_deedReporter = null;

		protected override void Start ()
		{
			base.Start();
			m_deedReporter = GetComponent<DeedReporter>();
			interactionUI = FindObjectOfType<InteractionUI>();
			StartCoroutine(UpdateUICoroutine());
		}

		protected IEnumerator UpdateUICoroutine()
		{
			while (true)
			{
				if (interactionUI != null && currentTarget != null)
				{
					interactionUI.npcSummaryText.text = currentTarget.GetSummaryText();
				}
				yield return new WaitForSeconds(uiUpdateFrequency);
			}
		}
		
		protected virtual void Update()
		{
			var x = transform.position.x + Input.GetAxis("Horizontal") * speedFactor;
			var y = transform.position.y + Input.GetAxis("Vertical") * speedFactor;
			moveToPosition = new Vector3(x, y, transform.position.z);
		}

#if USE_PHYSICS2D || !UNITY_2018_1_OR_NEWER

        protected virtual void OnCollisionEnter2D(Collision2D coll)
		{
			CheckTouchNPC(coll.gameObject.GetComponent<NPC>());
		}
		
		protected virtual void OnCollisionExit2D(Collision2D coll)
		{
			CheckUntouchNPC(coll.gameObject.GetComponent<NPC>());
		}

#endif

        protected void CheckTouchNPC(NPC npc)
		{
			if (npc == null || currentTarget != null) return;
			currentTarget = npc;
			interactionUI.SetInteractionPanel(true);
		}

		protected void CheckUntouchNPC(NPC npc)
		{
			if (npc != null && npc == currentTarget)
			{
				currentTarget = null;
				interactionUI.SetInteractionPanel(false);
			}
		}

		public virtual void CommitDeed(string tag)
		{
			if (currentTarget == null) return;
			m_deedReporter.ReportDeed(tag, currentTarget.factionMember);
		}
		
	}

}
