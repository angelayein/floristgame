using System;
using System.Collections.Generic;
using System.Linq;

namespace FloristGame.Core.Economy
{
    // Reputation gates which customer types/difficulty tiers appear (see
    // RequestGenerator.TierForReputation) and which story/shop unlocks fire.
    public sealed class ReputationTracker
    {
        public float Reputation { get; private set; } = 50f;

        private readonly List<(float Threshold, string UnlockId)> _unlocks;
        private readonly HashSet<string> _granted = new HashSet<string>();

        public event Action<string> OnUnlockGranted;

        public ReputationTracker(IEnumerable<(float Threshold, string UnlockId)> unlocks)
        {
            _unlocks = (unlocks ?? Enumerable.Empty<(float, string)>()).OrderBy(u => u.Threshold).ToList();
        }

        public void ApplyTransactionResult(float satisfactionScore)
        {
            float delta = (satisfactionScore - 50f) / 10f;
            Reputation = Clamp(Reputation + delta, 0f, 100f);
            CheckUnlocks();
        }

        public void ApplyPenalty(float amount)
        {
            Reputation = Clamp(Reputation - amount, 0f, 100f);
        }

        private void CheckUnlocks()
        {
            foreach (var (threshold, unlockId) in _unlocks)
            {
                if (Reputation >= threshold && _granted.Add(unlockId))
                {
                    OnUnlockGranted?.Invoke(unlockId);
                }
            }
        }

        private static float Clamp(float value, float min, float max) => value < min ? min : (value > max ? max : value);
    }
}
