using System.Collections.Generic;
using System.Linq;

namespace FloristGame.Core.Arrangements
{
    // The finished bouquet the player builds in the workspace, ready to be
    // scored against a CustomerRequest.
    public sealed class Arrangement
    {
        public IReadOnlyList<StemEntry> Stems { get; }
        public WrappingType Wrapping { get; }
        public VaseType Vase { get; }
        public ArrangementStyle Style { get; }

        public int TotalStems => Stems.Sum(s => s.Count);

        public Arrangement(IEnumerable<StemEntry> stems, WrappingType wrapping, VaseType vase, ArrangementStyle style)
        {
            Stems = stems.Where(s => s.Count > 0).ToList();
            Wrapping = wrapping;
            Vase = vase;
            Style = style;
        }

        public static Arrangement Empty { get; } = new Arrangement(Enumerable.Empty<StemEntry>(), WrappingType.None, VaseType.None, ArrangementStyle.Round);
    }
}
