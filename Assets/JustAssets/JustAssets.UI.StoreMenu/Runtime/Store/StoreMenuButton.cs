using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace JustAssets.UI.StoreMenu.Store
{
    [Serializable]
    public class StoreMenuButton
    {
        public bool ActivationState = true;

        public Toggle Button;

        public StoreMenuCategory Category;

        public UnityAction<bool> EventCallback { get; set; }
    }
}