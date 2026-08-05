using System.Collections.Generic;
using UnityEngine;
using FloristGame.Core.Flowers;

namespace FloristGame.Data
{
    [CreateAssetMenu(fileName = "Flower_", menuName = "Florist Game/Flower Species")]
    public class FlowerDefinition : ScriptableObject
    {
        public string id;
        public string displayName;
        public FlowerColor color;
        public FlowerMeaning primaryMeaning;
        public List<FlowerMeaning> secondaryMeanings = new List<FlowerMeaning>();
        public List<Season> inSeason = new List<Season>();
        public float baseStemPrice = 2f;

        [Range(0f, 1f)]
        public float wiltRatePerDay = 0.15f;

        public bool isHighPollen;
        public Sprite icon;

        public FlowerSpecies ToCoreSpecies()
        {
            return new FlowerSpecies(id, displayName, color, primaryMeaning, secondaryMeanings, inSeason, baseStemPrice, wiltRatePerDay, isHighPollen);
        }
    }
}
