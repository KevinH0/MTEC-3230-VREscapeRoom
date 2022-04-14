namespace JustAssets.UI.StoreMenu.Example
{
    internal class ItemTemplate<TItemCategory> : IItemTemplate
    {
        public int Stock { get; set; }

        public int Cost { get; set; }

        public TItemCategory Category { get; set; }
    }
}