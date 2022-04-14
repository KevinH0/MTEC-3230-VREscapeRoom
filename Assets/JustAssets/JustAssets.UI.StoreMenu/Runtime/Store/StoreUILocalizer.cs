using System.Collections.Generic;
using JustAssets.Shared.Providers;

namespace JustAssets.UI.StoreMenu.Store
{
    public class StoreUILocalizer
    {
        private List<LocalizedStoreUIText> _elementsToLocalize = new List<LocalizedStoreUIText>();

        private ILocalizationProvider _localizationProvider;

        public static StoreUILocalizer Instance { get; } = new StoreUILocalizer();

        public ILocalizationProvider LocalizationProvider
        {
            get => _localizationProvider;
            set
            {
                _localizationProvider = value;
                foreach (var uiText in _elementsToLocalize)
                {
                    Localize(uiText);
                }
            }
        }

        private void Localize(LocalizedStoreUIText uiText)
        {
            uiText.SetText(_localizationProvider.Localize(uiText.LocalizationKey));
        }

        public void Register(LocalizedStoreUIText elementToLocalize)
        {
            _elementsToLocalize.Add(elementToLocalize);

            if (_localizationProvider != null)
                Localize(elementToLocalize);
        }

        public void Unregister(LocalizedStoreUIText elementToLocalize)
        {
            _elementsToLocalize.Remove(elementToLocalize);
        }
    }
}