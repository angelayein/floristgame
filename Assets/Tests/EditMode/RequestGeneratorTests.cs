using System;
using System.Collections.Generic;
using NUnit.Framework;
using FloristGame.Core.Arrangements;
using FloristGame.Core.Customers;
using FloristGame.Core.Flowers;
using FloristGame.Core.Generation;

namespace FloristGame.Core.Tests
{
    public class RequestGeneratorTests
    {
        [TestCase(0f, DifficultyTier.Simple)]
        [TestCase(29.9f, DifficultyTier.Simple)]
        [TestCase(30f, DifficultyTier.Intermediate)]
        [TestCase(59.9f, DifficultyTier.Intermediate)]
        [TestCase(60f, DifficultyTier.Advanced)]
        [TestCase(84.9f, DifficultyTier.Advanced)]
        [TestCase(85f, DifficultyTier.VIP)]
        [TestCase(100f, DifficultyTier.VIP)]
        public void TierForReputation_MatchesExpectedBoundaries(float reputation, DifficultyTier expected)
        {
            Assert.AreEqual(expected, RequestGenerator.TierForReputation(reputation));
        }

        private static List<OccasionProfile> OneOccasion()
        {
            return new List<OccasionProfile>
            {
                new OccasionProfile(Occasion.Everyday, "Just Because", 10, 25,
                    new[] { FlowerMeaning.Friendship }, new[] { FlowerMeaning.Farewell },
                    new[] { FlowerColor.Red, FlowerColor.Pink },
                    ArrangementStyle.Round, 0.9f)
            };
        }

        private static List<FlowerSpecies> SmallCatalog()
        {
            return new List<FlowerSpecies>
            {
                new FlowerSpecies("rose_red", "Red Rose", FlowerColor.Red, FlowerMeaning.Romance,
                    new FlowerMeaning[0], new[] { Season.AllYear }, 3f, 0.12f, false),
                new FlowerSpecies("lily", "Lily", FlowerColor.White, FlowerMeaning.Sympathy,
                    new FlowerMeaning[0], new[] { Season.Summer }, 4f, 0.18f, true),
            };
        }

        [Test]
        public void Generate_ProducesInternallyConsistentRequests()
        {
            var occasions = OneOccasion();
            var catalog = SmallCatalog();
            var rng = new Random(42);

            foreach (DifficultyTier tier in Enum.GetValues(typeof(DifficultyTier)))
            {
                for (int i = 0; i < 20; i++)
                {
                    var request = RequestGenerator.Generate(rng, occasions, catalog, tier, "Test Customer");

                    Assert.LessOrEqual(request.BudgetMin, request.BudgetMax);
                    Assert.Greater(request.MinStems, 0);
                    Assert.That(request.Patience, Is.InRange(0f, 1f));
                    Assert.That(request.Vagueness, Is.InRange(0f, 1f));
                    Assert.IsFalse(string.IsNullOrEmpty(request.FlavorText));
                    Assert.AreEqual(occasions[0], request.Occasion);
                }
            }
        }

        [TestCase(DifficultyTier.Simple, 3, 6, 0.7f, 1.0f)]
        [TestCase(DifficultyTier.Intermediate, 5, 9, 0.45f, 0.7f)]
        [TestCase(DifficultyTier.Advanced, 6, 11, 0.3f, 0.5f)]
        [TestCase(DifficultyTier.VIP, 12, 36, 0.15f, 0.35f)]
        public void Generate_KeepsMinStemsAndPatienceWithinTierRanges(
            DifficultyTier tier, int minStemsFloor, int minStemsCeiling, float patienceFloor, float patienceCeiling)
        {
            var occasions = OneOccasion();
            var catalog = SmallCatalog();
            var rng = new Random(7);

            for (int i = 0; i < 30; i++)
            {
                var request = RequestGenerator.Generate(rng, occasions, catalog, tier, "Test Customer");

                Assert.That(request.MinStems, Is.InRange(minStemsFloor, minStemsCeiling));
                Assert.That(request.Patience, Is.InRange(patienceFloor, patienceCeiling));
            }
        }
    }
}
