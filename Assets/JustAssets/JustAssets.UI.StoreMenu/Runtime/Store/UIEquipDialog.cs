using System;
using System.Collections.Generic;
using System.Linq;
using JustAssets.Shared.Providers;
using JustAssets.Shared.UI.Animations;
using JustAssets.UI.StoreMenu.Store.Configuration;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS0649

namespace JustAssets.UI.StoreMenu.Store
{
    public sealed class UIEquipDialog : AnimatedUI
    {
        
        [SerializeField] private Button _equipButton;

        [SerializeField] private TMP_Text _captionText;

        [SerializeField] private TMP_Text _notificationText;

        [SerializeField] private TMP_Text _equipButtonText;

        [SerializeField] private UIStoreItem _storeItem;

        [SerializeField] private UIStoreStatComparison _comparison;

        [SerializeField]
        private string _storeNoMoreItemsLocaKey = "Store_EquipNoMoreItems";

        [SerializeField]
        private string _storeEquipCaptionLocaKey = "Store_EquipDialogCaption";

        [SerializeField]
        private string _storeEquipLocaKey = "Store_EquipDialogButton";

        [SerializeField]
        private string _storeDoNotEquipLocaKey = "Store_DoNotEquipDialogButton";

        private Dictionary<UnitId, bool> _equipResult;

        private ILocalizationProvider _localization;

        private ItemId _itemId;

        private string _displayName;

        private UIItemConfiguration _itemConfiguration;

        private int _boughtCount;

        protected override void Init()
        {
            base.Init();
            _equipButton.onClick.AddListener(OnEquip);
        }

        protected override void DeInit()
        {
            _equipButton.onClick.RemoveListener(OnEquip);
            base.DeInit();
        }
        
        private void OnEquip()
        {
            var units = _equipResult.Where(x => x.Value).Select(x=>x.Key).ToList();
            Hide(new EquipDialogClosedEventArgs(false, _itemId, units));
        }

        public void Init(ItemId itemId, string displayName, int count, IDictionary<UnitId, UIStoreStatComparison.UnitStatData> unitsAndStats,
            ILocalizationProvider localizationProvider, IUnitsProvider unitsProvider, UIItemConfiguration itemConfiguration,
            UIStoreConfiguration storeConfig)
        {
            _itemId = itemId;
            _boughtCount = count;
            _displayName = displayName;
            _itemConfiguration = itemConfiguration;
            _equipResult = unitsAndStats.Keys.ToDictionary(x => x, x=>false);
            _localization = localizationProvider;

            UpdateItem(count);

            SetButtonText(false);
            _captionText.text = localizationProvider.Localize(_storeEquipCaptionLocaKey);
            _notificationText.text = localizationProvider.Localize(_storeNoMoreItemsLocaKey);

            _comparison.Init(unitsProvider, localizationProvider, unitsAndStats, storeConfig);
            _comparison.SelectionChanged += OnSelectedUnitsChanged;
        }

        private void UpdateItem(int count)
        {
            _storeItem.Init(_itemId, _displayName, 0, count, UIStoreItem.StoreItemElements.None, null, _itemConfiguration);
        }

        private void SetButtonText(bool anySelected)
        {
            var storeEquipText = _localization.Localize(anySelected ? _storeEquipLocaKey : _storeDoNotEquipLocaKey);
            _equipButtonText.text = storeEquipText;
        }

        private void OnSelectedUnitsChanged(UnitId unit, bool isSet)
        {
            _equipResult[unit] = isSet;
            var selectedCount = _equipResult.Count(x => x.Value);

            UpdateItem(_boughtCount - selectedCount);

            _notificationText.enabled = selectedCount >= _boughtCount;

            _comparison.DisableUnselected(selectedCount >= _boughtCount);

            SetButtonText(selectedCount > 0);
        }
    }

    internal class EquipDialogClosedEventArgs : AnimatedUIClosedEventArgs
    {
        public ItemId ItemId { get; }

        public List<UnitId> UnitIds { get; }

        public EquipDialogClosedEventArgs(bool isCanceled, ItemId itemId, List<UnitId> unitIds) : 
            base(isCanceled)
        {
            ItemId = itemId;
            UnitIds = unitIds;
        }
    }
}