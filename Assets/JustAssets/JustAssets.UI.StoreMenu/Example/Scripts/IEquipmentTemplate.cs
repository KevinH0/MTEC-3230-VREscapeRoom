using System.Collections.Generic;

namespace JustAssets.UI.StoreMenu.Example
{
    internal interface IEquipmentTemplate<TStatKey>
    {
        Dictionary<TStatKey, int> Stats { get; set; }
    }
}