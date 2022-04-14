using System;
using System.Collections.Generic;
using System.Linq;
using JustAssets.Shared.Providers;

namespace JustAssets.UI.StoreMenu.Example
{
    public sealed class Localization : ILocalizationProvider
    {
        private readonly Dictionary<string, string> _localizationData;

#if DEBUG
        private readonly LocalizationData _localizationDataFile;
#endif

        public Localization(LocalizationData localizationData)
        {
#if DEBUG
            _localizationDataFile = localizationData;
#endif
            
            _localizationData = localizationData?.Data?.ToDictionary(k => k.Key, v => v.Translation) ?? new Dictionary<string, string>();
        }

        public string Localize(string locaKey, params object[] parameters)
        {
            if (_localizationData.TryGetValue(locaKey, out var translation))
                return string.Format(translation, parameters);

#if DEBUG
            if (_localizationDataFile.Data.All(x => x.Key != locaKey))
                _localizationDataFile.Data.Add(new LocalizationData.Mapping { Key = locaKey, Translation = locaKey });
#endif

            return string.Format(locaKey, parameters);
        }

        public event Action DataChanged;

        private void OnDataChanged()
        {
            DataChanged?.Invoke();
        }
    }
}