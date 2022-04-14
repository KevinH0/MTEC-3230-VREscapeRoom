using System;
using System.Linq;
using JustAssets.Shared.Providers;
using JustAssets.UI.StoreMenu.Store.Configuration;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
#pragma warning disable CS0649

namespace JustAssets.UI.StoreMenu.Store
{
    public sealed class UIStoreUnitWithStats : MonoBehaviour
    {
        public UIUnitStats UIUnitStatsUp;

        public UIUnitStats UIUnitStatsDown;

        public GameObject ArrowUp;

        public GameObject ArrowDown;

        public TMP_Text EquippedText;
        
        [FormerlySerializedAs("_toggle")]
        public Toggle Toggle;

        [SerializeField]
        private Image _unitImage;

        public UnitId UnitId { get; private set; }

        public event Action<UIStoreUnitWithStats, bool> ToggleChanged;

        public void Init(IUnitsProvider unitsProvider, ILocalizationProvider localization, ToggleGroup toggleGroup, UnitId unitId, UIStoreStatComparison.UnitStatData unitStatData, UIStoreConfiguration storeConfiguration)
        {
            UnitId = unitId;

            var positiveData = unitStatData.StatData.Where(x => x.Value.Delta > 0).ToDictionary(x => x.Key, x => x.Value);
            var negativeData = unitStatData.StatData.Where(x => x.Value.Delta < 0).ToDictionary(x => x.Key, x => x.Value);

            var negativeSectionVisible = negativeData.Count > 0;
            ArrowDown.SetActive(negativeSectionVisible);
            if (negativeSectionVisible)
                UIUnitStatsDown.Init(storeConfiguration, negativeData);

            var positiveSectionVisible = positiveData.Count > 0;
            ArrowUp.SetActive(positiveSectionVisible);
            if (positiveSectionVisible)
                UIUnitStatsUp.Init(storeConfiguration, positiveData);
            
            unitsProvider.GetImageAsync(unitId, sprite => _unitImage.sprite = sprite);
            EquippedText.gameObject.SetActive(unitStatData.IsEquipped);
            EquippedText.text = localization.Localize("Store_Equipped");

            Toggle.group = toggleGroup;
            Toggle.onValueChanged.RemoveAllListeners();
            Toggle.onValueChanged.AddListener(OnToggleChanged);
            Toggle.interactable = unitStatData.IsSuited;
            Toggle.SetIsOnWithoutNotify(false);
        }

        private void OnToggleChanged(bool isSet)
        {
            ToggleChanged?.Invoke(this, isSet);
        }
    }
}