using System;

namespace JustAssets.Shared.Providers
{
    [Serializable]
    public class ItemId : ValueOf<int, ItemId>
    {
        public static ItemId Invalid => From(-1);
    }
}