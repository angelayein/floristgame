using System.Collections.Generic;
using System.Linq;
using FloristGame.Core.Arrangements;
using FloristGame.Core.Flowers;

namespace FloristGame.Core.Customers
{
    // A single customer's order, generated procedurally by RequestGenerator
    // and scored against whatever Arrangement the player builds for them.
    public sealed class CustomerRequest
    {
        public string CustomerName { get; }
        public OccasionProfile Occasion { get; }
        public int BudgetMin { get; }
        public int BudgetMax { get; }
        public IReadOnlyList<FlowerColor> PreferredColors { get; }
        public IReadOnlyList<string> RequiredSpeciesIds { get; }
        public IReadOnlyList<string> ExcludedSpeciesIds { get; }
        public bool HasAllergy { get; }
        public ArrangementStyle? PreferredStyle { get; }
        public int MinStems { get; }

        // 0..1: how much haggling / price gap this customer will put up with before walking.
        public float Patience { get; }

        // 0 (very specific ask) .. 1 (vague, "something nice") - softens scoring strictness.
        public float Vagueness { get; }

        public string FlavorText { get; }

        public CustomerRequest(
            string customerName,
            OccasionProfile occasion,
            int budgetMin,
            int budgetMax,
            IEnumerable<FlowerColor> preferredColors,
            IEnumerable<string> requiredSpeciesIds,
            IEnumerable<string> excludedSpeciesIds,
            bool hasAllergy,
            ArrangementStyle? preferredStyle,
            int minStems,
            float patience,
            float vagueness,
            string flavorText)
        {
            CustomerName = customerName;
            Occasion = occasion;
            BudgetMin = budgetMin;
            BudgetMax = budgetMax;
            PreferredColors = (preferredColors ?? Enumerable.Empty<FlowerColor>()).ToList();
            RequiredSpeciesIds = (requiredSpeciesIds ?? Enumerable.Empty<string>()).ToList();
            ExcludedSpeciesIds = (excludedSpeciesIds ?? Enumerable.Empty<string>()).ToList();
            HasAllergy = hasAllergy;
            PreferredStyle = preferredStyle;
            MinStems = minStems;
            Patience = patience;
            Vagueness = vagueness;
            FlavorText = flavorText;
        }
    }
}
