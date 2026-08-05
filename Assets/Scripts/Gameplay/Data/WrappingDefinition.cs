using UnityEngine;
using FloristGame.Core.Arrangements;

namespace FloristGame.Data
{
    [CreateAssetMenu(fileName = "Wrapping_", menuName = "Florist Game/Wrapping")]
    public class WrappingDefinition : ScriptableObject
    {
        public WrappingType type;
        public string displayName;
        public float cost = 1f;
        public Sprite icon;
    }
}
