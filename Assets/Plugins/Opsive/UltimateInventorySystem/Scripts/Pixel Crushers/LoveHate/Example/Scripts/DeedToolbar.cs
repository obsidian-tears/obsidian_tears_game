/*
 * This script is no longer used by the example scenes. The functionality
 * has been moved to RudimentaryPlayerController2D.
 * 
using UnityEngine;

namespace PixelCrushers.LoveHate.Example
{

	/// <summary>
	/// This script ties the example scene's activity buttons to
	/// the DeedReporter.
	/// </summary>
	public class DeedToolbar : MonoBehaviour 
	{

		public FactionMember target { get; set; }

		private DeedReporter m_deedReporter;

		private void Awake()
		{
			target = null;
			m_deedReporter = GetComponent<DeedReporter>();
		}

		private void OnCollisionEnter2D(Collision2D coll)
		{
			target = coll.gameObject.GetComponent<FactionMember>();
		}
		
		private void OnCollisionExit2D(Collision2D coll)
		{
			target = null;
		}

		public void CommitDeed(string tag)
		{
			if (target == null) return;
			m_deedReporter.ReportDeed(tag, target);
		}

	}

}
*/