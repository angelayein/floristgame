using System;
using FloristGame.Core.Flowers;

namespace FloristGame.Core.Economy
{
    // A single purchase of stems for one species, bought on a given day.
    // Freshness decays linearly from 1.0 at purchase to 0.0 once fully wilted.
    public sealed class InventoryBatch
    {
        public FlowerSpecies Species { get; }
        public int Stems { get; internal set; }
        public int PurchaseDay { get; }

        public InventoryBatch(FlowerSpecies species, int stems, int purchaseDay)
        {
            Species = species;
            Stems = stems;
            PurchaseDay = purchaseDay;
        }

        public float FreshnessOnDay(int day)
        {
            int age = day - PurchaseDay;
            float freshness = 1f - age * Species.WiltRatePerDay;
            return Math.Max(0f, Math.Min(1f, freshness));
        }

        public bool IsWiltedOnDay(int day) => FreshnessOnDay(day) <= 0f;
    }
}
