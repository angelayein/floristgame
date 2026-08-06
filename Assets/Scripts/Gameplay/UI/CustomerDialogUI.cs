using UnityEngine;
using UnityEngine.UI;
using FloristGame.Core.Customers;
using FloristGame.Core.Scoring;
using FloristGame.Data;

namespace FloristGame.Gameplay
{
    // Displays the current customer's request text and reacts with mood
    // portraits as their arrangement gets evaluated / haggled.
    public class CustomerDialogUI : MonoBehaviour
    {
        public GameManager gameManager;
        public Text customerNameLabel;
        public Text requestLabel;
        public Image portraitImage;

        [Tooltip("Used when GameManager has no archetypeCatalog configured, so the portrait is never just blank.")]
        public CustomerArchetype fallbackArchetype;

        private void OnEnable()
        {
            gameManager.OnCustomerArrived += HandleCustomerArrived;
            gameManager.OnArrangementEvaluated += HandleArrangementEvaluated;
            gameManager.OnCustomerWalkedAway += HandleCustomerWalkedAway;
        }

        private void OnDisable()
        {
            gameManager.OnCustomerArrived -= HandleCustomerArrived;
            gameManager.OnArrangementEvaluated -= HandleArrangementEvaluated;
            gameManager.OnCustomerWalkedAway -= HandleCustomerWalkedAway;
        }

        private void HandleCustomerArrived(CustomerRequest request, CustomerArchetype archetype)
        {
            if (customerNameLabel != null) customerNameLabel.text = request.CustomerName;
            if (requestLabel != null) requestLabel.text = request.FlavorText;
            SetPortrait(CustomerMood.Neutral);
        }

        private void HandleArrangementEvaluated(ScoreBreakdown breakdown)
        {
            SetPortrait(CustomerMoodMapper.FromScore(breakdown));
        }

        private void HandleCustomerWalkedAway()
        {
            SetPortrait(CustomerMood.Offended);
        }

        private void SetPortrait(CustomerMood mood)
        {
            if (portraitImage == null) return;

            var archetype = gameManager.CurrentArchetype != null ? gameManager.CurrentArchetype : fallbackArchetype;
            if (archetype == null) return;

            portraitImage.sprite = archetype.GetPortrait(mood);
        }
    }
}
