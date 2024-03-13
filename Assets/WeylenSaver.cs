using System;
using UnityEngine;
namespace PixelCrushers
{
    public class WeylenSaver : Saver
    {
        public bool didWeylenMove;

        public override string RecordData()
        {
            return didWeylenMove ? "true" : "false";
        }

        public override void ApplyData(string s)
        {
            if (string.IsNullOrEmpty(s)) return;
            if (s == "true") didWeylenMove = true;
            else if (s == "false") didWeylenMove = false;
            else Debug.LogError("WeylenSaver: PIXELCRUSHERS DID THE STUPID AGAIN!!!");
        }
    }
}