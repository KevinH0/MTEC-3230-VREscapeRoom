using JustAssets.UI.StoreMenu.Store;
using UnityEngine;

namespace JustAssets.UI.StoreMenu.Example
{
    public class UIStorePage : UIStorePageBase
    {
        [SerializeField]
        private StoreDataConfiguration _storeDataConfiguration;

        [SerializeField]
        private ItemDataConfiguration _itemDataConfiguration;

        [SerializeField]
        private LocalizationData _localizationData;

        [SerializeField]
        private UnitsConfiguration _unitConfiguration;

        protected override void Init()
        {
            base.Init();

            var storeDataProvider = new StoreDataProvider(_storeDataConfiguration);
            var itemProvider = new ItemProvider(_itemDataConfiguration);
            var inventoryProvider = new InventoryProvider(itemProvider);
            var unitsProvider = new UnitsProvider(_unitConfiguration, itemProvider);
            var storeManager = new StoreManager(inventoryProvider, itemProvider, storeDataProvider, unitsProvider);
            var localizationProvider = new Localization(_localizationData);
            Setup(storeManager, localizationProvider, itemProvider, unitsProvider, inventoryProvider);
        }
    }
}