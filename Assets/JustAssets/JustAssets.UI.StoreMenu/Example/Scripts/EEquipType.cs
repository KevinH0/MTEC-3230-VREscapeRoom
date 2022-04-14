using System;

namespace JustAssets.UI.StoreMenu.Example
{
    [Flags]
    public enum EEquipType
    {
        None = 0x0,

        Knife = 0x1,

        Staff = 0x2,

        Sword = 0x4,

        Hat = 0x8,

        Helmet = 0x10,

        Armlet = 0x20,

        Cloth = 0x40,

        Robe = 0x80,

        Armour = 0x100,

        AddOn = 0x200,

        Hammer = 0x400
    }
}