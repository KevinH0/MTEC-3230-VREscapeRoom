using JustAssets.Shared.Providers;
using JustAssets.Shared.UI.Animations;
using JustAssets.UI.StoreMenu.Store.Configuration;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

#pragma warning disable CS0649

namespace JustAssets.UI.StoreMenu.Store
{
    public sealed class UIBuySellDialog : AnimatedUI
    {
        private bool _isBuying;

        private ItemId _itemId;

        private int _money;

        private int _unitPrice;

        [SerializeField]
        private Button _buySellButton;

        [SerializeField]
        private TMP_Text _buySellButtonText;

        [SerializeField]
        private Button _cancelButton;

        [SerializeField]
        private TMP_Text _captionText;

        [SerializeField]
        private UIPlusMinusSlider _plusMinusSlider;

        [SerializeField]
        private UILabeledIconField _price;

        [SerializeField]
        private UILabeledIconField _remaining;

        [SerializeField]
        private TMP_Text _remainingText;

        [SerializeField]
        private string _storeBuyLocaKey = "Store_BuyItem";

        [SerializeField]
        private string _storeConfirmBuyLocaKey = "Store_ConfirmBuyItem";

        [SerializeField]
        private string _storeConfirmSellLocaKey = "Store_ConfirmSellItem";

        [SerializeField]
        [FormerlySerializedAs("_shopItem")]
        private UIStoreItem _storeItem;

        [SerializeField]
        private string _storeRemainingLocaKey = "Store_RemainingMoney";

        [SerializeField]
        private string _storeSellLocaKey = "Store_SellItem";

        [SerializeField]
        private string _storeSumLocaKey = "Store_SummedUpMoney";

        public void Init(ItemId itemId, string displayName, int price, bool isBuying, int owned, int maximum, int money,
            ILocalizationProvider localizationProvider, UIItemConfiguration itemConfiguration)
        {
            var count = 1;

            var storeConfirmBuySellText = localizationProvider.Localize(isBuying ? _storeConfirmBuyLocaKey : _storeConfirmSellLocaKey);
            var storeBuySellText = localizationProvider.Localize(isBuying ? _storeBuyLocaKey : _storeSellLocaKey);
            var remainingText = localizationProvider.Localize(isBuying ? _storeRemainingLocaKey : _storeSumLocaKey);

            _itemId = itemId;
            _isBuying = isBuying;
            _money = money;
            _unitPrice = price;
            _storeItem.Init(itemId, displayName, price, owned, UIStoreItem.StoreItemElements.PriceField, null, itemConfiguration);
            _plusMinusSlider.Set(count, maximum);
            PlusMinusSliderOnCountChanged(count);

            _buySellButtonText.text = storeBuySellText;
            _captionText.text = storeConfirmBuySellText;
            _remainingText.text = remainingText;
        }

        protected override void DeInit()
        {
            _cancelButton.onClick.RemoveAllListeners();
            _buySellButton.onClick.RemoveAllListeners();
            _plusMinusSlider.CountChanged -= PlusMinusSliderOnCountChanged;

            base.DeInit();
        }

        protected override void Init()
        {
            base.Init();

            _cancelButton.onClick.AddListener(CancelDialog);
            _buySellButton.onClick.AddListener(BuySell);
            _plusMinusSlider.CountChanged += PlusMinusSliderOnCountChanged;
        }

        private void BuySell()
        {
            var count = _plusMinusSlider.Count;
            Hide(new BuySellDialogClosedEventArgs(false, _itemId, count));
        }

        private void CancelDialog()
        {
            Hide(new AnimatedUIClosedEventArgs(true));
        }

        private void PlusMinusSliderOnCountChanged(int count)
        {
            var modelCost = count * _unitPrice;
            var remaining = _money + (_isBuying ? -modelCost : modelCost);
            _price.Init(modelCost.ToString());
            _remaining.Init(remaining.ToString());
        }
    }

    internal class BuySellDialogClosedEventArgs : AnimatedUIClosedEventArgs
    {
        public BuySellDialogClosedEventArgs(bool isCanceled, ItemId itemId, int amount) : base(isCanceled)
        {
            ItemId = itemId;
            Amount = amount;
        }

        public ItemId ItemId { get; }

        public int Amount { get; }
    }
}