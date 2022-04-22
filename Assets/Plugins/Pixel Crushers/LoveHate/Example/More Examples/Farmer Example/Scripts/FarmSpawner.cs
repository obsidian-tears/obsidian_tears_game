using UnityEngine;
using System.Collections;

namespace PixelCrushers.LoveHate.Example
{

	/// <summary>
	/// This script provides on click methods for spawning rabbits and wolves.
	/// </summary>
	public class FarmSpawner : MonoBehaviour {

		public GameObject rabbitPrefab;
		
		public GameObject wolfPrefab;

		public void SpawnRabbit()
		{
			Spawn(rabbitPrefab);
		}

		public void SpawnWolf()
		{
			Spawn(wolfPrefab);
		}

		private void Spawn(GameObject prefab)
		{
			if (prefab == null) return;
			var instance = Instantiate(prefab) as GameObject;
			instance.transform.position = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), -1);
		}

	}

}
