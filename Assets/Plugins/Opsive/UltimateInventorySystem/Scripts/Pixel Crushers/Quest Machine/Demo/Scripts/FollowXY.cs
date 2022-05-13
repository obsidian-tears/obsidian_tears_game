// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine.Demo
{

    /// <summary>
    /// Makes the GameObject follow another GameObject on the XY plane.
    /// </summary>
    public class FollowXY : MonoBehaviour
    {

        public Transform followTarget;

        private void Update()
        {
            transform.position = new Vector3(followTarget.position.x, followTarget.position.y, transform.position.z);
        }

    }

}