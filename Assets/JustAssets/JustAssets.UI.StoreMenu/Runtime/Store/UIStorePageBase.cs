using System;
using System.Collections.Generic;
using System.Linq;
using JustAssets.Shared.Providers;
using JustAssets.Shared.UI;
using JustAssets.Shared.UI.Animations;
using JustAssets.UI.StoreMenu.Store.Configuration;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

// ReSharper disable CollectionNeverUpdated.Local
#pragma warning disable CS0649

namespace JustAssets.UI.StoreMenu.Store
{
    public abstract class UIStorePageBase : AnimatedUI
    {
        private ItemCategory _activeCategory;

        private StoreMenuCategory _activeMenu;

        private ItemId _activeShopItemId;

        private UIBuySellDialog _buySellDialogInstance;

        private GameObject _dialogBlockerInstance;

        private PooledUIListManager.PooledUIList<UIStoreItem> _entries;

        private UIEquipDialog _equipDialogInstance;

        private IItemProvider _itemProvider;

        private UnitId _lastActiveStatUnitId;

        private ILocalizationProvider _localizationProvider;

        private Dictionary<ItemCategory, List<KeyValuePair<ItemId, int>>> _stock;

        private IStoreManager _storeManager;

        private IUnitsProvider _unitsProvider;

        [SerializeField]
        private Button _buySelectedButton;

        [SerializeField]
        private UIBuySellDialog _buySellDialogTemplate;

        [SerializeField]
        private List<StoreCategoryButton> _categoryButtons;

        [SerializeField]
        private Button _closeButton;

        [SerializeField]
        private UIStoreConfiguration _configuration;

        [SerializeField]
        private GameObject _dialogBlocker;

        [SerializeField]
        private UIEquipDialog _equipDialogTemplate;

        [SerializeField]
        private UIHeaderBox _infoBox;

        [SerializeField, FormerlySerializedAs("_itemUIConfig")]
        private UIItemConfiguration _itemConfiguration;

        [SerializeField]
        private List<StoreMenuButton> _menuButtons;

        [SerializeField]
        private TMP_Text _money = null;

        [SerializeField]
        private ToggleGroup _shopContentContainer = null;

        [SerializeField]
        private UIStoreStatComparison _statBox;

        [SerializeField]
        private string _storeBuyLocaKey = "Store_BuyItem";

        [SerializeField]
        [FormerlySerializedAs("_shopEntryTemplate ")]
        private UIStoreItem _storeEntryTemplate = null;

        [SerializeField]
        private StoreId _storeId = StoreId.From(5);

        [SerializeField]
        private string _storeSellLocaKey = "Store_SellItem";

        private IInventoryProvider _inventoryProvider;

        public event Action Closed;

        public event Action<string> PlaySound;

        private event Action<ItemId, UnitId> EquipmentPreviewChanged;

        public ItemCategory ActiveCategory
        {
            get => _activeCategory;
            set
            {
                if (Equals(_activeCategory, value))
                    return;

                _activeCategory = value;

                StoreCategoryButton button = _categoryButtons.FirstOrDefault(x => Equals(x.Category, _activeCategory)) ??
                                                            throw new ArgumentException(
                                                                $"The category {value} is not bound to any store button. Please define a button to bind it to.", nameof(value));

                if (button.Button != null)
                    button.Button.isOn = true;
                else
                    throw new Exception($"The Button property of the category button {value} was not set.");
            }
        }

        private StoreMenuCategory ActiveMenu
        {
            get => _activeMenu;
            set
            {
                if (Equals(_activeMenu, value))
                    return;

                _activeMenu = value;

                StoreMenuButton button = _menuButtons.FirstOrDefault(x => Equals(x.Category, _activeMenu));
                button.Button.isOn = button.ActivationState;

                ChangeActiveMenu();
            }
        }

        public StoreId StoreId
        {
            get => _storeId;
            set => _storeId = value;
        }

        public void Setup(IStoreManager storeManager, ILocalizationProvider localization, IItemProvider itemProvider,
            IUnitsProvider unitsProvider, IInventoryProvider inventoryProvider)
        {
            _localizationProvider = localization;
            _storeManager = storeManager;
            _inventoryProvider = inventoryProvider;
            _itemProvider = itemProvider;
            _unitsProvider = unitsProvider;

            StoreUILocalizer.Instance.LocalizationProvider = _localizationProvider;

            _storeManager.GiveDebugFunds();
            RebuildMenus();
            AddEventsToCategoryButtons();
            AddEventsToMenuButtons();
            RebuildBuyMenu();

            ReSetup();
        }

        protected override void DeInit()
        {
            _closeButton.onClick.RemoveListener(OnClosed);

            _buySellDialogInstance.Hidden -= OnBuySellDialogHidden;
            _equipDialogInstance.Hidden -= OnEquipDialogHidden;

            RemoveEventsFromCategoryButtons();
            RemoveEventsFromMenuButtons();

            base.DeInit();
        }

        protected override void Init()
        {
            base.Init();

            _entries = PooledUIListManager.GetPool(CreateItem, RemoveItem);
            InitDialogs();

            _buySelectedButton.onClick.AddListener(OnBuySellItemClicked);
            _closeButton.onClick.AddListener(OnClosed);
        }

        protected virtual void OnClosed()
        {
            Closed?.Invoke();
        }

        protected virtual void OnEquipmentPreviewChanged(ItemId arg1, UnitId arg2)
        {
            EquipmentPreviewChanged?.Invoke(arg1, arg2);
        }

        protected virtual void OnPlaySound(string obj)
        {
            PlaySound?.Invoke(obj);
        }

        private void AddEventsToCategoryButtons()
        {
            foreach (StoreCategoryButton button in _categoryButtons)
            {
                UnityAction<bool> unityAction = isOn => OnCategoryChanged(isOn, button.Category);
                button.EventCallback = unityAction;
                button.Button.onValueChanged.AddListener(unityAction);
            }
        }

        private void AddEventsToMenuButtons()
        {
            foreach (StoreMenuButton button in _menuButtons)
            {
                UnityAction<bool> openMenuAction = isOn => { OnMenuChanged(isOn, button); };
                button.EventCallback = openMenuAction;
                button.Button.onValueChanged.AddListener(openMenuAction);
            }
        }

        private UIStoreItem AddItem()
        {
            UIStoreItem uiShopItem = _entries.Create(_shopContentContainer.transform);
            uiShopItem.Toggled += OnShopItemToggled;
            return uiShopItem;
        }

        private void ChangeActiveMenu()
        {
            if (_activeMenu == StoreMenuCategory.Buy)
                RebuildBuyMenu();
            else
                RebuildSellMenu();

            OnPlaySound("UI_SellBuy");
        }

        private IDictionary<UnitId, UIStoreStatComparison.UnitStatData> ComputeStatData(ItemId itemId, IDictionary<UnitId, Dictionary<StatId, int>> statChanges)
        {
            var unitStats = _unitsProvider.GetStats();

            IDictionary<UnitId, UIStoreStatComparison.UnitStatData> unitsAndStatData = new Dictionary<UnitId, UIStoreStatComparison.UnitStatData>();

            foreach (UnitId unitId in unitStats.Keys)
            {
                var statData = new Dictionary<StatId, StatData>();

                var stats = unitStats[unitId];
                var changes = statChanges.TryGetValue(unitId, out var result) ? result : new Dictionary<StatId, int>();

                foreach (StatId statId in stats.Keys.Union(changes.Keys))
                {
                    var current = stats.TryGetValue(statId, out var valueCurrent) ? valueCurrent : 0;
                    var delta = changes.TryGetValue(statId, out var valueDelta) ? valueDelta : 0;
                    statData[statId] = new StatData(current, delta);
                }

                var isEquipped = _unitsProvider.GetEquipped(unitId).Contains(itemId);
                var isSuited = _unitsProvider.IsItemSuited(unitId, itemId, out _);
                unitsAndStatData[unitId] = new UIStoreStatComparison.UnitStatData(isEquipped, isSuited, statData);
            }

            return unitsAndStatData;
        }

        private UIStoreItem CreateItem()
        {
            return Instantiate(_storeEntryTemplate, _shopContentContainer.transform);
        }

        private void CreateItemsForActiveCategory()
        {
            _entries.Clear();

            var items = _stock[ActiveCategory];
            for (var i = 0; i < items.Count; i++)
            {
                ItemId itemId = items[i].Key;
                int countAvailable = items[i].Value;
                UIStoreItem entry = AddItem();

                UpdateItemValue(entry, itemId, countAvailable);

                if (i == 0)
                    entry.Focus();
            }

            UIStoreItem storeItem = FindBestItemToFocus();
            if (storeItem != null) storeItem.Focus();
            UpdateMoney();
        }

        private UIStoreItem FindBestItemToFocus()
        {
            return _entries.FirstOrDefault(x => _storeManager.GetSellMaximum(x.Id) > 0);
        }

        private string GetDisplayName(ItemId itemId)
        {
            var itemName = _itemProvider.GetName(itemId);
            return _localizationProvider.Localize("Item_" + (itemName ?? "Empty"));
        }

        private string GetFlavorText(ItemId itemId)
        {
            var itemName = _itemProvider.GetName(itemId);
            return _localizationProvider.Localize("ItemFlavorText_" + (itemName ?? "Empty"));
        }

        private void HideBlocker()
        {
            _dialogBlockerInstance.SetActive(false);
        }

        private void InitDialogs()
        {
            if (_dialogBlockerInstance is null)
                _dialogBlockerInstance = Instantiate(_dialogBlocker, transform);

            _dialogBlockerInstance.SetActive(false);

            if (_buySellDialogInstance is null)
                _buySellDialogInstance = Instantiate(_buySellDialogTemplate, transform);

            _buySellDialogInstance.Hidden += OnBuySellDialogHidden;
            _buySellDialogInstance.gameObject.SetActive(false);

            if (_equipDialogInstance is null)
                _equipDialogInstance = Instantiate(_equipDialogTemplate, transform);

            _equipDialogInstance.Hidden += OnEquipDialogHidden;
            _equipDialogInstance.gameObject.SetActive(false);
        }

        private void OnBuySellDialogHidden(AnimatedUI animatedUI, AnimatedUIClosedEventArgs uiClosedEventArgs)
        {
            if (uiClosedEventArgs.IsCanceled)
            {
                HideBlocker();
                return;
            }

            var eventArgs = (BuySellDialogClosedEventArgs)uiClosedEventArgs;
            Purchase(eventArgs.Amount, eventArgs.ItemId);
        }

        private void OnBuySellItemClicked()
        {
            var owned = _storeManager.GetSellMaximum(_activeShopItemId);

            var isBuying = ActiveMenu == StoreMenuCategory.Buy;

            // Compute maximum affordable
            var maximum = isBuying ? _storeManager.GetBuyMaximum(_activeShopItemId, StoreId) : owned;

            var price = _storeManager.GetPrice(_activeShopItemId, StoreId, !isBuying);
            var displayName = GetDisplayName(_activeShopItemId);

            _buySellDialogInstance.Init(_activeShopItemId, displayName, price, isBuying, owned, maximum, _storeManager.Money, _localizationProvider,
                _itemConfiguration);

            _dialogBlockerInstance.SetActive(true);
            _buySellDialogInstance.Show();
        }

        private void OnCategoryChanged(bool isOn, ItemCategory buttonCategory)
        {
            if (!isOn)
                return;

            ActiveCategory = buttonCategory;

            CreateItemsForActiveCategory();
        }

        private void OnEquipDialogHidden(AnimatedUI sender, AnimatedUIClosedEventArgs eventArgs)
        {
            var equipEventArgs = (EquipDialogClosedEventArgs)eventArgs;
            _dialogBlockerInstance.SetActive(false);
            foreach (UnitId unit in equipEventArgs.UnitIds)
                _storeManager.Equip(unit, equipEventArgs.ItemId);

            UpdateStatComparison(equipEventArgs.ItemId);

            HideBlocker();
        }

        private void OnMenuChanged(bool isOn, StoreMenuButton button)
        {
            if (isOn != button.ActivationState)
                return;

            ActiveMenu = button.Category;
        }

        private void OnShopItemToggled(UIStoreItem storeItem, bool isOn)
        {
            if (!isOn)
                return;

            _activeShopItemId = storeItem.Id;
            var flavorText = GetFlavorText(_activeShopItemId);
            _infoBox.Init(flavorText);

            var showEquipmentDetails = _itemProvider.IsEquipment(storeItem.Id) && ActiveMenu == StoreMenuCategory.Buy;
            _statBox.gameObject.SetActive(showEquipmentDetails);

            if (showEquipmentDetails)
                UpdateStatComparison(storeItem.Id);

            UpdateBuyItemButton();
        }

        private void Purchase(int amount, ItemId itemId)
        {
            UIStoreItem uiShopItem = _entries.FirstOrDefault(x => x.Id == itemId);

            if (_activeMenu == StoreMenuCategory.Buy)
                UpdateBoughtItem(amount, itemId, uiShopItem);
            else
            {
                if (_storeManager.Sell(StoreId, itemId, amount))
                    UpdateEntirelySoldItem(uiShopItem);
                else
                    UpdatePartiallySoldItem(itemId, uiShopItem);

                HideBlocker();
            }

            UpdateMoney();

            OnPlaySound("UI_MoneyChanged");
        }

        private void RebuildBuyMenu()
        {
            _stock = _storeManager.GetBuyStock(StoreId);
            ActiveCategory = _stock.Keys.FirstOrDefault();
            _buySelectedButton.GetComponentInChildren<TMP_Text>().text = _localizationProvider.Localize(_storeBuyLocaKey);

            SetupActiveCategoryButtons();
            CreateItemsForActiveCategory();
        }

        private void RebuildMenus()
        {
            foreach (StoreMenuButton menuButton in _menuButtons)
            {
                switch (menuButton.Category)
                {
                    case StoreMenuCategory.Buy:
                        menuButton.Button.isOn = menuButton.ActivationState;
                        break;
                    case StoreMenuCategory.Sell:
                        menuButton.Button.interactable = !_inventoryProvider.IsEmpty();
                        break;
                }
            }
        }

        private void RebuildSellMenu()
        {
            UpdateSellStock();
            ActiveCategory = _stock.Keys.FirstOrDefault();
            _buySelectedButton.GetComponentInChildren<TMP_Text>().text = _localizationProvider.Localize(_storeSellLocaKey);

            SetupActiveCategoryButtons();
            CreateItemsForActiveCategory();
        }

        private void RemoveEventsFromCategoryButtons()
        {
            foreach (StoreCategoryButton button in _categoryButtons)
            {
                if (button.EventCallback != null)
                    button.Button.onValueChanged.RemoveListener(button.EventCallback);
            }
        }

        private void RemoveEventsFromMenuButtons()
        {
            foreach (StoreMenuButton menuButton in _menuButtons)
            {
                if (menuButton.EventCallback != null)
                    menuButton.Button.onValueChanged.RemoveListener(menuButton.EventCallback);
            }
        }

        private void RemoveItem(UIStoreItem obj)
        {
            obj.Toggled -= OnShopItemToggled;
            obj.Reset();
        }

        private void SetupActiveCategoryButtons()
        {
            foreach (StoreCategoryButton button in _categoryButtons)
                button.Button.gameObject.SetActive(_stock.Keys.Contains(button.Category));
        }

        private void ShowEquipDialog(int amount, ItemId itemId)
        {
            _dialogBlockerInstance.SetActive(true);

            var displayName = GetDisplayName(itemId);
            var statChanges = _storeManager.GetUnits(itemId);
            var unitsAndStats = ComputeStatData(itemId, statChanges)
                .Where(x => _unitsProvider.IsItemSuited(x.Key, itemId, out var slot) && _unitsProvider.GetEquipmentInSlot(x.Key, slot) != itemId)
                .ToDictionary(x => x.Key, x => x.Value);

            if (unitsAndStats.Count == 0)
            {
                HideBlocker();
                return;
            }

            _equipDialogInstance.Init(itemId, displayName, amount, unitsAndStats, _localizationProvider, _unitsProvider, _itemConfiguration, _configuration);
            _equipDialogInstance.Show();
        }

        private void UpdateBoughtItem(int amount, ItemId itemId, UIStoreItem uiShopItem)
        {
            _storeManager.Buy(itemId, amount, StoreId);
            UpdateItemValue(uiShopItem, itemId, _storeManager.GetBuyMaximum(itemId, StoreId));

            RebuildMenus();
            UpdateBuyItemButton();

            if (_itemProvider.IsEquipment(itemId))
                ShowEquipDialog(amount, itemId);
            else
                HideBlocker();
        }

        private void UpdateBuyItemButton()
        {
            bool result;

            if (_activeMenu == StoreMenuCategory.Buy)
                result = _storeManager.CanAfford(_activeShopItemId);
            else
                result = _storeManager.GetSellMaximum(_activeShopItemId) > 0;

            _buySelectedButton.interactable = result;
        }

        private void UpdateEntirelySoldItem(UIStoreItem uiShopItem)
        {
            var index = _entries.FindIndex(x => x == uiShopItem);
            index = Math.Max(0, index - 1);

            _entries.Destroy(uiShopItem);

            if (_entries.Count > 0)
            {
                _entries[index].Focus();
                UpdateSellStock();
            }
            else if (_stock.Keys.Count > 1)
                RebuildSellMenu();
            else
                RebuildMenus();
        }

        private void UpdateItemValue(UIStoreItem entry, ItemId itemId, int countAvailable)
        {
            var itemsInPossession = _storeManager.GetSellMaximum(itemId);
            var isBuying = _activeMenu == StoreMenuCategory.Buy;
            var price = _storeManager.GetPrice(itemId, StoreId, !isBuying);
            var displayName = GetDisplayName(itemId);
            var isSold = isBuying && _storeManager.GetBuyMaximum(itemId, StoreId) == 0;

            UIStoreItem.StoreItemElements flags = UIStoreItem.StoreItemElements.None;

            if (isSold)
                flags |= UIStoreItem.StoreItemElements.SoldLabel;
            else
                flags |= UIStoreItem.StoreItemElements.PriceField;

            if (price > 0 && countAvailable != 0 && (isBuying || itemsInPossession > 0))
                flags |= UIStoreItem.StoreItemElements.Enabled;

            entry.Init(itemId, displayName, price, itemsInPossession, flags, _shopContentContainer, _itemConfiguration);
        }

        private void UpdateMoney()
        {
            _money.text = _storeManager.Money.ToString();
        }

        private void UpdatePartiallySoldItem(ItemId itemId, UIStoreItem uiShopItem)
        {
            uiShopItem.Blur();
            UIStoreItem newFocus = uiShopItem;
            if (_storeManager.GetSellMaximum(itemId) == 0)
                newFocus = FindBestItemToFocus();

            if (newFocus != null) newFocus.Focus();

            UpdateItemValue(uiShopItem, itemId, _inventoryProvider.GetItemCount(itemId));
        }

        private void UpdateSellStock()
        {
            _stock = _storeManager.GetSellStock();
        }

        private void UpdateStatComparison(ItemId storeItemId)
        {
            var statChanges = _storeManager.GetUnits(storeItemId);
            var unitsAndStatData = ComputeStatData(storeItemId, statChanges);

            _statBox.Init(_unitsProvider, _localizationProvider, unitsAndStatData, _configuration);

            if (_lastActiveStatUnitId == UnitId.Invalid || !statChanges.Select(x => x.Key).Contains(_lastActiveStatUnitId))
                _lastActiveStatUnitId = statChanges.FirstOrDefault().Key;

            OnEquipmentPreviewChanged(_activeShopItemId, _lastActiveStatUnitId);
        }
    }
}