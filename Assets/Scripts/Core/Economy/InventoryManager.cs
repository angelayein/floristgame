using System.Collections.Generic;
using System.Linq;
using FloristGame.Core.Flowers;

namespace FloristGame.Core.Economy
{
    // Tracks stock per species as a FIFO queue of purchase batches, so freshness
    // and wilting can be computed per-batch. This is the soft inventory-pressure
    // system: over-order and it wilts on the shelf; under-order and you can't
    // fill requests.
    public sealed class InventoryManager
    {
        private readonly Dictionary<string, List<InventoryBatch>> _batches = new Dictionary<string, List<InventoryBatch>>();

        public void AddStock(FlowerSpecies species, int stems, int day)
        {
            if (stems <= 0) return;

            if (!_batches.TryGetValue(species.Id, out var list))
            {
                list = new List<InventoryBatch>();
                _batches[species.Id] = list;
            }

            list.Add(new InventoryBatch(species, stems, day));
            list.Sort((a, b) => a.PurchaseDay.CompareTo(b.PurchaseDay));
        }

        public int GetAvailableStems(string speciesId, int day)
        {
            if (!_batches.TryGetValue(speciesId, out var list)) return 0;
            return list.Where(b => !b.IsWiltedOnDay(day)).Sum(b => b.Stems);
        }

        public float GetAverageFreshness(string speciesId, int day)
        {
            if (!_batches.TryGetValue(speciesId, out var list)) return 0f;

            int totalStems = 0;
            float weightedSum = 0f;
            foreach (var batch in list)
            {
                if (batch.IsWiltedOnDay(day)) continue;
                totalStems += batch.Stems;
                weightedSum += batch.FreshnessOnDay(day) * batch.Stems;
            }

            return totalStems == 0 ? 0f : weightedSum / totalStems;
        }

        // Consumes stems oldest-first (FIFO). Returns false without side effects
        // if there isn't enough non-wilted stock to fulfil the request.
        public bool TryConsume(string speciesId, int count, int day, out float averageFreshnessUsed)
        {
            averageFreshnessUsed = 0f;
            if (count <= 0) return true;
            if (GetAvailableStems(speciesId, day) < count) return false;

            var list = _batches[speciesId];
            int remaining = count;
            float weightedSum = 0f;

            foreach (var batch in list)
            {
                if (remaining <= 0) break;
                if (batch.IsWiltedOnDay(day)) continue;

                int take = System.Math.Min(remaining, batch.Stems);
                weightedSum += batch.FreshnessOnDay(day) * take;
                batch.Stems -= take;
                remaining -= take;
            }

            list.RemoveAll(b => b.Stems <= 0);
            averageFreshnessUsed = weightedSum / count;
            return true;
        }

        // Call once per day advance. Strips out fully wilted batches and reports
        // how much stock was wasted so the player feels the cost of over-ordering.
        public WasteReport AdvanceDay(int newDay)
        {
            var wasted = new Dictionary<string, int>();

            foreach (var kvp in _batches)
            {
                var list = kvp.Value;
                int wastedStems = list.Where(b => b.IsWiltedOnDay(newDay)).Sum(b => b.Stems);
                if (wastedStems > 0)
                {
                    wasted[kvp.Key] = wastedStems;
                }

                list.RemoveAll(b => b.IsWiltedOnDay(newDay));
            }

            return new WasteReport(wasted);
        }
    }
}
