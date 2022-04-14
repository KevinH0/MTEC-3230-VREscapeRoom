using System;

namespace JustAssets.Shared.Providers
{
    public interface ILocalizationProvider
    {
        string Localize(string locaKey, params object[] parameters);

        event Action DataChanged;
    }
}
