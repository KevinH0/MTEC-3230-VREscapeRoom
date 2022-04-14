using JustAssets.Shared.Providers;
using UnityEngine;

namespace JustAssets.UI.StoreMenu.Store.Configuration
{
    [CreateAssetMenu(menuName = "Scriptable Object/UI Store Configuration")]
    public class UIStoreConfiguration : ScriptableObject
    {
        public StatId[] VisibleStats;
    }
}