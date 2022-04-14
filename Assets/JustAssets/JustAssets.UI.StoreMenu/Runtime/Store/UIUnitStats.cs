using System.Collections.Generic;
using JustAssets.Shared.Providers;
using JustAssets.Shared.UI;
using JustAssets.UI.StoreMenu.Store.Configuration;
using UnityEngine;

namespace JustAssets.UI.StoreMenu.Store
{
    public class UIUnitStats : MonoBehaviour
    {
        [SerializeField] private UIUnitStat _statTemplate = null;

        [SerializeField] private bool _showOnlyStatChanges = true;

        private static PooledUIListManager.PooledUIList<UIUnitStat> _stats;
        
        private List<UIUnitStat> _instances = new List<UIUnitStat>();

        public void Init(UIStoreConfiguration configuration, IDictionary<StatId, StatData> unitStats)
        {
            if (_stats == null)
            {
                _stats = PooledUIListManager.GetPool(CreateUnitStatUI);
            }

            DeInit();

            foreach (StatId statType in configuration.VisibleStats)
            {
                if (_showOnlyStatChanges)
                {
                    if (unitStats != null && !unitStats.ContainsKey(statType))
                    {
                        continue;
                    }
                }

                var stat = unitStats.TryGetValue(statType, out var currentStat) ? currentStat : new StatData(0, 0);

                unitStats.TryGetValue(statType, out var delta);
                int? newStatValue = stat.Current + delta.Delta;

                var statsInstance = Add();
                statsInstance.Init(statType, stat.Current, newStatValue);
            }
        }

        private UIUnitStat Add()
        {
            var unitStatUI = _stats.Create();
            unitStatUI.transform.SetParent(transform);
            unitStatUI.transform.localScale = Vector3.one;
            _instances.Add(unitStatUI);
            return unitStatUI;
        }

        public void DeInit()
        {
            foreach (UIUnitStat ui in _instances)
            {
                _stats.Destroy(ui);
            }
            _instances.Clear();
        }

        private UIUnitStat CreateUnitStatUI()
        {
            return Instantiate(_statTemplate, transform);
        }
    }

    public struct StatData
    {
        public int Current;

        public int Delta;

        public StatData(int current, int delta)
        {
            Current = current;
            Delta = delta;
        }
    }
}