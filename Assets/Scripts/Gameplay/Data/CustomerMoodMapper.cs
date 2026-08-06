using FloristGame.Core.Scoring;

namespace FloristGame.Data
{
    // Turns an ArrangementScorer result into a portrait mood. Kept separate
    // from Core - this is presentation ("how do we react on screen"), not a
    // gameplay rule, so it doesn't belong in the engine-independent scoring
    // logic itself.
    public static class CustomerMoodMapper
    {
        public static CustomerMood FromScore(ScoreBreakdown breakdown)
        {
            if (breakdown.Overall >= 85f) return CustomerMood.Delighted;
            if (breakdown.Overall >= 60f) return CustomerMood.Happy;

            // A bad meaning/compliance sub-score (wrong flower symbolism, an
            // allergy violation, a flatly ignored request) reads as offense
            // rather than mere disappointment, regardless of the overall total.
            bool tookOffense = breakdown.MeaningFit < 30f || breakdown.ComplianceFit < 30f;
            if (tookOffense) return CustomerMood.Offended;

            return breakdown.Overall >= 35f ? CustomerMood.Confused : CustomerMood.Disappointed;
        }
    }
}
