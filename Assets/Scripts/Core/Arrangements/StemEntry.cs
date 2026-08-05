namespace FloristGame.Core.Arrangements
{
    // One line item within an arrangement: a species, how many stems of it,
    // and the average freshness of the stock used to build it.
    public readonly struct StemEntry
    {
        public string SpeciesId { get; }
        public int Count { get; }
        public float FreshnessAtUse { get; }

        public StemEntry(string speciesId, int count, float freshnessAtUse)
        {
            SpeciesId = speciesId;
            Count = count;
            FreshnessAtUse = freshnessAtUse;
        }
    }
}
