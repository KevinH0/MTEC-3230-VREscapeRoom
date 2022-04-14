using UnityEngine;

namespace JustAssets.Shared.Providers
{
    public interface IUnitRegistryProvider
    {
        GameObject FindLoadedUnit(int unitId);

        void LookAt(int fromUnitId, int toUnitId, Transform overrideTarget);
    }
}