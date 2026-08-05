using UnityEngine;
using UnityEngine.UI;

namespace FloristGame.Gameplay
{
    public class ReputationBarUI : MonoBehaviour
    {
        public GameManager gameManager;
        public Slider slider;

        private void Update()
        {
            if (slider != null && gameManager.Reputation != null)
            {
                slider.value = gameManager.Reputation.Reputation / 100f;
            }
        }
    }
}
