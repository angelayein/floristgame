using System;
using NUnit.Framework;
using FloristGame.Core.Arrangements;
using FloristGame.Core.Customers;
using FloristGame.Core.Economy;
using FloristGame.Core.Flowers;
using FloristGame.Core.Scoring;

namespace FloristGame.Core.Tests
{
    public class HaggleNegotiatorTests
    {
        private static CustomerRequest MakeRequest(float patience)
        {
            var occasion = new OccasionProfile(
                Occasion.Everyday, "Just Because", 30, 70,
                new FlowerMeaning[0], new FlowerMeaning[0], new FlowerColor[0],
                ArrangementStyle.Round, 0.5f);

            return new CustomerRequest("Taylor", occasion, 30, 70, new FlowerColor[0],
                new string[0], new string[0], false, null, 3, patience, 0.5f, "flavor");
        }

        [Test]
        public void PriceAtPerceivedValue_IsAccepted()
        {
            var request = MakeRequest(0.5f);
            var breakdown = new ScoreBreakdown { Overall = 100f };
            // budgetMid = 50, satisfactionFactor = 1.25 -> perceived = 62.5
            float perceived = HaggleNegotiator.ComputePerceivedValue(request, breakdown);

            var result = HaggleNegotiator.Evaluate(request, breakdown, perceived, 0, new Random(1));

            Assert.AreEqual(HaggleOutcome.Accepted, result.Outcome);
        }

        [Test]
        public void ModerateGapWithHighPatience_AlwaysCounters()
        {
            // patience = 1.0 makes the walk-chance term exactly 0 on round 0,
            // so this is guaranteed to counter regardless of RNG draw.
            var request = MakeRequest(1.0f);
            var breakdown = new ScoreBreakdown { Overall = 80f };
            float perceived = HaggleNegotiator.ComputePerceivedValue(request, breakdown);
            float askedPrice = perceived * 2f;

            var result = HaggleNegotiator.Evaluate(request, breakdown, askedPrice, 0, new Random(1));

            Assert.AreEqual(HaggleOutcome.CounterOffered, result.Outcome);
            Assert.Less(result.CounterPrice, askedPrice);
            Assert.Greater(result.CounterPrice, 0f);
        }

        [Test]
        public void HugeGapWithNoPatience_AlwaysWalksAway()
        {
            // patience = 0 and a massive overask push walk-chance to >= 1.0,
            // which is guaranteed to trigger regardless of RNG draw since
            // Random.NextDouble() is always < 1.0.
            var request = MakeRequest(0f);
            var breakdown = new ScoreBreakdown { Overall = 50f };
            float perceived = HaggleNegotiator.ComputePerceivedValue(request, breakdown);
            float askedPrice = perceived * 5f;

            var result = HaggleNegotiator.Evaluate(request, breakdown, askedPrice, 0, new Random(1));

            Assert.AreEqual(HaggleOutcome.WalkedAway, result.Outcome);
        }
    }
}
