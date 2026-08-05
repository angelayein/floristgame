using System;
using System.Collections.Generic;
using System.Linq;
using FloristGame.Core.Arrangements;
using FloristGame.Core.Customers;
using FloristGame.Core.Flowers;

namespace FloristGame.Core.Scoring
{
    // Pure scoring logic: given what the customer asked for and what the
    // player built and charged, produce a 0-100 satisfaction score plus a
    // breakdown explaining it. No engine dependency, no randomness -
    // deterministic and fully unit-testable.
    public static class ArrangementScorer
    {
        private const float WeightCompliance = 0.25f;
        private const float WeightBudget = 0.20f;
        private const float WeightColor = 0.15f;
        private const float WeightMeaning = 0.25f;
        private const float WeightFreshness = 0.10f;
        private const float WeightStyle = 0.05f;

        public static ScoreBreakdown Score(
            CustomerRequest request,
            Arrangement arrangement,
            IReadOnlyDictionary<string, FlowerSpecies> catalog,
            float chargedPrice)
        {
            var breakdown = new ScoreBreakdown();

            breakdown.ComplianceFit = ScoreCompliance(request, arrangement, catalog, breakdown.Notes);
            breakdown.BudgetFit = ScoreBudget(request, chargedPrice, breakdown.Notes);
            breakdown.ColorMatch = ScoreColor(request, arrangement, catalog, breakdown.Notes);
            breakdown.MeaningFit = ScoreMeaning(request, arrangement, catalog, breakdown.Notes);
            breakdown.FreshnessQuality = ScoreFreshness(arrangement, breakdown.Notes);
            breakdown.StyleMatch = ScoreStyle(request, arrangement);

            float raw =
                breakdown.ComplianceFit * WeightCompliance +
                breakdown.BudgetFit * WeightBudget +
                breakdown.ColorMatch * WeightColor +
                breakdown.MeaningFit * WeightMeaning +
                breakdown.FreshnessQuality * WeightFreshness +
                breakdown.StyleMatch * WeightStyle;

            // Vague, forgiving requests soften the gap between the raw score and a
            // perfect 100 - a customer who just said "something nice" can't be too
            // upset about color choices they never specified.
            float lenience = Clamp01(request.Occasion.Forgiveness * 0.5f + request.Vagueness * 0.5f);
            float final = raw + (100f - raw) * lenience * 0.3f;

            breakdown.Overall = Clamp(final, 0f, 100f);
            return breakdown;
        }

        private static float ScoreCompliance(
            CustomerRequest request,
            Arrangement arrangement,
            IReadOnlyDictionary<string, FlowerSpecies> catalog,
            List<string> notes)
        {
            float score = 100f;

            if (request.HasAllergy)
            {
                bool hasAllergen = arrangement.Stems.Any(s => catalog.TryGetValue(s.SpeciesId, out var sp) && sp.IsHighPollen);
                if (hasAllergen)
                {
                    score -= 60f;
                    notes.Add("Included a high-pollen flower despite the customer's allergy.");
                }
            }

            foreach (var requiredId in request.RequiredSpeciesIds)
            {
                if (!arrangement.Stems.Any(s => s.SpeciesId == requiredId))
                {
                    score -= 25f;
                    string name = catalog.TryGetValue(requiredId, out var sp) ? sp.DisplayName : requiredId;
                    notes.Add($"Missing requested flower: {name}.");
                }
            }

            foreach (var excludedId in request.ExcludedSpeciesIds)
            {
                if (arrangement.Stems.Any(s => s.SpeciesId == excludedId))
                {
                    score -= 25f;
                    string name = catalog.TryGetValue(excludedId, out var sp) ? sp.DisplayName : excludedId;
                    notes.Add($"Included a flower the customer explicitly didn't want: {name}.");
                }
            }

            if (arrangement.TotalStems < request.MinStems)
            {
                score -= 15f;
                notes.Add("The arrangement is smaller than requested.");
            }

            return Clamp(score, 0f, 100f);
        }

        private static float ScoreBudget(CustomerRequest request, float chargedPrice, List<string> notes)
        {
            if (chargedPrice <= request.BudgetMax && chargedPrice >= request.BudgetMin)
            {
                return 100f;
            }

            if (chargedPrice > request.BudgetMax)
            {
                float overBy = (chargedPrice - request.BudgetMax) / Math.Max(1f, request.BudgetMax);
                notes.Add("Priced above the customer's stated budget.");
                return Clamp(100f - overBy * 150f, 0f, 100f);
            }

            float underBy = (request.BudgetMin - chargedPrice) / Math.Max(1f, request.BudgetMin);
            if (underBy > 0.5f)
            {
                notes.Add("Suspiciously cheap for the occasion.");
                return Clamp(100f - underBy * 50f, 0f, 100f);
            }

            return 100f;
        }

        private static float ScoreColor(
            CustomerRequest request,
            Arrangement arrangement,
            IReadOnlyDictionary<string, FlowerSpecies> catalog,
            List<string> notes)
        {
            if (request.PreferredColors.Count == 0) return 100f;
            if (arrangement.TotalStems == 0)
            {
                notes.Add("The arrangement is empty.");
                return 0f;
            }

            int matchingStems = arrangement.Stems
                .Where(s => catalog.TryGetValue(s.SpeciesId, out var sp) && request.PreferredColors.Contains(sp.Color))
                .Sum(s => s.Count);

            float ratio = matchingStems / (float)arrangement.TotalStems;
            if (ratio < 0.5f)
            {
                notes.Add("Doesn't match the color palette the customer wanted.");
            }

            return Clamp(ratio * 100f, 0f, 100f);
        }

        private static float ScoreMeaning(
            CustomerRequest request,
            Arrangement arrangement,
            IReadOnlyDictionary<string, FlowerSpecies> catalog,
            List<string> notes)
        {
            var occasion = request.Occasion;
            if (arrangement.TotalStems == 0) return 0f;

            float score = 100f;
            bool anyExpectedMeaning = occasion.ExpectedMeanings.Count == 0;

            foreach (var stem in arrangement.Stems)
            {
                if (!catalog.TryGetValue(stem.SpeciesId, out var species)) continue;
                float weight = stem.Count / (float)arrangement.TotalStems;

                if (occasion.TabooMeanings.Contains(species.PrimaryMeaning))
                {
                    score -= 70f * weight;
                    notes.Add($"{species.DisplayName} means \"{species.PrimaryMeaning}\" - a jarring choice for {occasion.DisplayName}.");
                }
                else if (occasion.ExpectedMeanings.Contains(species.PrimaryMeaning) || species.SecondaryMeanings.Any(m => occasion.ExpectedMeanings.Contains(m)))
                {
                    anyExpectedMeaning = true;
                }
            }

            if (!anyExpectedMeaning)
            {
                score -= 20f;
                notes.Add("None of the flowers really carry the meaning this occasion calls for.");
            }

            return Clamp(score, 0f, 100f);
        }

        private static float ScoreFreshness(Arrangement arrangement, List<string> notes)
        {
            if (arrangement.TotalStems == 0) return 0f;

            float weighted = arrangement.Stems.Sum(s => s.FreshnessAtUse * s.Count) / arrangement.TotalStems;
            if (weighted < 0.4f)
            {
                notes.Add("Some of these flowers are visibly wilted.");
            }

            return Clamp(weighted * 100f, 0f, 100f);
        }

        private static float ScoreStyle(CustomerRequest request, Arrangement arrangement)
        {
            if (request.PreferredStyle == null) return 100f;
            return request.PreferredStyle.Value == arrangement.Style ? 100f : 50f;
        }

        private static float Clamp(float value, float min, float max) => value < min ? min : (value > max ? max : value);
        private static float Clamp01(float value) => Clamp(value, 0f, 1f);
    }
}
