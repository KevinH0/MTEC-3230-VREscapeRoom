using System;
using System.Collections.Generic;
using System.Linq;
using JustAssets.Shared.Providers;

namespace JustAssets.UI.StoreMenu.Example
{
    public class StoreManager : IStoreManager
    {
        private readonly IStoreDataProvider _dataProvider;

        private readonly IInventoryProvider _inventoryProvider;

        private readonly IItemProvider _itemProvider;

        private readonly IUnitsProvider _unitProvider;

        public StoreManager(IInventoryProvider inventoryProvider, IItemProvider itemProvider, IStoreDataProvider dataProvider, IUnitsProvider unitProvider)
        {
            _inventoryProvider = inventoryProvider;
            _itemProvider = itemProvider;
            _dataProvider = dataProvider;
            _unitProvider = unitProvider;
        }

        /// <summary>
        ///     The players money.
        /// </summary>
        public int Money => _inventoryProvider.Money;

        public void Buy(ItemId itemId, int amount, StoreId storeId)
        {
            if (amount > ComputeItemBuyMaximum(itemId))
                throw new ArgumentOutOfRangeException(nameof(amount));

            var totalCost = GetPrice(itemId, storeId, false) * amount;

            _inventoryProvider.Pay(totalCost);
            _inventoryProvider.AddItem(itemId, amount);

            _dataProvider.SellItem(storeId, itemId, amount);
        }

        public int GetBuyMaximum(ItemId itemId, StoreId storeId)
        {
            var moneyMaximum = Money / GetPrice(itemId, storeId, false);
            var inventoryMaximum = ComputeItemBuyMaximum(itemId);
            var storeStockSetting = _dataProvider.GetAssortmentOfGoods(storeId)[itemId];
            var storeMaximum = storeStockSetting < 0 ? int.MaxValue : storeStockSetting;

            return Math.Min(moneyMaximum, Math.Min(inventoryMaximum, storeMaximum));
        }

        /// <summary>
        ///     Sells given item <see cref="amount" /> times.
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="itemId">Item to sell.</param>
        /// <param name="amount">Times to sell.</param>
        /// <returns>True if last item was sold.</returns>
        public bool Sell(StoreId storeId, ItemId itemId, int amount)
        {
            var totalCost = GetPrice(itemId, storeId, true) * amount;

            _inventoryProvider.TakeItem(itemId, amount);
            _inventoryProvider.Credit(totalCost);
            
            _dataProvider.PurchaseItem(storeId, itemId, amount);

            return !_inventoryProvider.HasItem(itemId);
        }
        
        /// <summary>
        ///     Returns the unit the item is best suited for and outputs the stat changes caused by the item.
        /// </summary>
        /// <param name="itemId">The item to inspect.</param>
        /// <returns>The best matching unit.</returns>
        public IDictionary<UnitId, Dictionary<StatId, int>> GetUnits(ItemId itemId)
        {
            IDictionary<UnitId, Dictionary<StatId, int>> matches = new Dictionary<UnitId, Dictionary<StatId, int>>();
            var unitIds = _unitProvider.GetUnitIds();

            if (!_itemProvider.IsEquipment(itemId))
                return null;

            foreach (UnitId unitId in unitIds)
            {
                if (_unitProvider.IsItemSuited(unitId, itemId, out EquipmentSlot slot))
                {
                    ItemId itemCurrentId = _unitProvider.GetEquipmentInSlot(unitId, slot);
                    var diff = ComputeDifference(itemCurrentId, itemId);
                    matches.Add(new KeyValuePair<UnitId, Dictionary<StatId, int>>(unitId, diff));
                }
            }

            // Find stats with most impact
            var heuristic = new Dictionary<StatId, int>();
            foreach (var matchesValue in matches.Values)
            {
                foreach (var statPair in matchesValue)
                {
                    int value;
                    if (!heuristic.TryGetValue(statPair.Key, out value))
                        value = heuristic[statPair.Key] = 0;

                    value += Math.Abs(statPair.Value);

                    heuristic[statPair.Key] = value;
                }
            }

            var priorities = heuristic.ToList();
            priorities.Sort((a, b) => a.Value.CompareTo(b.Value));

            // Remove stats which are 0 or least important
            for (var i = 1; i < priorities.Count; i++)
            {
                if (i <= 2 && priorities[i].Value != 0) continue;

                foreach (var matchesValue in matches.Values)
                    matchesValue.Remove(priorities[i].Key);
            }

            return matches;
        }

        /// <summary>
        ///     Computes how many items can be sold.
        /// </summary>
        /// <param name="itemId">The item trying to sell.</param>
        /// <returns>The count which can be sold.</returns>
        public int GetSellMaximum(ItemId itemId)
        {
            var owned = _inventoryProvider.GetItemCount(itemId);

            return owned;
        }

        /// <summary>
        ///     Computes the item price depending on item, store and if buying or selling.
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="storeId">The id of the store.</param>
        /// <param name="isSelling">If the item is sold.</param>
        /// <returns>The money to pay or retrieve.</returns>
        public int GetPrice(ItemId itemId, StoreId storeId, bool isSelling)
        {
            var baseCost = _itemProvider.GetBaseCost(itemId);

            if (!isSelling)
                return baseCost;

            var ratio = _dataProvider.GetSellRatio(storeId);

            return baseCost * ratio / 100;
        }

        /// <summary>
        ///     Checks if the player has enough money and inventory space.
        /// </summary>
        /// <param name="itemId">The item to buy.</param>
        /// <returns>If the item can be bought.</returns>
        public bool CanAfford(ItemId itemId)
        {
            var canAfford = _itemProvider.GetBaseCost(itemId) <= Money;

            if (!canAfford)
                return false;

            return ComputeItemBuyMaximum(itemId) > 0;
        }

        public void GiveDebugFunds()
        {
#if DEBUG
            _inventoryProvider.Credit(100000);
#endif
        }

        public Dictionary<ItemCategory, List<KeyValuePair<ItemId, int>>> GetSellStock()
        {
            var sellStock = new Dictionary<ItemCategory, List<KeyValuePair<ItemId, int>>>(_inventoryProvider.GetCategorized());

            var removeCategories = new List<ItemCategory>();
            foreach (var stockValue in sellStock)
            {
                for (var index = 0; index < stockValue.Value.Count; index++)
                {
                    if (_itemProvider.GetBaseCost(stockValue.Value[index].Key) < 0)
                        stockValue.Value.RemoveAt(index);
                }

                if (stockValue.Value.Count == 0)
                    removeCategories.Add(stockValue.Key);
            }

            foreach (ItemCategory itemCategory in removeCategories)
                sellStock.Remove(itemCategory);

            return sellStock;
        }

        public Dictionary<ItemCategory, List<KeyValuePair<ItemId, int>>> GetBuyStock(StoreId storeId)
        {
            var storeInventory = _dataProvider.GetAssortmentOfGoods(storeId);
            var stock = CategorizeItems(storeInventory);
            return stock;
        }

        public void Equip(UnitId unitId, ItemId itemId)
        {
            if (!_unitProvider.IsItemSuited(unitId, itemId, out EquipmentSlot slot))
                throw new ArgumentException("Item has to be suited to be equippable.");

            // Take the item from the inventory
            _inventoryProvider.TakeItem(itemId, 1);

            ItemId currentlyEquipped = _unitProvider.GetEquipmentInSlot(unitId, slot);

            // Put old item into inventory.
            if (currentlyEquipped != ItemId.Invalid)
                _inventoryProvider.AddItem(currentlyEquipped, 1);

            _unitProvider.Equip(unitId, itemId, slot);
        }

        /// <summary>
        ///     Sorts items by category.
        /// </summary>
        private Dictionary<ItemCategory, List<KeyValuePair<ItemId, int>>> CategorizeItems(Dictionary<ItemId, int> items)
        {
            var storage = new Dictionary<ItemCategory, List<KeyValuePair<ItemId, int>>>();

            foreach (var itemIdCount in items)
            {
                ItemCategory category = _itemProvider.GetCategory(itemIdCount.Key);

                if (_inventoryProvider.IsCategoryHidden(category))
                    continue;

                if (!storage.TryGetValue(category, out var list))
                    list = storage[category] = new List<KeyValuePair<ItemId, int>>();

                list.Add(new KeyValuePair<ItemId, int>(itemIdCount.Key, itemIdCount.Value));
            }

            return storage;
        }

        private Dictionary<StatId, int> ComputeDifference(ItemId itemCurrentId, ItemId itemNewId)
        {
            var result = new Dictionary<StatId, int>();
            bool isCurrentEquipment = _itemProvider.TryGetEquipmentDetails(itemCurrentId, out var currentEquipmentStats);
            bool isNewEquipment = _itemProvider.TryGetEquipmentDetails(itemNewId, out var nextEquipmentStats);

            if (isCurrentEquipment)
            {
                foreach (var equipmentStat in currentEquipmentStats.Stats)
                    result[equipmentStat.Key] = -equipmentStat.Value;
            }

            if (isNewEquipment)
            {
                foreach (var equipmentStat in nextEquipmentStats.Stats)
                {
                    if (!result.ContainsKey(equipmentStat.Key))
                        result[equipmentStat.Key] = 0;

                    result[equipmentStat.Key] += equipmentStat.Value;
                }
            }

            return result;
        }

        private int ComputeItemBuyMaximum(ItemId itemId)
        {
            return _inventoryProvider.ItemCapacity - _inventoryProvider.GetItemCount(itemId);
        }

        private int CountEquipped(ItemId itemId)
        {
            var equipped = 0;

            var unitIds = _unitProvider.GetUnitIds();
            foreach (UnitId unitId in unitIds)
            {
                foreach (ItemId itemEquipped in _unitProvider.GetEquipped(unitId))
                {
                    if (itemEquipped == itemId)
                        equipped++;
                }
            }

            return equipped;
        }
    }
}