using System;

namespace JustAssets.UI.StoreMenu.Example
{
    [Flags]
    public enum EItemCategory
    {
        Equipment = 0x1,

        Consumable = 0x2,

        KeyItem = 0x4
    }
}