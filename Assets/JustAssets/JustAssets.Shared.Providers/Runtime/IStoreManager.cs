using System.Collections.Generic;

namespace JustAssets.Shared.Providers
{
    public interface IStoreManager
    {
        int Money { get; }

        void Buy(ItemId itemId, int amount, StoreId storeId);

        /// <summary>
        ///     Checks if the player has enough money and inventory space.
        /// </summary>
        /// <param name="itemId">The item to buy.</param>
        /// <returns>If the item can be bought.</returns>
        bool CanAfford(ItemId itemId);

        void Equip(UnitId unitId, ItemId itemId);

        int GetBuyMaximum(ItemId itemId, StoreId storeId);

        Dictionary<ItemCategory, List<KeyValuePair<ItemId, int>>> GetBuyStock(StoreId storeId);

        /// <summary>
        ///     Computes the item price depending on item, store and if buying or selling.
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="storeId">The id of the store.</param>
        /// <param name="isSelling">If the item is sold.</param>
        /// <returns>The money to pay or retrieve.</returns>
        int GetPrice(ItemId itemId, StoreId storeId, bool isSelling);

        /// <summary>
        ///     Computes how many items can be sold.
        /// </summary>
        /// <param name="itemId">The item trying to sell.</param>
        /// <returns>The count which can be sold.</returns>
        int GetSellMaximum(ItemId itemId);

        Dictionary<ItemCategory, List<KeyValuePair<ItemId, int>>> GetSellStock();

        /// <summary>
        ///     Returns the unit the item is best suited for and outputs the stat changes caused by the item.
        /// </summary>
        /// <param name="itemId">The item to inspect.</param>
        /// <returns>The best matching unit.</returns>
        IDictionary<UnitId, Dictionary<StatId, int>> GetUnits(ItemId itemId);

        void GiveDebugFunds();

        /// <summary>
        ///     Sells given item <see cref="amount" /> times.
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="itemId">TItem to sell.</param>
        /// <param name="amount">Times to sell.</param>
        /// <returns>True if last item was sold.</returns>
        bool Sell(StoreId storeId, ItemId itemId, int amount);
    }
}