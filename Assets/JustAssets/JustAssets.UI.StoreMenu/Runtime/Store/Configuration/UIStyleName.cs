using System;
using JustAssets.Shared.Providers;

namespace JustAssets.UI.StoreMenu.Store.Configuration
{
    [Serializable]
    public class UIStyleName : ValueOf<string, UIStyleName>
    {
        public static UIStyleName Invalid => From(null);
    }
}