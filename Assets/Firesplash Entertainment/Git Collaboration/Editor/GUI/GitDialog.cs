using System;

namespace Firesplash.GameDevAssets.GitCollab.GUI
{
    internal struct GitDialog
    {
        public string title;
        public string text;
        public string positiveButton;
        public string negativeButton;
        public Action onPositive;
        public Action onNegative;
    }
}
