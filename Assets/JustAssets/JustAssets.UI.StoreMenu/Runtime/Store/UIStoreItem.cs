using System;
using JustAssets.Shared.Providers;
using JustAssets.UI.StoreMenu.Store.Configuration;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace JustAssets.UI.StoreMenu.Store
{
    public class UIStoreItem : MonoBehaviour
    {
        [SerializeField]
        private Image _icon;

        [SerializeField]
        private TMP_Text _name;

        [SerializeField]
        private TMP_Text _price;

        [SerializeField]
        private TMP_Text _owned;
        
        [SerializeField, FormerlySerializedAs("Toggle")]
        private Toggle _toggle;

        [SerializeField]
        private TMP_Text _soldText;

        [SerializeField]
        private GameObject _priceField;

        [SerializeField]
        private GameObject _soldLabel;

        public void OnEnable()
        {
            if (_toggle != null) _toggle.onValueChanged.AddListener(OnToggled);
        }
        public void OnDisable()
        {
            if (_toggle != null) _toggle.onValueChanged.RemoveListener(OnToggled);
        }

        private void OnToggled(bool isOn)
        {
            OnToggled(this, isOn);
        }

        public void Init(ItemId id, string displayName, int price, int itemsInPossession, StoreItemElements storeItemElements, ToggleGroup toggleGroup,
            UIItemConfiguration configuration)
        {
            Id = id;

            if (_toggle != null)
            {
                _toggle.@group = toggleGroup;
                _toggle.interactable = storeItemElements.HasFlag(StoreItemElements.Enabled);
            }

            _name.text = displayName;

            _priceField.gameObject.SetActive(storeItemElements.HasFlag(StoreItemElements.PriceField));
            _soldLabel.gameObject.SetActive(storeItemElements.HasFlag(StoreItemElements.SoldLabel));
            _soldText.gameObject.SetActive(storeItemElements.HasFlag(StoreItemElements.SoldLabel));
            _price.text = price > 0 ? price.ToString() : "-";
            _icon.sprite = configuration.Get(id);
            _owned.text = itemsInPossession.ToString();
        }

        [Flags]
        public enum StoreItemElements
        {
            None = 0,

            PriceField = 1,

            SoldLabel = 2,

            Enabled = 4
        }

        public ItemId Id { get; private set; }

        public event Action<UIStoreItem, bool> Toggled;

        protected virtual void OnToggled(UIStoreItem sender, bool isOn)
        {
            Toggled?.Invoke(sender, isOn);
        }

        public void Focus()
        {
            _toggle.isOn = true;
        }

        public void Reset()
        {
            _name.text = "-";
            _toggle.group = null;
            _toggle.isOn = false;
        }

        public void Blur()
        {
            _toggle.isOn = false;
        }
    }
}
