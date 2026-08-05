using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FloristGame.Core.Arrangements;
using FloristGame.Core.Customers;
using FloristGame.Core.Economy;
using FloristGame.Core.Flowers;
using FloristGame.Core.Generation;
using FloristGame.Core.GameState;
using FloristGame.Core.Scoring;
using FloristGame.Data;

namespace FloristGame.Gameplay
{
    // Central orchestrator: owns the Core systems, holds the content catalog,
    // and exposes the request/build/haggle/settle flow that the Interaction
    // and UI scripts drive. Contains no gameplay rules itself - those all
    // live in FloristGame.Core so they stay engine-independent and testable.
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Content")]
        public List<FlowerDefinition> flowerCatalog = new List<FlowerDefinition>();
        public List<OccasionDefinition> occasionCatalog = new List<OccasionDefinition>();

        [Header("Economy")]
        public float startingCash = 200f;
        public int startingStemsPerSpecies = 20;
        public int daysPerSeason = 7;

        public InventoryManager Inventory { get; private set; }
        public ReputationTracker Reputation { get; private set; }
        public EconomyLedger Economy { get; private set; }
        public DayCycleController DayCycle { get; private set; }
        public CustomerRequest CurrentRequest { get; private set; }

        public event Action<CustomerRequest> OnCustomerArrived;
        public event Action<ScoreBreakdown> OnArrangementEvaluated;
        public event Action<HaggleResult> OnHaggleRoundResolved;
        public event Action<float, ScoreBreakdown> OnSaleCompleted; // finalPrice, breakdown
        public event Action OnCustomerWalkedAway;
        public event Action<int, Season> OnDayAdvanced;
        public event Action<string> OnUnlockGranted;

        private Dictionary<string, FlowerSpecies> _speciesById;
        private List<OccasionProfile> _occasionProfiles;
        private System.Random _rng;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _rng = new System.Random();
            _speciesById = flowerCatalog.ToDictionary(f => f.id, f => f.ToCoreSpecies());
            _occasionProfiles = occasionCatalog.Select(o => o.ToCoreProfile()).ToList();

            Inventory = new InventoryManager();
            Economy = new EconomyLedger(startingCash);

            var unlocks = new List<(float, string)>
            {
                (30f, "intermediate_customers"),
                (60f, "advanced_customers"),
                (85f, "vip_customers")
            };
            Reputation = new ReputationTracker(unlocks);
            Reputation.OnUnlockGranted += id => OnUnlockGranted?.Invoke(id);

            DayCycle = new DayCycleController(new[] { Season.Spring, Season.Summer, Season.Fall, Season.Winter }, daysPerSeason, 1);
            DayCycle.OnDayAdvanced += (day, season) => OnDayAdvanced?.Invoke(day, season);
        }

        private void Start()
        {
            SeedStartingInventory();
            SpawnNextCustomer();
        }

        public FlowerSpecies GetSpecies(string speciesId) => _speciesById.TryGetValue(speciesId, out var s) ? s : null;

        public IReadOnlyDictionary<string, FlowerSpecies> Catalog => _speciesById;

        private void SeedStartingInventory()
        {
            foreach (var species in _speciesById.Values)
            {
                Inventory.AddStock(species, startingStemsPerSpecies, DayCycle.CurrentDay);
            }
        }

        public void SpawnNextCustomer()
        {
            var tier = RequestGenerator.TierForReputation(Reputation.Reputation);
            CurrentRequest = RequestGenerator.Generate(_rng, _occasionProfiles, _speciesById.Values.ToList(), tier, GenerateCustomerName());
            OnCustomerArrived?.Invoke(CurrentRequest);
        }

        // Pure evaluation - does not charge, does not mutate reputation. Call
        // this once the player has built an Arrangement and picked a price.
        public ScoreBreakdown EvaluateArrangement(Arrangement arrangement, float chargedPrice)
        {
            var breakdown = ArrangementScorer.Score(CurrentRequest, arrangement, _speciesById, chargedPrice);
            OnArrangementEvaluated?.Invoke(breakdown);
            return breakdown;
        }

        public HaggleResult Haggle(ScoreBreakdown breakdown, float askedPrice, int round)
        {
            var result = HaggleNegotiator.Evaluate(CurrentRequest, breakdown, askedPrice, round, _rng);
            OnHaggleRoundResolved?.Invoke(result);
            if (result.Outcome == HaggleOutcome.WalkedAway)
            {
                OnCustomerWalkedAway?.Invoke();
            }
            return result;
        }

        // Applies the final agreed price: pays out, updates reputation, fires
        // the completion event. Caller is responsible for advancing to the
        // next customer (SpawnNextCustomer) once any UI/celebration finishes.
        public void CompleteSale(ScoreBreakdown breakdown, float finalPrice)
        {
            Economy.Add(finalPrice);
            Reputation.ApplyTransactionResult(breakdown.Overall);
            OnSaleCompleted?.Invoke(finalPrice, breakdown);
        }

        public void AdvanceToNextDay()
        {
            DayCycle.AdvanceDay();
            Inventory.AdvanceDay(DayCycle.CurrentDay);
            SpawnNextCustomer();
        }

        private static readonly string[] CustomerNames =
        {
            "Alex", "Jordan", "Sam", "Riley", "Morgan", "Taylor", "Casey", "Jamie", "Drew", "Quinn"
        };

        private string GenerateCustomerName() => CustomerNames[_rng.Next(CustomerNames.Length)];
    }
}
