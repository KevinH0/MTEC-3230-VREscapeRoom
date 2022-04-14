using System;
using System.Collections.Generic;
using System.Linq;
using JustAssets.Shared.Providers;
using UnityEngine;

namespace JustAssets.UI.StoreMenu.Example
{
    public class UnitsProvider : IUnitsProvider
    {
        private readonly IItemProvider _itemProvider;

        private readonly Dictionary<UnitId, UnitsConfiguration.UnitData> _units;

        public UnitsProvider(UnitsConfiguration unitConfiguration, IItemProvider itemProvider)
        {
            _itemProvider = itemProvider;
            _units = unitConfiguration.Data.ToDictionary(k => k.Id, v => v);
        }

        public IEnumerable<UnitId> GetUnitIds()
        {
            return _units.Keys;
        }

        public bool IsItemSuited(UnitId unitId, ItemId itemId, out EquipmentSlot equipSlot)
        {
            if (!_units.TryGetValue(unitId, out UnitsConfiguration.UnitData data))
            {
                equipSlot = default;
                return false;
            }

            foreach (UnitsConfiguration.UnitData.SupportedEquipmentOnSlot equipmentType in data.SupportedOnSlot)
            {
                var isEquipment = _itemProvider.TryGetEquipmentDetails(itemId, out EquipmentDetails details);

                if (!isEquipment)
                    continue;

                if ((details.Type.Value & (int)equipmentType.Types) > 0)
                {
                    equipSlot = equipmentType.Slot;
                    return true;
                }
            }

            equipSlot = default;
            return false;
        }

        public ItemId GetEquipmentInSlot(UnitId unitId, EquipmentSlot equipSlot)
        {
            if (!_units.TryGetValue(unitId, out UnitsConfiguration.UnitData data))
                return ItemId.Invalid;

            return data.Equipment.FirstOrDefault(x => x.Slot == equipSlot)?.ItemId ?? ItemId.Invalid;
        }

        public IEnumerable<ItemId> GetEquipped(UnitId unitId)
        {
            if (!_units.TryGetValue(unitId, out UnitsConfiguration.UnitData data))
                return new ItemId[] { };

            return data.Equipment.Select(x => x.ItemId);
        }

        public void Equip(UnitId unitId, ItemId itemId, EquipmentSlot slot)
        {
            if (_units.TryGetValue(unitId, out UnitsConfiguration.UnitData data))
            {
                UnitsConfiguration.UnitData.EquipmentSlot equipmentSlot = data.Equipment.FirstOrDefault(x => x.Slot == slot);

                if (equipmentSlot != null)
                    equipmentSlot.ItemId = itemId;
                else
                    data.Equipment.Add(new UnitsConfiguration.UnitData.EquipmentSlot { ItemId = itemId, Slot = slot });
            }
        }

        public IDictionary<UnitId, IDictionary<StatId, int>> GetStats()
        {
            return GetUnitIds().ToDictionary(x => x, GetStats);
        }

        public void GetImageAsync(UnitId unitId, Action<Sprite> onSpriteLoaded)
        {
            if (!_units.TryGetValue(unitId, out UnitsConfiguration.UnitData data))
                onSpriteLoaded(null);

            onSpriteLoaded(data.Image);
        }

        public IDictionary<StatId, int> GetStats(UnitId unitId)
        {
            if (!_units.TryGetValue(unitId, out UnitsConfiguration.UnitData data))
                return null;

            var combined = new Dictionary<StatId, int>();
            foreach (UnitsConfiguration.UnitData.EquipmentSlot equipmentSlot in data.Equipment)
            {
                var isEquipment = _itemProvider.TryGetEquipmentDetails(equipmentSlot.ItemId, out EquipmentDetails equipmentDetails);

                if (!isEquipment)
                    continue;

                foreach (var stat in equipmentDetails.Stats)
                {
                    if (!combined.ContainsKey(stat.Key))
                        combined[stat.Key] = 0;

                    combined[stat.Key] += stat.Value;
                }
            }

            return combined;
        }
    }
}