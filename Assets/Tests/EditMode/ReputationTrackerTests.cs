using System.Collections.Generic;
using NUnit.Framework;
using FloristGame.Core.Economy;

namespace FloristGame.Core.Tests
{
    public class ReputationTrackerTests
    {
        [Test]
        public void ApplyTransactionResult_MovesReputationTowardScore()
        {
            var tracker = new ReputationTracker(new List<(float, string)>());

            tracker.ApplyTransactionResult(100f); // delta = (100-50)/10 = +5

            Assert.That(tracker.Reputation, Is.EqualTo(55f).Within(0.01f));
        }

        [Test]
        public void ApplyPenalty_ClampsAtZero()
        {
            var tracker = new ReputationTracker(new List<(float, string)>());

            tracker.ApplyPenalty(200f);

            Assert.AreEqual(0f, tracker.Reputation);
        }

        [Test]
        public void UnlockFiresExactlyOnceWhenThresholdCrossed()
        {
            var unlocks = new List<(float, string)> { (60f, "advanced_customers") };
            var tracker = new ReputationTracker(unlocks);
            var fired = new List<string>();
            tracker.OnUnlockGranted += id => fired.Add(id);

            tracker.ApplyTransactionResult(100f); // 50 -> 55, below threshold
            Assert.AreEqual(0, fired.Count);

            tracker.ApplyTransactionResult(100f); // 55 -> 60, crosses threshold
            Assert.AreEqual(1, fired.Count);
            Assert.AreEqual("advanced_customers", fired[0]);

            tracker.ApplyTransactionResult(100f); // 60 -> 65, already granted
            Assert.AreEqual(1, fired.Count);
        }
    }
}
