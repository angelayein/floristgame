using System.Collections.Generic;
using UnityEngine;
using FloristGame.Core.Flowers;
using FloristGame.Core.Scoring;

namespace FloristGame.Gameplay
{
    // Spawns one DraggableFlowerChip per FlowerDefinition in the
    // GameManager's catalog into a laid-out container, and keeps each
    // chip's stock count / freshness display in sync as stems get
    // consumed or wilt away. Put this on the StockTray GameObject
    // alongside its Horizontal/Grid Layout Group.
    public class StockTraySpawner : MonoBehaviour
    {
        public GameManager gameManager;
        public DraggableFlowerChip chipPrefab;
        public Transform container;

        private readonly Dictionary<string, DraggableFlowerChip> _chipsBySpeciesId = new Dictionary<string, DraggableFlowerChip>();

        private void Start()
        {
            SpawnChips();
            RefreshAllStock();
        }

        private void OnEnable()
        {
            if (gameManager == null) return;
            gameManager.OnArrangementEvaluated += HandleInventoryChanged;
            gameManager.OnDayAdvanced += HandleDayAdvanced;
        }

        private void OnDisable()
        {
            if (gameManager == null) return;
            gameManager.OnArrangementEvaluated -= HandleInventoryChanged;
            gameManager.OnDayAdvanced -= HandleDayAdvanced;
        }

        private void SpawnChips()
        {
            var existing = new List<GameObject>();
            foreach (Transform child in container)
            {
                existing.Add(child.gameObject);
            }
            foreach (var go in existing)
            {
                Destroy(go);
            }
            _chipsBySpeciesId.Clear();

            foreach (var definition in gameManager.flowerCatalog)
            {
                var chip = Instantiate(chipPrefab, container);
                chip.SetFlower(definition);
                _chipsBySpeciesId[definition.id] = chip;
            }
        }

        private void HandleInventoryChanged(ScoreBreakdown breakdown) => RefreshAllStock();
        private void HandleDayAdvanced(int day, Season season) => RefreshAllStock();

        private void RefreshAllStock()
        {
            int day = gameManager.DayCycle.CurrentDay;
            foreach (var kvp in _chipsBySpeciesId)
            {
                int available = gameManager.Inventory.GetAvailableStems(kvp.Key, day);
                float freshness = gameManager.Inventory.GetAverageFreshness(kvp.Key, day);
                kvp.Value.RefreshStock(available, freshness);
            }
        }
    }
}
