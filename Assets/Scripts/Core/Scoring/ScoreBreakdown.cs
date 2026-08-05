using System.Collections.Generic;

namespace FloristGame.Core.Scoring
{
    // Result of scoring an Arrangement against a CustomerRequest. Sub-scores
    // are 0-100 and are surfaced individually so the UI can show *why* a
    // customer reacted the way they did (comedy relies on this).
    public sealed class ScoreBreakdown
    {
        public float ComplianceFit { get; set; }
        public float BudgetFit { get; set; }
        public float ColorMatch { get; set; }
        public float MeaningFit { get; set; }
        public float FreshnessQuality { get; set; }
        public float StyleMatch { get; set; }
        public float Overall { get; set; }
        public List<string> Notes { get; } = new List<string>();
    }
}
