using System.Collections.Generic;
using System.Linq;
using FloristGame.Core.Arrangements;
using FloristGame.Core.Economy;

namespace FloristGame.Gameplay
{
    // Mutable in-progress bouquet the player assembles in the workspace by
    // dragging flower chips in. Converts to an immutable Core Arrangement
    // (pulling freshness values from inventory) when the player serves it.
    public class ArrangementBuilder
    {
        private readonly Dictionary<string, int> _stemCounts = new Dictionary<string, int>();

        public WrappingType Wrapping { get; set; } = WrappingType.None;
        public VaseType Vase { get; set; } = VaseType.None;
        public ArrangementStyle Style { get; set; } = ArrangementStyle.Round;

        public IReadOnlyDictionary<string, int> StemCounts => _stemCounts;
        public int TotalStems => _stemCounts.Values.Sum();

        public void AddStem(string speciesId, int count = 1)
        {
            if (count <= 0) return;
            _stemCounts.TryGetValue(speciesId, out int existing);
            _stemCounts[speciesId] = existing + count;
        }

        public bool RemoveStem(string speciesId, int count = 1)
        {
            if (!_stemCounts.TryGetValue(speciesId, out int existing) || existing <= 0) return false;
            int newCount = existing - count;
            if (newCount <= 0)
            {
                _stemCounts.Remove(speciesId);
            }
            else
            {
                _stemCounts[speciesId] = newCount;
            }
            return true;
        }

        public void Clear() => _stemCounts.Clear();

        // Builds the final Arrangement and consumes the corresponding stock
        // from inventory. Returns false (making no changes) if inventory
        // can't cover everything currently in the builder.
        public bool TryFinalize(InventoryManager inventory, int day, out Arrangement arrangement)
        {
            arrangement = null;

            foreach (var kvp in _stemCounts)
            {
                if (inventory.GetAvailableStems(kvp.Key, day) < kvp.Value) return false;
            }

            var entries = new List<StemEntry>();
            foreach (var kvp in _stemCounts)
            {
                inventory.TryConsume(kvp.Key, kvp.Value, day, out float freshness);
                entries.Add(new StemEntry(kvp.Key, kvp.Value, freshness));
            }

            arrangement = new Arrangement(entries, Wrapping, Vase, Style);
            return true;
        }
    }
}
