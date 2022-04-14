using System;
using System.Collections.Generic;
using System.Linq;
using JustAssets.Shared.Providers;

namespace JustAssets.UI.StoreMenu.Example
{
    public class StoreDataProvider : IStoreDataProvider
    {
        private StoreDataConfiguration _data;

        public StoreDataProvider(StoreDataConfiguration data)
        {
            _data = data;
        }

        public Dictionary<ItemId, int> GetAssortmentOfGoods(StoreId storeId)
        {
            return _data[storeId].ToDictionary(x => x.Key, x => Math.Max(-1, x.Value.Amount - x.Value.SoldAmount));
        }

        public int GetSellRatio(StoreId storeId)
        {
            return _data.GetRatio(storeId);
        }

        public void SellItem(StoreId storeId, ItemId itemId, int amount)
        {
            _data[storeId][itemId].SoldAmount += amount;
        }

        public void PurchaseItem(StoreId storeId, ItemId itemId, int amount)
        {
            var storeItem = _data[storeId][itemId];
            storeItem.SoldAmount = Math.Max(0, storeItem.SoldAmount - amount);
        }
    }
}