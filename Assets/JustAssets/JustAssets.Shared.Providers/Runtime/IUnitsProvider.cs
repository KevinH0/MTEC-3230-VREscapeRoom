using System;
using System.Collections.Generic;
using UnityEngine;

namespace JustAssets.Shared.Providers
{
    public interface IUnitsProvider
    {
        /// <summary>
        ///     This will set the item on the given slot. If you want to return the old item back to the inventory then use
        ///     GetEquipmentInSlot to retrieve it and do something with it.
        /// </summary>
        /// <param name="unitId">The unit to equip an item to.</param>
        /// <param name="itemId">The item to equip.</param>
        /// <param name="slot">The slot to equip the item to.</param>
        void Equip(UnitId unitId, ItemId itemId, EquipmentSlot slot);

        ItemId GetEquipmentInSlot(UnitId unitId, EquipmentSlot equipSlot);
        
        IEnumerable<ItemId> GetEquipped(UnitId unitId);

        void GetImageAsync(UnitId unitId, Action<Sprite> sprite);

        IDictionary<StatId, int> GetStats(UnitId unitId);

        IDictionary<UnitId, IDictionary<StatId, int>> GetStats();

        IEnumerable<UnitId> GetUnitIds();

        bool IsItemSuited(UnitId unitId, ItemId itemId, out EquipmentSlot equipSlot);
    }
}