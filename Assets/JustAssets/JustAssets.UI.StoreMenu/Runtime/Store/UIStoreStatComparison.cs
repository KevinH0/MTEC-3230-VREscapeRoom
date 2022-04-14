using System;
using System.Collections.Generic;
using JustAssets.Shared.Providers;
using JustAssets.Shared.UI;
using JustAssets.UI.StoreMenu.Store.Configuration;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS0649

namespace JustAssets.UI.StoreMenu.Store
{
    public sealed class UIStoreStatComparison : MonoBehaviour
    {
        private PooledUIListManager.PooledUIList<UIStoreUnitWithStats> _instances;

        [SerializeField]
        private Transform _contentContainer;

        [SerializeField]
        private UIStoreUnitWithStats _template;

        [SerializeField]
        private ToggleGroup _toggleGroup;

        public event Action<UnitId, bool> SelectionChanged;

        public void DisableUnselected(bool setDisabled)
        {
            foreach (var instance in _instances)
            {
                instance.Toggle.interactable = !setDisabled || instance.Toggle.isOn;
            }
        }

        public void Init(IUnitsProvider unitsProvider, ILocalizationProvider localizationProvider, IDictionary<UnitId, UnitStatData> unitsAndStats,
            UIStoreConfiguration storeConfiguration)
        {
            if (_instances == null)
                _instances = PooledUIListManager.GetPool(CreateUnitWithStatsUI, DestroyUnitWithStatsUI, template: _template);

            _instances.SetCount(unitsAndStats.Count);

            var i = 0;
            foreach (var unitAndStatChange in unitsAndStats)
            {
                UIStoreUnitWithStats instance = _instances[i];
                UnitId unitId = unitAndStatChange.Key;

                instance.Init(unitsProvider, localizationProvider, _toggleGroup, unitId, unitsAndStats[unitId], storeConfiguration);
                i++;
            }
        }

        public struct UnitStatData
        {
            public bool IsEquipped { get; }

            public bool IsSuited { get; }

            public Dictionary<StatId, StatData> StatData { get; }

            public UnitStatData(bool isEquipped, bool isSuited, Dictionary<StatId, StatData> statData)
            {
                IsEquipped = isEquipped;
                IsSuited = isSuited;
                StatData = statData;
            }
        }

        private UIStoreUnitWithStats CreateUnitWithStatsUI()
        {
            UIStoreUnitWithStats uiStoreUnitWithStats = Instantiate(_template, _contentContainer);
            uiStoreUnitWithStats.ToggleChanged += OnToggleChanged;
            return uiStoreUnitWithStats;
        }

        private void DestroyUnitWithStatsUI(UIStoreUnitWithStats instance)
        {
            instance.ToggleChanged -= OnToggleChanged;
        }

        private void OnToggleChanged(UIStoreUnitWithStats uiStoreUnitWithStats, bool isSet)
        {
            SelectionChanged?.Invoke(uiStoreUnitWithStats.UnitId, isSet);
        }
    }
}