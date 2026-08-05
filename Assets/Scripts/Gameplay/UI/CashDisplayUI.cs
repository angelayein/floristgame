using UnityEngine;
using UnityEngine.UI;

namespace FloristGame.Gameplay
{
    public class CashDisplayUI : MonoBehaviour
    {
        public GameManager gameManager;
        public Text cashLabel;

        private void Update()
        {
            if (cashLabel != null && gameManager.Economy != null)
            {
                cashLabel.text = $"${gameManager.Economy.Cash:0.00}";
            }
        }
    }
}
