using System.Collections.Generic;

namespace JustAssets.Shared.Providers
{
    public struct EquipmentDetails
    {
        public EquipmentDetails(Dictionary<StatId, int> stats, EquipmentType type)
        {
            Stats = stats;
            Type = type;
        }

        public Dictionary<StatId, int> Stats { get; }

        public EquipmentType Type { get; }
    }
}