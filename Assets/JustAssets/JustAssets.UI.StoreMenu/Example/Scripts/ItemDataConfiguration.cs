using System;
using System.Collections.Generic;
using System.Linq;
using JustAssets.Shared.Providers;
using UnityEngine;

namespace JustAssets.UI.StoreMenu.Example
{
    [CreateAssetMenu(menuName = "Scriptable Object/Item Data Configuration")]
    public class ItemDataConfiguration : ScriptableObject
    {
        [Serializable]
        public class ItemData
        {
            [Serializable]
            public class StatEntry
            {
                public StatId Stat;

                public int Value;
            }

            public ItemId Id;

            public string Name;

            public int Cost;
            
            public bool IsEquipment;

            public EItemCategory Category;

            [SerializeField]
            public List<StatEntry> EquipmentStats;

            public EquipmentType EquipmentType;
        }

        [SerializeField]
        private List<ItemData> _data = new List<ItemData>();

        public ItemData this[ItemId itemId]
        {
            get
            {
                return _data.FirstOrDefault(x => x.Id == itemId) ??
                       throw new ArgumentException($"There is no data defined for item with id {itemId}.", nameof(itemId));
            }
        }

        public Dictionary<ItemId, ItemData> Data
        {
            get
            {
                Dictionary<ItemId, ItemData> dictionary = new Dictionary<ItemId, ItemData>();
                foreach (ItemData data in _data)
                    dictionary[data.Id] = data;
                return dictionary;
            }
        }
    }
}