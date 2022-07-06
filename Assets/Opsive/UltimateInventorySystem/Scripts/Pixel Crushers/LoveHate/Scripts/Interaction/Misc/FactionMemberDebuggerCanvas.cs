// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// Canvas used by FactionMemberDebugger.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class FactionMemberDebuggerCanvas : MonoBehaviour
    {

        public UnityEngine.UI.Text padText;
        public UnityEngine.UI.Text memoryCountText;
        public UnityEngine.UI.Text lastMemoryText;

        [Tooltip("If ticked, add only if this is a debug build.")]
        public bool onlyInDebugBuild = false;

    }
}