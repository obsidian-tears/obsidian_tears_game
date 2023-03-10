using System.Collections;
using System.Collections.Generic;
using Core;
using Opsive.UltimateInventorySystem.UI.Menus.Shop;
using Opsive.UltimateInventorySystem.UI.Panels;
using UnityEngine;

namespace GameManagers
{
    /// <summary>
    /// Use this class as a main access point to the game UI
    /// </summary>
    public class GameUIManager : MonoSingleton<GameUIManager>
    {
        public CustomShopMenu ShopMenu;
        protected override void Init()
        {
        }
    }
}
