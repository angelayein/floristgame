using System.Collections.Generic;
using UnityEngine;
using FloristGame.Core.Arrangements;
using FloristGame.Core.Customers;
using FloristGame.Core.Flowers;

namespace FloristGame.Data
{
    [CreateAssetMenu(fileName = "Occasion_", menuName = "Florist Game/Occasion")]
    public class OccasionDefinition : ScriptableObject
    {
        public Occasion occasion;
        public string displayName;
        public int typicalBudgetMin = 20;
        public int typicalBudgetMax = 50;
        public List<FlowerMeaning> expectedMeanings = new List<FlowerMeaning>();
        public List<FlowerMeaning> tabooMeanings = new List<FlowerMeaning>();
        public List<FlowerColor> preferredPalette = new List<FlowerColor>();
        public ArrangementStyle preferredStyle = ArrangementStyle.Round;

        [Range(0f, 1f)]
        [Tooltip("0 = strict/unforgiving (weddings, funerals). 1 = very lenient (just because).")]
        public float forgiveness = 0.5f;

        public OccasionProfile ToCoreProfile()
        {
            return new OccasionProfile(occasion, displayName, typicalBudgetMin, typicalBudgetMax, expectedMeanings, tabooMeanings, preferredPalette, preferredStyle, forgiveness);
        }
    }
}
