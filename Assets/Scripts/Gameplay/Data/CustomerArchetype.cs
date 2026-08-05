using System.Collections.Generic;
using UnityEngine;

namespace FloristGame.Data
{
    public enum CustomerMood
    {
        Neutral,
        Delighted,
        Happy,
        Confused,
        Offended,
        Disappointed
    }

    [System.Serializable]
    public struct MoodPortrait
    {
        public CustomerMood mood;
        public Sprite sprite;
    }

    // Cosmetic/flavor wrapper around a customer appearance - the actual
    // request logic (RequestGenerator) doesn't know or care which archetype
    // is standing at the counter, only which DifficultyTier is active.
    [CreateAssetMenu(fileName = "Customer_", menuName = "Florist Game/Customer Archetype")]
    public class CustomerArchetype : ScriptableObject
    {
        public string archetypeName;
        public List<MoodPortrait> portraits = new List<MoodPortrait>();
        public List<string> barks = new List<string>();

        public Sprite GetPortrait(CustomerMood mood)
        {
            foreach (var portrait in portraits)
            {
                if (portrait.mood == mood) return portrait.sprite;
            }
            return portraits.Count > 0 ? portraits[0].sprite : null;
        }
    }
}
