using System.Collections.Generic;

namespace JustAssets.Shared.Providers
{
    public interface IItemProvider
    {
        Dictionary<StatId, int> ComputeDifference(ItemId itemCurrent, ItemId objId);

        ItemCategory GetCategory(ItemId id);

        bool TryGetEquipmentDetails(ItemId itemId, out EquipmentDetails details);

        int GetBaseCost(ItemId itemId);

        string GetName(ItemId itemId);

        bool IsEquipment(ItemId itemId);
    }
}