using System;
using FloristGame.Core.Customers;
using FloristGame.Core.Scoring;

namespace FloristGame.Core.Economy
{
    public enum HaggleOutcome
    {
        Accepted,
        CounterOffered,
        WalkedAway
    }

    public sealed class HaggleResult
    {
        public HaggleOutcome Outcome { get; }
        public float CounterPrice { get; }
        public string CustomerLine { get; }

        public HaggleResult(HaggleOutcome outcome, float counterPrice, string customerLine)
        {
            Outcome = outcome;
            CounterPrice = counterPrice;
            CustomerLine = customerLine;
        }
    }

    // Like the pizza game's price haggling: the customer has a perceived fair
    // value based on how happy the arrangement makes them and their stated
    // budget. Ask for too much and they'll counter or, if you push it and their
    // patience is low, walk out.
    public static class HaggleNegotiator
    {
        private static readonly string[] AcceptLines =
        {
            "That works for me!",
            "Deal.",
            "Fair enough, I'll take it."
        };

        private static readonly string[] CounterLines =
        {
            "That's a bit much... can you do better?",
            "Hmm, how about a little less?",
            "I was hoping to pay less than that."
        };

        private static readonly string[] WalkLines =
        {
            "That's way too expensive. Never mind.",
            "I'll look elsewhere, thanks.",
            "Forget it - not paying that."
        };

        public static float ComputePerceivedValue(CustomerRequest request, ScoreBreakdown score)
        {
            float budgetMid = (request.BudgetMin + request.BudgetMax) / 2f;
            float satisfactionFactor = 0.5f + (score.Overall / 100f) * 0.75f; // ranges roughly 0.5x .. 1.25x budget
            return budgetMid * satisfactionFactor;
        }

        public static HaggleResult Evaluate(CustomerRequest request, ScoreBreakdown score, float askedPrice, int round, Random rng)
        {
            float perceived = ComputePerceivedValue(request, score);
            float tolerance = 0.10f + request.Patience * 0.25f;
            float diffRatio = (askedPrice - perceived) / Math.Max(1f, perceived);

            if (diffRatio <= tolerance)
            {
                return new HaggleResult(HaggleOutcome.Accepted, askedPrice, PickLine(rng, AcceptLines));
            }

            float walkChance = Clamp01((diffRatio - tolerance) * (1f - request.Patience)) + round * 0.15f;
            if (rng.NextDouble() < walkChance)
            {
                return new HaggleResult(HaggleOutcome.WalkedAway, 0f, PickLine(rng, WalkLines));
            }

            float counter = perceived * (1f + tolerance * 0.5f);
            counter = Math.Min(counter, askedPrice * 0.95f);
            return new HaggleResult(HaggleOutcome.CounterOffered, (float)Math.Round(counter, 2), PickLine(rng, CounterLines));
        }

        private static string PickLine(Random rng, string[] lines) => lines[rng.Next(lines.Length)];
        private static float Clamp01(float value) => value < 0f ? 0f : (value > 1f ? 1f : value);
    }
}
