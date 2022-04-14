using System.Collections.Generic;

namespace JustAssets.UI.StoreMenu.Example
{

    internal class EquipmentTemplate<TItemCategory, TStatKey> : ItemTemplate<TItemCategory>, IEquipmentTemplate<TStatKey>
    {
        public Dictionary<TStatKey, int> Stats { get; set; }
    }
}