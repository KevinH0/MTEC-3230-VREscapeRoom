using System.Collections.Generic;
using System.Linq;
using JustAssets.Shared.Providers;

namespace JustAssets.UI.StoreMenu.Example
{
    public class InventoryProvider : IInventoryProvider
    {
        private readonly IItemProvider _itemProvider;

        private readonly Dictionary<ItemId, int> _stock = new Dictionary<ItemId, int>();

        public InventoryProvider(IItemProvider itemProvider)
        {
            _itemProvider = itemProvider;
        }

        public int Money { get; private set; }

        public int ItemCapacity => 99;

        public Dictionary<ItemCategory, List<KeyValuePair<ItemId, int>>> GetCategorized()
        {
            Dictionary<ItemCategory, List<KeyValuePair<ItemId, int>>> dictionary = new Dictionary<ItemCategory, List<KeyValuePair<ItemId, int>>>();
            foreach (var pairs in _stock.GroupBy(x => _itemProvider.GetCategory(x.Key)))
                dictionary.Add(pairs.Key, pairs.Select(x => new KeyValuePair<ItemId, int>(x.Key, x.Value)).ToList());
            return dictionary;
        }

        public int GetItemCount(ItemId itemId)
        {
            return _stock.TryGetValue(itemId, out var count) ? count : 0;
        }

        public void TakeItem(ItemId itemId, int amount)
        {
            _stock[itemId] -= amount;
            if (_stock[itemId] <= 0)
                _stock.Remove(itemId);
        }

        public void AddItem(ItemId itemId, int amount)
        {
            if (_stock.ContainsKey(itemId))
                _stock[itemId] += amount;
            else
                _stock.Add(itemId, amount);
        }

        public bool HasItem(ItemId itemId)
        {
            return _stock.ContainsKey(itemId);
        }

        public void Credit(int amount)
        {
            Money += amount;
        }

        public void Pay(int amount)
        {
            Money -= amount;
        }

        public IEnumerable<ItemId> GetItemIDs()
        {
            return _stock.Keys;
        }

        public bool IsCategoryHidden(ItemCategory category)
        {
            return category.Value <= 0;
        }

        public bool IsEmpty()
        {
            var inventoryItems = GetItemIDs();

            foreach (ItemId inventoryItem in inventoryItems)
            {
                var worthSomething = _itemProvider.GetBaseCost(inventoryItem) > 0;
                if (worthSomething)
                {
                    ItemCategory itemCategory = _itemProvider.GetCategory(inventoryItem);
                    var isCategoryHidden = IsCategoryHidden(itemCategory);
                    if (!isCategoryHidden)
                        return false;
                }
            }

            return true;
        }
    }
}