using System;

namespace JustAssets.Shared.Providers
{
    [Serializable]
    public class StatId : ValueOf<int, StatId>
    {
        public static StatId Invalid => From(-1);
    }
}