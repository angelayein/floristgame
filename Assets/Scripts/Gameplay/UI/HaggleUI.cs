using UnityEngine;
using UnityEngine.UI;
using FloristGame.Core.Economy;

namespace FloristGame.Gameplay
{
    // Buttons/labels for the haggle round: shows the customer's line and
    // current price, lets the player hold firm or accept the counter.
    public class HaggleUI : MonoBehaviour
    {
        public GameManager gameManager;
        public HaggleController haggleController;
        public GameObject panelRoot;
        public Text customerLineLabel;
        public Text priceLabel;
        public Button holdFirmButton;
        public Button acceptButton;

        private void OnEnable()
        {
            gameManager.OnHaggleRoundResolved += HandleRoundResolved;
            if (holdFirmButton != null) holdFirmButton.onClick.AddListener(haggleController.HoldFirm);
            if (acceptButton != null) acceptButton.onClick.AddListener(AcceptCurrentCounter);
        }

        private void OnDisable()
        {
            gameManager.OnHaggleRoundResolved -= HandleRoundResolved;
            if (holdFirmButton != null) holdFirmButton.onClick.RemoveListener(haggleController.HoldFirm);
            if (acceptButton != null) acceptButton.onClick.RemoveListener(AcceptCurrentCounter);
        }

        private void AcceptCurrentCounter()
        {
            haggleController.AcceptCounter(haggleController.CurrentAskedPrice);
        }

        private void HandleRoundResolved(HaggleResult result)
        {
            bool showPanel = result.Outcome == HaggleOutcome.CounterOffered;
            if (panelRoot != null) panelRoot.SetActive(showPanel);
            if (customerLineLabel != null) customerLineLabel.text = result.CustomerLine;
            if (priceLabel != null) priceLabel.text = $"${haggleController.CurrentAskedPrice:0.00}";
        }
    }
}
