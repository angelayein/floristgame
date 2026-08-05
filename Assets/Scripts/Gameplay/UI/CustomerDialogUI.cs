using UnityEngine;
using UnityEngine.UI;
using FloristGame.Core.Customers;
using FloristGame.Data;

namespace FloristGame.Gameplay
{
    // Displays the current customer's request text and portrait mood.
    public class CustomerDialogUI : MonoBehaviour
    {
        public GameManager gameManager;
        public Text customerNameLabel;
        public Text requestLabel;
        public Image portraitImage;
        public CustomerArchetype fallbackArchetype;

        private void OnEnable()
        {
            gameManager.OnCustomerArrived += HandleCustomerArrived;
        }

        private void OnDisable()
        {
            gameManager.OnCustomerArrived -= HandleCustomerArrived;
        }

        private void HandleCustomerArrived(CustomerRequest request)
        {
            if (customerNameLabel != null) customerNameLabel.text = request.CustomerName;
            if (requestLabel != null) requestLabel.text = request.FlavorText;

            if (portraitImage != null && fallbackArchetype != null)
            {
                portraitImage.sprite = fallbackArchetype.GetPortrait(CustomerMood.Neutral);
            }
        }
    }
}
