using System;

namespace JustAssets.Shared.Providers
{
    [Serializable]
    public class UnitId : ValueOf<int, UnitId>
    {
        public static UnitId Invalid => From(-1);
    }
}