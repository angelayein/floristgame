using System.Collections.Generic;
using System.Linq;

namespace FloristGame.Core.Economy
{
    // Summary of stock that wilted past use when a day rolled over.
    public sealed class WasteReport
    {
        public IReadOnlyDictionary<string, int> WastedStemsBySpecies { get; }
        public int TotalWastedStems => WastedStemsBySpecies.Values.Sum();

        public WasteReport(IReadOnlyDictionary<string, int> wastedStemsBySpecies)
        {
            WastedStemsBySpecies = wastedStemsBySpecies;
        }

        public static WasteReport Empty { get; } = new WasteReport(new Dictionary<string, int>());
    }
}
