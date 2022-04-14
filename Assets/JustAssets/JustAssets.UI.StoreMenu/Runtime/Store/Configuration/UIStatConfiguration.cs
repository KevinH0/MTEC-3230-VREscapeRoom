using System;
using System.Collections.Generic;
using System.Linq;
using JustAssets.Shared.Providers;
using UnityEngine;
using UnityEngine.Serialization;

namespace JustAssets.UI.StoreMenu.Store.Configuration
{
    [CreateAssetMenu(menuName = "Scriptable Object/StoreMenu UI Stat Configuration")]
    public class UIStatConfiguration : ScriptableObject
    {
        [FormerlySerializedAs("UnitMapping")]
        public List<Entry> Mapping = new List<Entry>();

        public Sprite Get(StatId stat)
        {
            var found = Mapping.FirstOrDefault(x => Equals(x.Stat, stat));
            if (found != null)
                return found.Sprite;

            found = Mapping.FirstOrDefault(x => x.Stat == StatId.Invalid);

            return found?.Sprite;
        }

        [Serializable]
        public class Entry
        {
            public Sprite Sprite;

            public StatId Stat;
        }
    }
}