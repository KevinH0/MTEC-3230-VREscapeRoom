using JustAssets.Shared.Providers;
using JustAssets.UI.StoreMenu;
using JustAssets.UI.StoreMenu.Example;
using UnityEditor;

namespace Assets.JustAssets.Example
{
    [CustomPropertyDrawer(typeof(ItemCategory))]
    public sealed class ItemCategoryDrawer : ValueOfDrawer<EItemCategory>
    { }
}