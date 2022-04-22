using UnityEngine;
using System.Collections;

namespace PixelCrushers.LoveHate.Example
{

	/// <summary>
	/// This script moves 2D GameObjects. It's used in the example scene
	/// for the player and NPCs.
	/// </summary>
	public class Mover2D : MonoBehaviour
	{

		public bool controlMovement = true;

		public Vector3 moveToPosition;

        protected Rect cameraRect;

#if USE_PHYSICS2D || !UNITY_2018_1_OR_NEWER
        protected Rigidbody2D rb;

		protected virtual void Start()
		{
			moveToPosition = transform.position;
			rb = GetComponent<Rigidbody2D>();
			var bottomLeft = Camera.main.ScreenToWorldPoint(Vector3.zero);
			var topRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight));
			cameraRect = new Rect(bottomLeft.x, bottomLeft.y, topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);
		}
		
		protected virtual void FixedUpdate()
		{
			if (controlMovement)
			{
				var x = Mathf.Clamp(moveToPosition.x, cameraRect.xMin, cameraRect.xMax);
				var y = Mathf.Clamp(moveToPosition.y, cameraRect.yMin, cameraRect.yMax);
				rb.MovePosition(new Vector2(x, y));
			}
		}
#else
        protected virtual void Start() {}
#endif

    }

}
