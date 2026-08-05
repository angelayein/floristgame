using System.Collections.Generic;
using System.Linq;

namespace FloristGame.Core.Flowers
{
    // Immutable definition of a flower species. Unity-side FlowerDefinition
    // ScriptableObjects convert themselves into one of these so all gameplay
    // rules can operate on plain data with no engine dependency.
    public sealed class FlowerSpecies
    {
        public string Id { get; }
        public string DisplayName { get; }
        public FlowerColor Color { get; }
        public FlowerMeaning PrimaryMeaning { get; }
        public IReadOnlyList<FlowerMeaning> SecondaryMeanings { get; }
        public IReadOnlyList<Season> InSeason { get; }
        public float BaseStemPrice { get; }

        // Fraction of freshness lost per in-game day (0.1 = loses 10%/day).
        public float WiltRatePerDay { get; }

        // High-pollen species (lilies, etc.) that allergy-sensitive customers must avoid.
        public bool IsHighPollen { get; }

        public FlowerSpecies(
            string id,
            string displayName,
            FlowerColor color,
            FlowerMeaning primaryMeaning,
            IEnumerable<FlowerMeaning> secondaryMeanings,
            IEnumerable<Season> inSeason,
            float baseStemPrice,
            float wiltRatePerDay,
            bool isHighPollen)
        {
            Id = id;
            DisplayName = displayName;
            Color = color;
            PrimaryMeaning = primaryMeaning;
            SecondaryMeanings = (secondaryMeanings ?? Enumerable.Empty<FlowerMeaning>()).ToList();
            InSeason = (inSeason ?? Enumerable.Empty<Season>()).ToList();
            BaseStemPrice = baseStemPrice;
            WiltRatePerDay = wiltRatePerDay;
            IsHighPollen = isHighPollen;
        }

        public bool HasMeaning(FlowerMeaning meaning)
        {
            return PrimaryMeaning == meaning || SecondaryMeanings.Contains(meaning);
        }

        public bool IsInSeason(Season season)
        {
            if (season == Season.AllYear) return true;
            if (InSeason.Count == 0) return true;
            return InSeason.Contains(season) || InSeason.Contains(Season.AllYear);
        }
    }
}
