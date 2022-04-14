using System;
using System.Collections.Generic;
using JustAssets.Shared.Providers;
using UnityEngine;

namespace JustAssets.UI.StoreMenu.Example
{
    [CreateAssetMenu(menuName = "Scriptable Object/Units Configuration")]
    public class UnitsConfiguration : ScriptableObject
    {
        [Serializable]
        public class UnitData
        {
            public UnitId Id;

            public List<EquipmentSlot> Equipment = new List<EquipmentSlot>();

            public List<SupportedEquipmentOnSlot> SupportedOnSlot = new List<SupportedEquipmentOnSlot>();

            public Sprite Image;

            [Serializable]
            public class SupportedEquipmentOnSlot
            {
                public Shared.Providers.EquipmentSlot Slot;

                public EEquipType Types;
            }

            [Serializable]
            public class EquipmentSlot
            {
                public ItemId ItemId;

                public Shared.Providers.EquipmentSlot Slot;
            }
        }

        public List<UnitData> Data = new List<UnitData>();
    }
}