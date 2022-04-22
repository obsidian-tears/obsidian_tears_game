using UnityEngine;
using System.Collections;
using System.Text;

namespace PixelCrushers.LoveHate.Example
{

	/// <summary>
	/// Basic NPC script for the example scene. Note that it uses the event system
	/// to handle IWitnessDeedEventHandler's OnWitnessDeed() method.
	/// </summary>
	public class NPC : Mover2D, IWitnessDeedEventHandler
	{

		// The NPC's power level, which is used in the rumor evaluation function
		// to determine power level relative to the actor of the deed. If the 
		// actor is more powerful than this NPC, the NPC might feel cowed or
		// submissive. If the actor is less powerful, the NPC might feel angry
		// or disdainful if the actor does something the NPC doesn't like.
		public float powerLevel = 1;
		public float selfPerceivedPowerLevel = 1;

		private Animator m_animator;

		public FactionMember factionMember { get; private set; }

		private const float UpdateFrequency = 0.5f;

		private Vector3 m_startPosition;

		// Start() does basic setup and also registers custom power level
		// delegates and starts wandering.
		protected override void Start()
		{
			base.Start();
			m_startPosition = transform.position;
			m_animator = GetComponent<Animator>();
			factionMember = GetComponent<FactionMember>();
			if (factionMember != null)
			{
				factionMember.GetPowerLevel = GetPowerLevel;
				factionMember.GetSelfPerceivedPowerLevel = GetSelfPerceivedPowerLevel;
			}
			Wander();
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

        private void OnCollisionEnter2D(Collision2D coll)
		{
			if (coll.gameObject.CompareTag("Player"))
			{
				Idle();
			}
		}

        private void OnCollisionExit2D(Collision2D coll)
		{
			if (coll.gameObject.CompareTag("Player"))
			{
				Wander();
			}
		}

#endif

        public void Idle()
		{
			StopAllCoroutines();
		}

		public void Wander()
		{
			StopAllCoroutines();
			StartCoroutine(WanderCoroutine());
		}
		
		private IEnumerator WanderCoroutine()
		{
			const float MaxIdle = 5;
			const float MaxRange = 3;
			const float Speed = 0.2f;
			while (true)
			{
				yield return new WaitForSeconds(Random.Range(1, MaxIdle));
				var x = Mathf.Clamp(m_startPosition.x + Random.Range(-MaxRange, MaxRange), cameraRect.xMin, cameraRect.xMax);
				var y = Mathf.Clamp(m_startPosition.y + Random.Range(-MaxRange, MaxRange), cameraRect.yMin, cameraRect.yMax);
				var destination = new Vector3(x, y, m_startPosition.z);
				while (Vector3.Distance(transform.position, destination) > 1)
				{
					yield return null;
					moveToPosition = new Vector2(Mathf.Lerp(transform.position.x, destination.x, Speed * GameTime.deltaTime),
					                             Mathf.Lerp(transform.position.y, destination.y, Speed * GameTime.deltaTime));
				}
			}
		}

		// When the NPC witnesses a deed, it generates a rumor that records
		// how it feels about the deed. The rumor has, among other values,
		// a pleasure value. The method below checks whether the NPC found
		// the deed pleasing or displeasing, and then plays an appropriate
		// animation.
		public void OnWitnessDeed(Rumor rumor)
		{
			if (factionMember == null || rumor == null) return;
			if (rumor.pleasure < -0.25)
			{
				m_animator.SetTrigger("Sad");
			} 
			else if (rumor.pleasure > 0.25)
			{
				m_animator.SetTrigger("Happy");
			}
		}
		
		public string GetSummaryText()
		{
			if (factionMember == null) return string.Empty;
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("NPC: {0}\n", name);
			sb.AppendFormat("Faction: {0}\n", factionMember.faction.name);
			sb.AppendFormat("Description: {0}\n", factionMember.faction.description);
			sb.Append("\nParents:\n");
			for (int p = 0; p < factionMember.faction.parents.Length; p++)
			{
				var parentID = factionMember.faction.parents[p];
				sb.AppendFormat("\t{0}\n", factionMember.factionManager.GetFaction(parentID).name);
			}
			
			sb.Append("\nPAD:\n");
			sb.AppendFormat("\tPleasure: {0}\n", factionMember.pad.pleasure);
			sb.AppendFormat("\tArousal: {0}\n" , factionMember.pad.arousal);
			sb.AppendFormat("\tDominance: {0}\n", factionMember.pad.dominance);
			sb.AppendFormat("\tHappiness: {0}\n", factionMember.pad.happiness);
			sb.AppendFormat("\tTemperament: {0}\n", factionMember.pad.GetTemperament());
			
			sb.Append("\nTraits:\n");
			for (int i = 0; i < factionMember.factionManager.factionDatabase.personalityTraitDefinitions.Length; i++)
			{
				sb.AppendFormat("\t{0}: {1}\n", factionMember.factionManager.factionDatabase.personalityTraitDefinitions[i].name, factionMember.faction.traits[i]);
			}
			
			sb.Append("\nRelationships:\n");
			for (int r = 0; r < factionMember.faction.relationships.Count; r++)
			{
				var relationship = factionMember.faction.relationships[r];
				sb.AppendFormat("\t{0}: {1}\n", factionMember.factionManager.GetFaction(relationship.factionID).name, relationship.affinity);
			}
			
			sb.Append("\nMemories:\n");
			for (int m = 0; m < factionMember.longTermMemory.Count; m++)
			{
				var rumor = factionMember.longTermMemory[m];
				sb.AppendFormat("\t{0} {1} {2}: impact {3}", factionMember.factionManager.GetFaction(rumor.actorFactionID).name,
				                rumor.tag, factionMember.factionManager.GetFaction(rumor.targetFactionID).name, rumor.impact);
				if (rumor.count > 1)
				{
					sb.AppendFormat(" (x{0})", rumor.count);
				}
				sb.Append("\n");
			}
			return sb.ToString();
		}
			
	}

}
