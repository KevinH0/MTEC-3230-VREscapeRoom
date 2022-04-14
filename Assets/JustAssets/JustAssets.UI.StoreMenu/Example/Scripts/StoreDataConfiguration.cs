using System;
using System.Collections.Generic;
using System.Linq;
using JustAssets.Shared.Providers;
using UnityEngine;

namespace JustAssets.UI.StoreMenu.Example
{
    [CreateAssetMenu(menuName = "Scriptable Object/Store Data Configuration")]
    public class StoreDataConfiguration : ScriptableObject
    {
        [SerializeField]
        private List<StoreData> _storeData = new List<StoreData>();

        public Dictionary<ItemId, StoreData.StoreItem> this[StoreId storeId]
        {
            get { return GetStore(storeId).Stock.ToDictionary(x => x.ItemId, x => x); }
        }

        public int GetRatio(StoreId storeId)
        {
            return GetStore(storeId).Ratio;
        }

        private StoreData GetStore(StoreId storeId)
        {
            return _storeData.First(x => x.StoreId == storeId);
        }

        [Serializable]
        public class StoreData
        {
            public StoreId StoreId;
            
            public int Ratio = 100;

            public List<StoreItem> Stock = new List<StoreItem>();
            
            [Serializable]
            public class StoreItem
            {
                public ItemId ItemId;

                public int Amount;

                public int SoldAmount;
            }
        }
    }
}