using System;
using JustAssets.Shared.Providers;
using UnityEngine.Events;
using UnityEngine.UI;

namespace JustAssets.UI.StoreMenu.Store
{
    [Serializable]
    internal class StoreCategoryButton
    {
        public Toggle Button;

        public ItemCategory Category;

        public UnityAction<bool> EventCallback { get; set; }
    }
}