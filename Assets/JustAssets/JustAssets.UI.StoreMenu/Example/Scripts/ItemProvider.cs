using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JustAssets.Shared.Providers;

namespace JustAssets.UI.StoreMenu.Example
{

    public class ItemProvider : IItemProvider
    {
        private readonly ItemDataConfiguration _itemDataConfiguration;

        public ItemProvider(ItemDataConfiguration itemDataConfiguration)
        {
            _itemDataConfiguration = itemDataConfiguration;
        }

        public bool TryGetEquipmentDetails(ItemId itemId, out EquipmentDetails details)
        {
            if (itemId == ItemId.Invalid)
                return false;

            ItemDataConfiguration.ItemData item = _itemDataConfiguration[itemId];
            var dictionary = item.EquipmentStats.ToDictionary(k => k.Stat, v => v.Value);
            EquipmentType itemEquipmentType = item.EquipmentType;
            details = new EquipmentDetails(dictionary, itemEquipmentType);
            
            return true;
        }

        public string GetName(ItemId itemId)
        {
            return _itemDataConfiguration[itemId].Name;
        }

        public bool IsEquipment(ItemId itemId)
        {
            return _itemDataConfiguration[itemId].IsEquipment;
        }

        public Dictionary<StatId, int> ComputeDifference(ItemId itemCurrent, ItemId itemNew)
        {
            var result = new Dictionary<StatId, int>();

            if (itemCurrent != ItemId.Invalid)
            {
                var currentItemInstance = _itemDataConfiguration[itemCurrent].EquipmentStats;
                foreach (ItemDataConfiguration.ItemData.StatEntry equipmentStat in currentItemInstance)
                    result[equipmentStat.Stat] = -equipmentStat.Value;
            }

            if (itemNew != ItemId.Invalid)
            {
                var nextItemInstance = _itemDataConfiguration[itemNew].EquipmentStats;
                foreach (ItemDataConfiguration.ItemData.StatEntry equipmentStat in nextItemInstance)
                {
                    if (!result.ContainsKey(equipmentStat.Stat))
                        result[equipmentStat.Stat] = 0;

                    result[equipmentStat.Stat] += equipmentStat.Value;
                }
            }

            return result;
        }

        public ItemCategory GetCategory(ItemId id)
        {
            return ItemCategory.From((int)_itemDataConfiguration[id].Category);
        }

        public int GetBaseCost(ItemId itemId)
        {
            return _itemDataConfiguration[itemId].Cost;
        }
    }
}