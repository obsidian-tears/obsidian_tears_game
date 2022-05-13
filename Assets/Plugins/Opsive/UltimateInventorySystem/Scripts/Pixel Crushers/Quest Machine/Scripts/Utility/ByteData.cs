// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Serializable array of bytes.
    /// </summary>
    [Serializable]
    public class ByteData
    {
        public byte[] bytes;

        public ByteData() { }

        public ByteData(byte[] bytes)
        {
            this.bytes = bytes;
        }
    }

}