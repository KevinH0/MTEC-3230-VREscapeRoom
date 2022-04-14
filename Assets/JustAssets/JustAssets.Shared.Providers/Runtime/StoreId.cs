using System;

namespace JustAssets.Shared.Providers
{
    [Serializable]
    public class StoreId : ValueOf<int, StoreId>
    {
        public static StoreId Invalid => From(-1);
    }
}