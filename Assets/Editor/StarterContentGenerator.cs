using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using FloristGame.Core.Arrangements;
using FloristGame.Core.Customers;
using FloristGame.Core.Flowers;
using FloristGame.Data;

namespace FloristGame.EditorTools
{
    // Generates a starter set of FlowerDefinition / OccasionDefinition /
    // WrappingDefinition / VaseDefinition assets so the project has real,
    // playable content without hand-authoring 30+ ScriptableObjects in the
    // inspector. Safe to re-run - existing assets at the target paths are
    // left untouched.
    public static class StarterContentGenerator
    {
        private const string FlowersFolder = "Assets/Data/Generated/Flowers";
        private const string OccasionsFolder = "Assets/Data/Generated/Occasions";
        private const string WrappingsFolder = "Assets/Data/Generated/Wrappings";
        private const string VasesFolder = "Assets/Data/Generated/Vases";

        [MenuItem("Florist Game/Generate Starter Content")]
        public static void Generate()
        {
            EnsureFolder(FlowersFolder);
            EnsureFolder(OccasionsFolder);
            EnsureFolder(WrappingsFolder);
            EnsureFolder(VasesFolder);

            int created = 0;
            created += GenerateFlowers();
            created += GenerateOccasions();
            created += GenerateWrappings();
            created += GenerateVases();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Florist Game starter content: created {created} new asset(s) (existing assets were left untouched).");
        }

        private static int GenerateFlowers()
        {
            var seeds = new List<FlowerSeed>
            {
                new FlowerSeed("rose_red", "Red Rose", FlowerColor.Red, FlowerMeaning.Romance, new FlowerMeaning[0], new[] { Season.AllYear }, 3f, 0.12f, false),
                new FlowerSeed("rose_yellow", "Yellow Rose", FlowerColor.Yellow, FlowerMeaning.Friendship, new[] { FlowerMeaning.Jealousy }, new[] { Season.AllYear }, 3f, 0.12f, false),
                new FlowerSeed("rose_white", "White Rose", FlowerColor.White, FlowerMeaning.Purity, new[] { FlowerMeaning.NewBeginnings }, new[] { Season.AllYear }, 3f, 0.12f, false),
                new FlowerSeed("lily_stargazer", "Stargazer Lily", FlowerColor.Pink, FlowerMeaning.Sympathy, new[] { FlowerMeaning.Purity }, new[] { Season.Summer, Season.Fall }, 4f, 0.18f, true),
                new FlowerSeed("daisy", "Daisy", FlowerColor.White, FlowerMeaning.Friendship, new FlowerMeaning[0], new[] { Season.Spring, Season.Summer }, 1.5f, 0.20f, false),
                new FlowerSeed("sunflower", "Sunflower", FlowerColor.Yellow, FlowerMeaning.Gratitude, new FlowerMeaning[0], new[] { Season.Summer }, 2.5f, 0.15f, false),
                new FlowerSeed("tulip", "Tulip", FlowerColor.Red, FlowerMeaning.NewBeginnings, new FlowerMeaning[0], new[] { Season.Spring }, 2f, 0.20f, false),
                new FlowerSeed("carnation", "Carnation", FlowerColor.Pink, FlowerMeaning.Gratitude, new[] { FlowerMeaning.Sympathy }, new[] { Season.AllYear }, 1.5f, 0.10f, false),
                new FlowerSeed("orchid", "Orchid", FlowerColor.Purple, FlowerMeaning.Romance, new FlowerMeaning[0], new[] { Season.AllYear }, 6f, 0.08f, false),
                new FlowerSeed("babys_breath", "Baby's Breath", FlowerColor.White, FlowerMeaning.Purity, new FlowerMeaning[0], new[] { Season.AllYear }, 0.5f, 0.05f, false),
                new FlowerSeed("chrysanthemum", "Chrysanthemum", FlowerColor.Orange, FlowerMeaning.Farewell, new[] { FlowerMeaning.Sympathy }, new[] { Season.Fall }, 2f, 0.12f, false),
                new FlowerSeed("marigold", "Marigold", FlowerColor.Orange, FlowerMeaning.Jealousy, new FlowerMeaning[0], new[] { Season.Fall }, 1.5f, 0.15f, false),
                new FlowerSeed("peony", "Peony", FlowerColor.Pink, FlowerMeaning.Romance, new[] { FlowerMeaning.Celebration }, new[] { Season.Spring }, 4f, 0.15f, false),
                new FlowerSeed("iris", "Iris", FlowerColor.Blue, FlowerMeaning.NewBeginnings, new[] { FlowerMeaning.Gratitude }, new[] { Season.Summer }, 3f, 0.13f, false),
            };

            int created = 0;
            foreach (var seed in seeds)
            {
                string path = $"{FlowersFolder}/Flower_{seed.Id}.asset";
                if (AssetDatabase.LoadAssetAtPath<FlowerDefinition>(path) != null) continue;

                var asset = ScriptableObject.CreateInstance<FlowerDefinition>();
                asset.id = seed.Id;
                asset.displayName = seed.DisplayName;
                asset.color = seed.Color;
                asset.primaryMeaning = seed.PrimaryMeaning;
                asset.secondaryMeanings = new List<FlowerMeaning>(seed.SecondaryMeanings);
                asset.inSeason = new List<Season>(seed.InSeason);
                asset.baseStemPrice = seed.BaseStemPrice;
                asset.wiltRatePerDay = seed.WiltRatePerDay;
                asset.isHighPollen = seed.IsHighPollen;

                AssetDatabase.CreateAsset(asset, path);
                created++;
            }
            return created;
        }

        private static int GenerateOccasions()
        {
            var seeds = new List<OccasionSeed>
            {
                new OccasionSeed(Occasion.Everyday, "Just Because", 10, 25, new[] { FlowerMeaning.Friendship, FlowerMeaning.Gratitude }, new[] { FlowerMeaning.Farewell }, new FlowerColor[0], ArrangementStyle.Round, 0.9f),
                new OccasionSeed(Occasion.Romance, "Romance", 30, 70, new[] { FlowerMeaning.Romance }, new[] { FlowerMeaning.Jealousy, FlowerMeaning.Farewell, FlowerMeaning.Sympathy }, new[] { FlowerColor.Red, FlowerColor.Pink }, ArrangementStyle.Round, 0.2f),
                new OccasionSeed(Occasion.Apology, "Apology", 25, 50, new[] { FlowerMeaning.Apology, FlowerMeaning.Gratitude }, new[] { FlowerMeaning.Jealousy }, new[] { FlowerColor.White, FlowerColor.Pink, FlowerColor.Purple }, ArrangementStyle.Round, 0.3f),
                new OccasionSeed(Occasion.Sympathy, "Sympathy / Funeral", 40, 90, new[] { FlowerMeaning.Sympathy, FlowerMeaning.Farewell }, new[] { FlowerMeaning.Romance, FlowerMeaning.Jealousy, FlowerMeaning.Celebration }, new[] { FlowerColor.White }, ArrangementStyle.Cascade, 0.1f),
                new OccasionSeed(Occasion.Wedding, "Wedding", 80, 300, new[] { FlowerMeaning.Romance, FlowerMeaning.Celebration, FlowerMeaning.NewBeginnings, FlowerMeaning.Purity }, new[] { FlowerMeaning.Farewell, FlowerMeaning.Jealousy, FlowerMeaning.Sympathy }, new[] { FlowerColor.White, FlowerColor.Pink }, ArrangementStyle.Cascade, 0.15f),
                new OccasionSeed(Occasion.Birthday, "Birthday", 25, 60, new[] { FlowerMeaning.Celebration, FlowerMeaning.Friendship }, new[] { FlowerMeaning.Farewell, FlowerMeaning.Sympathy }, new FlowerColor[0], ArrangementStyle.Round, 0.6f),
                new OccasionSeed(Occasion.Anniversary, "Anniversary", 40, 100, new[] { FlowerMeaning.Romance, FlowerMeaning.Celebration }, new[] { FlowerMeaning.Farewell, FlowerMeaning.Jealousy }, new[] { FlowerColor.Red, FlowerColor.Purple }, ArrangementStyle.Round, 0.35f),
                new OccasionSeed(Occasion.Graduation, "Graduation", 25, 60, new[] { FlowerMeaning.Celebration, FlowerMeaning.NewBeginnings }, new[] { FlowerMeaning.Farewell, FlowerMeaning.Sympathy }, new FlowerColor[0], ArrangementStyle.Round, 0.6f),
                new OccasionSeed(Occasion.GetWell, "Get Well Soon", 20, 45, new[] { FlowerMeaning.Friendship, FlowerMeaning.Gratitude }, new[] { FlowerMeaning.Farewell, FlowerMeaning.Sympathy, FlowerMeaning.Jealousy }, new FlowerColor[0], ArrangementStyle.Round, 0.55f),
                new OccasionSeed(Occasion.NewBaby, "New Baby", 30, 70, new[] { FlowerMeaning.Celebration, FlowerMeaning.NewBeginnings, FlowerMeaning.Purity }, new[] { FlowerMeaning.Farewell, FlowerMeaning.Jealousy }, new[] { FlowerColor.White, FlowerColor.Pink, FlowerColor.Yellow }, ArrangementStyle.Round, 0.5f),
                new OccasionSeed(Occasion.Prom, "Prom", 20, 45, new[] { FlowerMeaning.Romance, FlowerMeaning.Celebration }, new[] { FlowerMeaning.Farewell, FlowerMeaning.Sympathy }, new FlowerColor[0], ArrangementStyle.Round, 0.45f),
            };

            int created = 0;
            foreach (var seed in seeds)
            {
                string path = $"{OccasionsFolder}/Occasion_{seed.Occasion}.asset";
                if (AssetDatabase.LoadAssetAtPath<OccasionDefinition>(path) != null) continue;

                var asset = ScriptableObject.CreateInstance<OccasionDefinition>();
                asset.occasion = seed.Occasion;
                asset.displayName = seed.DisplayName;
                asset.typicalBudgetMin = seed.BudgetMin;
                asset.typicalBudgetMax = seed.BudgetMax;
                asset.expectedMeanings = new List<FlowerMeaning>(seed.ExpectedMeanings);
                asset.tabooMeanings = new List<FlowerMeaning>(seed.TabooMeanings);
                asset.preferredPalette = new List<FlowerColor>(seed.PreferredPalette);
                asset.preferredStyle = seed.PreferredStyle;
                asset.forgiveness = seed.Forgiveness;

                AssetDatabase.CreateAsset(asset, path);
                created++;
            }
            return created;
        }

        private static int GenerateWrappings()
        {
            var seeds = new (WrappingType Type, string Name, float Cost)[]
            {
                (WrappingType.Kraft, "Kraft Paper", 1f),
                (WrappingType.Tissue, "Tissue Paper", 1.5f),
                (WrappingType.Cellophane, "Cellophane", 2f),
                (WrappingType.Ribbon, "Ribbon Wrap", 2.5f),
                (WrappingType.LuxuryBox, "Luxury Box", 6f),
            };

            int created = 0;
            foreach (var seed in seeds)
            {
                string path = $"{WrappingsFolder}/Wrapping_{seed.Type}.asset";
                if (AssetDatabase.LoadAssetAtPath<WrappingDefinition>(path) != null) continue;

                var asset = ScriptableObject.CreateInstance<WrappingDefinition>();
                asset.type = seed.Type;
                asset.displayName = seed.Name;
                asset.cost = seed.Cost;

                AssetDatabase.CreateAsset(asset, path);
                created++;
            }
            return created;
        }

        private static int GenerateVases()
        {
            var seeds = new (VaseType Type, string Name, float Cost)[]
            {
                (VaseType.Basic, "Basic Vase", 2f),
                (VaseType.Mason, "Mason Jar", 3f),
                (VaseType.Glass, "Glass Vase", 4f),
                (VaseType.Ceramic, "Ceramic Vase", 5f),
                (VaseType.Crystal, "Crystal Vase", 12f),
            };

            int created = 0;
            foreach (var seed in seeds)
            {
                string path = $"{VasesFolder}/Vase_{seed.Type}.asset";
                if (AssetDatabase.LoadAssetAtPath<VaseDefinition>(path) != null) continue;

                var asset = ScriptableObject.CreateInstance<VaseDefinition>();
                asset.type = seed.Type;
                asset.displayName = seed.Name;
                asset.cost = seed.Cost;

                AssetDatabase.CreateAsset(asset, path);
                created++;
            }
            return created;
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            Directory.CreateDirectory(path);
            AssetDatabase.Refresh();
        }

        private readonly struct FlowerSeed
        {
            public readonly string Id;
            public readonly string DisplayName;
            public readonly FlowerColor Color;
            public readonly FlowerMeaning PrimaryMeaning;
            public readonly FlowerMeaning[] SecondaryMeanings;
            public readonly Season[] InSeason;
            public readonly float BaseStemPrice;
            public readonly float WiltRatePerDay;
            public readonly bool IsHighPollen;

            public FlowerSeed(string id, string displayName, FlowerColor color, FlowerMeaning primaryMeaning,
                FlowerMeaning[] secondaryMeanings, Season[] inSeason, float baseStemPrice, float wiltRatePerDay, bool isHighPollen)
            {
                Id = id;
                DisplayName = displayName;
                Color = color;
                PrimaryMeaning = primaryMeaning;
                SecondaryMeanings = secondaryMeanings;
                InSeason = inSeason;
                BaseStemPrice = baseStemPrice;
                WiltRatePerDay = wiltRatePerDay;
                IsHighPollen = isHighPollen;
            }
        }

        private readonly struct OccasionSeed
        {
            public readonly Occasion Occasion;
            public readonly string DisplayName;
            public readonly int BudgetMin;
            public readonly int BudgetMax;
            public readonly FlowerMeaning[] ExpectedMeanings;
            public readonly FlowerMeaning[] TabooMeanings;
            public readonly FlowerColor[] PreferredPalette;
            public readonly ArrangementStyle PreferredStyle;
            public readonly float Forgiveness;

            public OccasionSeed(Occasion occasion, string displayName, int budgetMin, int budgetMax,
                FlowerMeaning[] expectedMeanings, FlowerMeaning[] tabooMeanings, FlowerColor[] preferredPalette,
                ArrangementStyle preferredStyle, float forgiveness)
            {
                Occasion = occasion;
                DisplayName = displayName;
                BudgetMin = budgetMin;
                BudgetMax = budgetMax;
                ExpectedMeanings = expectedMeanings;
                TabooMeanings = tabooMeanings;
                PreferredPalette = preferredPalette;
                PreferredStyle = preferredStyle;
                Forgiveness = forgiveness;
            }
        }
    }
}
