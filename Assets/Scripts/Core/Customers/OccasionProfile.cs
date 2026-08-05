using System.Collections.Generic;
using System.Linq;
using FloristGame.Core.Arrangements;
using FloristGame.Core.Flowers;

namespace FloristGame.Core.Customers
{
    // Rules that describe how a given occasion should be judged: what it
    // typically costs, which flower meanings fit or clash, and how forgiving
    // customers requesting it tend to be of mistakes.
    public sealed class OccasionProfile
    {
        public Occasion Occasion { get; }
        public string DisplayName { get; }
        public int TypicalBudgetMin { get; }
        public int TypicalBudgetMax { get; }
        public IReadOnlyList<FlowerMeaning> ExpectedMeanings { get; }
        public IReadOnlyList<FlowerMeaning> TabooMeanings { get; }
        public IReadOnlyList<FlowerColor> PreferredPalette { get; }
        public ArrangementStyle PreferredStyle { get; }

        // 0 = strict / unforgiving (weddings, funerals). 1 = very lenient ("just because").
        public float Forgiveness { get; }

        public OccasionProfile(
            Occasion occasion,
            string displayName,
            int typicalBudgetMin,
            int typicalBudgetMax,
            IEnumerable<FlowerMeaning> expectedMeanings,
            IEnumerable<FlowerMeaning> tabooMeanings,
            IEnumerable<FlowerColor> preferredPalette,
            ArrangementStyle preferredStyle,
            float forgiveness)
        {
            Occasion = occasion;
            DisplayName = displayName;
            TypicalBudgetMin = typicalBudgetMin;
            TypicalBudgetMax = typicalBudgetMax;
            ExpectedMeanings = (expectedMeanings ?? Enumerable.Empty<FlowerMeaning>()).ToList();
            TabooMeanings = (tabooMeanings ?? Enumerable.Empty<FlowerMeaning>()).ToList();
            PreferredPalette = (preferredPalette ?? Enumerable.Empty<FlowerColor>()).ToList();
            PreferredStyle = preferredStyle;
            Forgiveness = forgiveness;
        }
    }
}
