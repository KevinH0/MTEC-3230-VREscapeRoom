using System.Collections.Generic;

namespace JustAssets.Shared.Providers
{
    public interface IStoreDataProvider
    {
        /// <summary>
        ///     Returns the stores inventory.
        /// </summary>
        /// <param name="storeId">The id of the store.</param>
        /// <returns>The assortment of goods and the amount available.</returns>
        Dictionary<ItemId, int> GetAssortmentOfGoods(StoreId storeId);

        /// <summary>
        ///     The ratio being applied when buying or selling an item. 100 means the base cost is applied for selling and buying.
        ///     50 Means buying is twice as expensive as the base price and selling will give just 50% of the base price.
        /// </summary>
        /// <param name="storeId">The id of the store.</param>
        /// <returns></returns>
        int GetSellRatio(StoreId storeId);

        /// <summary>
        ///     Removes a store item from the available quantity.
        /// </summary>
        /// <param name="storeId">The id of the store.</param>
        /// <param name="itemId">The item to remove.</param>
        /// <param name="amount">The amount to remove.</param>
        void SellItem(StoreId storeId, ItemId itemId, int amount);

        /// <summary>
        ///     Adds a store item to the available quantity.
        /// </summary>
        /// <param name="storeId">The id of the store.</param>
        /// <param name="itemId">The item to add.</param>
        /// <param name="amount">The amount to add.</param>
        void PurchaseItem(StoreId storeId, ItemId itemId, int amount);
    }
}