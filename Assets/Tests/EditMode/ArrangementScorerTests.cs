using System.Collections.Generic;
using NUnit.Framework;
using FloristGame.Core.Arrangements;
using FloristGame.Core.Customers;
using FloristGame.Core.Flowers;
using FloristGame.Core.Scoring;

namespace FloristGame.Core.Tests
{
    public class ArrangementScorerTests
    {
        private FlowerSpecies _redRose;
        private FlowerSpecies _lily;
        private Dictionary<string, FlowerSpecies> _catalog;

        [SetUp]
        public void SetUp()
        {
            _redRose = new FlowerSpecies("rose_red", "Red Rose", FlowerColor.Red, FlowerMeaning.Romance,
                new FlowerMeaning[0], new[] { Season.AllYear }, 3f, 0.12f, false);

            _lily = new FlowerSpecies("lily_stargazer", "Stargazer Lily", FlowerColor.Pink, FlowerMeaning.Sympathy,
                new[] { FlowerMeaning.Purity }, new[] { Season.Summer }, 4f, 0.18f, true);

            _catalog = new Dictionary<string, FlowerSpecies>
            {
                { _redRose.Id, _redRose },
                { _lily.Id, _lily }
            };
        }

        private static OccasionProfile RomanceOccasion()
        {
            return new OccasionProfile(
                Occasion.Romance, "Romance", 30, 70,
                new[] { FlowerMeaning.Romance },
                new[] { FlowerMeaning.Jealousy, FlowerMeaning.Farewell, FlowerMeaning.Sympathy },
                new[] { FlowerColor.Red, FlowerColor.Pink },
                ArrangementStyle.Round, 0.2f);
        }

        [Test]
        public void PerfectArrangement_ScoresNearlyPerfect()
        {
            var occasion = RomanceOccasion();
            var request = new CustomerRequest("Alex", occasion, 30, 70, new[] { FlowerColor.Red },
                new string[0], new string[0], false, ArrangementStyle.Round, 6, 0.5f, 0.1f, "flavor");

            var arrangement = new Arrangement(
                new[] { new StemEntry(_redRose.Id, 6, 1f) },
                WrappingType.Ribbon, VaseType.Glass, ArrangementStyle.Round);

            var breakdown = ArrangementScorer.Score(request, arrangement, _catalog, 50f);

            Assert.That(breakdown.Overall, Is.EqualTo(100f).Within(0.5f));
        }

        [Test]
        public void AllergySensitiveCustomer_PenalizesHighPollenFlower()
        {
            var occasion = RomanceOccasion();
            var request = new CustomerRequest("Sam", occasion, 30, 70, new FlowerColor[0],
                new string[0], new string[0], true, null, 3, 0.5f, 0.5f, "flavor");

            var arrangement = new Arrangement(
                new[] { new StemEntry(_lily.Id, 6, 1f) },
                WrappingType.Kraft, VaseType.Basic, ArrangementStyle.Round);

            var breakdown = ArrangementScorer.Score(request, arrangement, _catalog, 50f);

            Assert.That(breakdown.ComplianceFit, Is.EqualTo(40f).Within(0.01f));
            Assert.That(breakdown.Notes.Exists(n => n.Contains("allergy")));
        }

        [Test]
        public void TabooMeaning_TanksMeaningFitAndExplainsWhy()
        {
            var occasion = RomanceOccasion();
            var request = new CustomerRequest("Jamie", occasion, 30, 70, new FlowerColor[0],
                new string[0], new string[0], false, null, 3, 0.5f, 0.5f, "flavor");

            var arrangement = new Arrangement(
                new[] { new StemEntry(_lily.Id, 6, 1f) },
                WrappingType.Kraft, VaseType.Basic, ArrangementStyle.Round);

            var breakdown = ArrangementScorer.Score(request, arrangement, _catalog, 50f);

            Assert.That(breakdown.MeaningFit, Is.EqualTo(10f).Within(0.01f));
            Assert.That(breakdown.Notes.Exists(n => n.Contains(_lily.DisplayName)));
        }

        [Test]
        public void OverBudget_LowersBudgetFitProportionally()
        {
            var occasion = RomanceOccasion();
            var request = new CustomerRequest("Riley", occasion, 30, 70, new FlowerColor[0],
                new string[0], new string[0], false, null, 3, 0.5f, 0.5f, "flavor");

            var arrangement = new Arrangement(
                new[] { new StemEntry(_redRose.Id, 6, 1f) },
                WrappingType.Kraft, VaseType.Basic, ArrangementStyle.Round);

            // 70 (max budget) -> charging 140 is 100% over: overBy = 1.0 -> 100 - 150 = -50 -> clamped to 0.
            var breakdown = ArrangementScorer.Score(request, arrangement, _catalog, 140f);

            Assert.That(breakdown.BudgetFit, Is.EqualTo(0f).Within(0.01f));
            Assert.That(breakdown.Notes.Exists(n => n.Contains("budget")));
        }

        [Test]
        public void BelowMinimumStems_PenalizesCompliance()
        {
            var occasion = RomanceOccasion();
            var request = new CustomerRequest("Drew", occasion, 30, 70, new FlowerColor[0],
                new string[0], new string[0], false, null, 10, 0.5f, 0.5f, "flavor");

            var arrangement = new Arrangement(
                new[] { new StemEntry(_redRose.Id, 3, 1f) },
                WrappingType.Kraft, VaseType.Basic, ArrangementStyle.Round);

            var breakdown = ArrangementScorer.Score(request, arrangement, _catalog, 50f);

            Assert.That(breakdown.ComplianceFit, Is.EqualTo(85f).Within(0.01f));
        }

        [Test]
        public void MissingRequiredSpecies_PenalizesCompliance()
        {
            var occasion = RomanceOccasion();
            var request = new CustomerRequest("Morgan", occasion, 30, 70, new FlowerColor[0],
                new[] { _redRose.Id }, new string[0], false, null, 3, 0.5f, 0.5f, "flavor");

            var arrangement = new Arrangement(
                new[] { new StemEntry(_lily.Id, 5, 1f) },
                WrappingType.Kraft, VaseType.Basic, ArrangementStyle.Round);

            var breakdown = ArrangementScorer.Score(request, arrangement, _catalog, 50f);

            Assert.That(breakdown.ComplianceFit, Is.EqualTo(75f).Within(0.01f));
        }

        [Test]
        public void WiltedFlowers_LowerFreshnessQuality()
        {
            var occasion = RomanceOccasion();
            var request = new CustomerRequest("Casey", occasion, 30, 70, new FlowerColor[0],
                new string[0], new string[0], false, null, 3, 0.5f, 0.5f, "flavor");

            var arrangement = new Arrangement(
                new[] { new StemEntry(_redRose.Id, 6, 0.2f) },
                WrappingType.Kraft, VaseType.Basic, ArrangementStyle.Round);

            var breakdown = ArrangementScorer.Score(request, arrangement, _catalog, 50f);

            Assert.That(breakdown.FreshnessQuality, Is.EqualTo(20f).Within(0.01f));
            Assert.That(breakdown.Notes.Exists(n => n.Contains("wilted")));
        }
    }
}
