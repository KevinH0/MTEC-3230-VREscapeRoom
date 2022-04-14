using System.Collections.Generic;

namespace JustAssets.Shared.Providers
{
    public interface IInventoryProvider
    {
        int Money { get; }

        int ItemCapacity { get; }

        void AddItem(ItemId itemId, int amount);

        void Credit(int amount);

        Dictionary<ItemCategory, List<KeyValuePair<ItemId, int>>> GetCategorized();

        int GetItemCount(ItemId itemId);

        IEnumerable<ItemId> GetItemIDs();

        bool HasItem(ItemId itemId);

        bool IsCategoryHidden(ItemCategory category);

        bool IsEmpty();

        void Pay(int amount);

        void TakeItem(ItemId itemId, int amount);
    }
}