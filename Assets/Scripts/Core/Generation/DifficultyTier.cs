namespace FloristGame.Core.Generation
{
    // Early game: Simple. Mid game: Intermediate (hagglers, allergy-sensitive).
    // Late game: Advanced/VIP (wedding-scale multi-bouquet orders, mystery
    // shoppers, high specificity, low patience).
    public enum DifficultyTier
    {
        Simple,
        Intermediate,
        Advanced,
        VIP
    }
}
