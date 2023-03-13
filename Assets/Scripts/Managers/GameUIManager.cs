using System.Collections;
using System.Collections.Generic;
using Core;
using Opsive.UltimateInventorySystem.UI.Menus.Shop;
using Opsive.UltimateInventorySystem.UI.Panels;
using UnityEngine;
using UnityEngine.UI;

namespace GameManagers
{
    /// <summary>
    /// Use this class as a main access point to the game UI
    /// </summary>
    public class GameUIManager : MonoSingleton<GameUIManager>
    {
        [Header("Menus")]
        public CustomShopMenu ShopMenu;
        [Header("Graphic Elements")]
        public Image LoadingIndicator;
        public Image Blocker;

        protected override void Init()
        {
        }

        //TODO this definitely needs some refactor regarding its behaviour (see ReactController for context)
        public void ShowLoadingIndicator(bool show, bool involveBlocker)
        {
            if (LoadingIndicator != null)
            {
                LoadingIndicator.gameObject.SetActive(show);
            }

            if (involveBlocker && Blocker != null)
            {
                Blocker.gameObject.SetActive(show);
            }
        }
    }
}
