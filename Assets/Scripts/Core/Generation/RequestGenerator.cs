using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FloristGame.Core.Arrangements;
using FloristGame.Core.Customers;
using FloristGame.Core.Flowers;

namespace FloristGame.Core.Generation
{
    // Procedurally builds a CustomerRequest for the day, scaling vagueness,
    // strictness and haggling difficulty with the player's reputation tier.
    // Takes a System.Random so callers (and tests) can seed it for
    // deterministic output.
    public static class RequestGenerator
    {
        public static DifficultyTier TierForReputation(float reputation)
        {
            if (reputation < 30f) return DifficultyTier.Simple;
            if (reputation < 60f) return DifficultyTier.Intermediate;
            if (reputation < 85f) return DifficultyTier.Advanced;
            return DifficultyTier.VIP;
        }

        public static CustomerRequest Generate(
            Random rng,
            IReadOnlyList<OccasionProfile> occasions,
            IReadOnlyList<FlowerSpecies> catalog,
            DifficultyTier tier,
            string customerName)
        {
            if (occasions == null || occasions.Count == 0)
            {
                throw new ArgumentException("At least one OccasionProfile is required.", nameof(occasions));
            }

            var occasion = occasions[rng.Next(occasions.Count)];

            int budgetJitter = Math.Max(1, (occasion.TypicalBudgetMax - occasion.TypicalBudgetMin) / 4);
            int budgetMin = Math.Max(1, occasion.TypicalBudgetMin - rng.Next(0, budgetJitter));
            int budgetMax = occasion.TypicalBudgetMax + rng.Next(0, budgetJitter);

            float vagueness = VaguenessRangeFor(tier, rng);
            bool hasAllergy = tier != DifficultyTier.Simple && rng.NextDouble() < 0.2;

            List<FlowerColor> preferredColors = vagueness > 0.6f
                ? new List<FlowerColor>()
                : PickPreferredColors(rng, occasion);

            List<string> required = (tier == DifficultyTier.Advanced || tier == DifficultyTier.VIP) && rng.NextDouble() < 0.4
                ? PickSpeciesIds(rng, catalog, 1)
                : new List<string>();

            List<string> excluded = tier != DifficultyTier.Simple && rng.NextDouble() < 0.3
                ? PickSpeciesIds(rng, catalog.Where(s => !required.Contains(s.Id)).ToList(), 1)
                : new List<string>();

            ArrangementStyle? style = (tier == DifficultyTier.Advanced || tier == DifficultyTier.VIP) && rng.NextDouble() < 0.5
                ? (ArrangementStyle)rng.Next(0, 4)
                : (ArrangementStyle?)null;

            int minStems = MinStemsFor(tier, rng);
            float patience = PatienceFor(tier, rng);

            string flavorText = BuildFlavorText(occasion, vagueness, preferredColors, required, excluded, hasAllergy, catalog, minStems);

            return new CustomerRequest(
                customerName,
                occasion,
                budgetMin,
                budgetMax,
                preferredColors,
                required,
                excluded,
                hasAllergy,
                style,
                minStems,
                patience,
                vagueness,
                flavorText);
        }

        private static float VaguenessRangeFor(DifficultyTier tier, Random rng)
        {
            switch (tier)
            {
                case DifficultyTier.Simple: return 0.6f + (float)rng.NextDouble() * 0.4f;
                case DifficultyTier.Intermediate: return 0.3f + (float)rng.NextDouble() * 0.4f;
                case DifficultyTier.Advanced: return 0.1f + (float)rng.NextDouble() * 0.3f;
                default: return (float)rng.NextDouble() * 0.2f;
            }
        }

        private static int MinStemsFor(DifficultyTier tier, Random rng)
        {
            switch (tier)
            {
                case DifficultyTier.Simple: return 3 + rng.Next(0, 4);
                case DifficultyTier.Intermediate: return 5 + rng.Next(0, 5);
                case DifficultyTier.Advanced: return 6 + rng.Next(0, 6);
                default: return 12 + rng.Next(0, 25);
            }
        }

        private static float PatienceFor(DifficultyTier tier, Random rng)
        {
            switch (tier)
            {
                case DifficultyTier.Simple: return 0.7f + (float)rng.NextDouble() * 0.3f;
                case DifficultyTier.Intermediate: return 0.45f + (float)rng.NextDouble() * 0.25f;
                case DifficultyTier.Advanced: return 0.3f + (float)rng.NextDouble() * 0.2f;
                default: return 0.15f + (float)rng.NextDouble() * 0.2f;
            }
        }

        private static List<FlowerColor> PickPreferredColors(Random rng, OccasionProfile occasion)
        {
            var pool = occasion.PreferredPalette.Count > 0
                ? occasion.PreferredPalette.ToList()
                : Enum.GetValues(typeof(FlowerColor)).Cast<FlowerColor>().ToList();

            int count = 1 + rng.Next(0, 2);
            var chosen = new List<FlowerColor>();
            for (int i = 0; i < count && pool.Count > 0; i++)
            {
                int index = rng.Next(pool.Count);
                chosen.Add(pool[index]);
                pool.RemoveAt(index);
            }
            return chosen;
        }

        private static List<string> PickSpeciesIds(Random rng, IReadOnlyList<FlowerSpecies> catalog, int count)
        {
            if (catalog.Count == 0) return new List<string>();
            var pool = catalog.ToList();
            var chosen = new List<string>();
            for (int i = 0; i < count && pool.Count > 0; i++)
            {
                int index = rng.Next(pool.Count);
                chosen.Add(pool[index].Id);
                pool.RemoveAt(index);
            }
            return chosen;
        }

        private static string BuildFlavorText(
            OccasionProfile occasion,
            float vagueness,
            List<FlowerColor> preferredColors,
            List<string> required,
            List<string> excluded,
            bool hasAllergy,
            IReadOnlyList<FlowerSpecies> catalog,
            int minStems)
        {
            var sb = new StringBuilder();

            if (vagueness > 0.6f)
            {
                sb.Append("I need something nice for ").Append(OccasionPhrase(occasion.Occasion)).Append('.');
            }
            else
            {
                sb.Append("I need flowers for ").Append(OccasionPhrase(occasion.Occasion)).Append(", around $")
                  .Append(occasion.TypicalBudgetMin).Append('-').Append(occasion.TypicalBudgetMax).Append('.');

                if (preferredColors.Count > 0)
                {
                    sb.Append(" Something in ").Append(string.Join(" or ", preferredColors)).Append('.');
                }

                if (required.Count > 0)
                {
                    string names = string.Join(", ", required.Select(id => catalog.FirstOrDefault(s => s.Id == id)?.DisplayName ?? id));
                    sb.Append(" Please include ").Append(names).Append('.');
                }

                if (excluded.Count > 0)
                {
                    string names = string.Join(", ", excluded.Select(id => catalog.FirstOrDefault(s => s.Id == id)?.DisplayName ?? id));
                    sb.Append(" No ").Append(names).Append(", please.");
                }

                sb.Append(" At least ").Append(minStems).Append(" stems.");
            }

            if (hasAllergy)
            {
                sb.Append(" Oh - I'm allergic to pollen, so nothing like lilies.");
            }

            return sb.ToString();
        }

        private static string OccasionPhrase(Customers.Occasion occasion)
        {
            switch (occasion)
            {
                case Customers.Occasion.Everyday: return "no particular reason, just because";
                case Customers.Occasion.Romance: return "a date";
                case Customers.Occasion.Apology: return "an apology";
                case Customers.Occasion.Sympathy: return "a funeral";
                case Customers.Occasion.Wedding: return "a wedding";
                case Customers.Occasion.Birthday: return "a birthday";
                case Customers.Occasion.Anniversary: return "an anniversary";
                case Customers.Occasion.Graduation: return "a graduation";
                case Customers.Occasion.GetWell: return "someone in the hospital";
                case Customers.Occasion.NewBaby: return "a new baby";
                case Customers.Occasion.Prom: return "prom";
                default: return "a special occasion";
            }
        }
    }
}
