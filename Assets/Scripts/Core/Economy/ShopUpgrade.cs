namespace FloristGame.Core.Economy
{
    // Cooler capacity, register/haggle speed, design-table slots, delivery
    // van, etc. - a static catalog entry describing one purchasable upgrade.
    public sealed class ShopUpgrade
    {
        public string Id { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public float Cost { get; }

        public ShopUpgrade(string id, string displayName, string description, float cost)
        {
            Id = id;
            DisplayName = displayName;
            Description = description;
            Cost = cost;
        }
    }
}
