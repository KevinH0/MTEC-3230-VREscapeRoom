using System;
using System.Collections.Generic;
using System.Linq;
using JustAssets.Shared.Providers;
using UnityEngine;

namespace JustAssets.UI.StoreMenu.Store.Configuration
{
    [CreateAssetMenu(menuName = "Scriptable Object/StoreMenu UI Item Configuration")]
    public class UIItemConfiguration : ScriptableObject
    {
        public List<Entry> ItemMapping = new List<Entry>();

        public Sprite Get(ItemId itemId)
        {
            var found = ItemMapping.FirstOrDefault(x => x.Item == itemId);
            if (found != null)
                return found.Sprite;

            found = ItemMapping.FirstOrDefault(x => x.Item == ItemId.Invalid);

            return found?.Sprite;
        }

        [Serializable]
        public class Entry 
        {
            public ItemId Item;

            public Sprite Sprite;
        }
    }
} 