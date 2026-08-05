using NUnit.Framework;
using FloristGame.Core.Economy;
using FloristGame.Core.Flowers;

namespace FloristGame.Core.Tests
{
    public class InventoryManagerTests
    {
        private static FlowerSpecies MakeSpecies(float wiltRatePerDay)
        {
            return new FlowerSpecies("x", "X", FlowerColor.Red, FlowerMeaning.Romance,
                new FlowerMeaning[0], new[] { Season.AllYear }, 2f, wiltRatePerDay, false);
        }

        [Test]
        public void FreshStock_IsFullyAvailableOnPurchaseDay()
        {
            var inventory = new InventoryManager();
            var species = MakeSpecies(0.25f);

            inventory.AddStock(species, 10, day: 1);

            Assert.AreEqual(10, inventory.GetAvailableStems("x", 1));
        }

        [Test]
        public void StockWiltsFullyAfterEnoughDays()
        {
            var inventory = new InventoryManager();
            var species = MakeSpecies(0.25f); // loses 25%/day -> fully wilted after 4 days

            inventory.AddStock(species, 10, day: 1);

            Assert.AreEqual(10, inventory.GetAvailableStems("x", 4)); // 25% left, still usable
            Assert.AreEqual(0, inventory.GetAvailableStems("x", 5));  // fully wilted
        }

        [Test]
        public void TryConsume_UsesOldestBatchesFirst()
        {
            var inventory = new InventoryManager();
            var species = MakeSpecies(0.25f);

            inventory.AddStock(species, 5, day: 1); // 50% fresh by day 3
            inventory.AddStock(species, 5, day: 3); // 100% fresh on day 3

            bool ok = inventory.TryConsume("x", 7, day: 3, out float averageFreshness);

            Assert.IsTrue(ok);
            // 5 stems at 0.5 freshness + 2 stems at 1.0 freshness, averaged over 7
            Assert.That(averageFreshness, Is.EqualTo((5 * 0.5f + 2 * 1.0f) / 7f).Within(0.001f));
            Assert.AreEqual(3, inventory.GetAvailableStems("x", 3));
        }

        [Test]
        public void TryConsume_FailsWithoutSideEffectsWhenNotEnoughStock()
        {
            var inventory = new InventoryManager();
            var species = MakeSpecies(0.1f);
            inventory.AddStock(species, 3, day: 1);

            bool ok = inventory.TryConsume("x", 5, day: 1, out float averageFreshness);

            Assert.IsFalse(ok);
            Assert.AreEqual(0f, averageFreshness);
            Assert.AreEqual(3, inventory.GetAvailableStems("x", 1));
        }

        [Test]
        public void AdvanceDay_ReportsAndRemovesWastedStock()
        {
            var inventory = new InventoryManager();
            var species = MakeSpecies(0.5f); // fully wilted after 2 days
            inventory.AddStock(species, 4, day: 1);

            var report = inventory.AdvanceDay(3);

            Assert.AreEqual(4, report.TotalWastedStems);
            Assert.AreEqual(0, inventory.GetAvailableStems("x", 3));
        }
    }
}
