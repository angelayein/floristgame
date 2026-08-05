using UnityEngine;
using FloristGame.Core.Arrangements;

namespace FloristGame.Data
{
    [CreateAssetMenu(fileName = "Vase_", menuName = "Florist Game/Vase")]
    public class VaseDefinition : ScriptableObject
    {
        public VaseType type;
        public string displayName;
        public float cost = 2f;
        public Sprite icon;
    }
}
