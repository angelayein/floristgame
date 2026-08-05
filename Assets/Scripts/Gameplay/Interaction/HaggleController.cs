using UnityEngine;
using FloristGame.Core.Economy;
using FloristGame.Core.Scoring;

namespace FloristGame.Gameplay
{
    // Runs the round-by-round haggle loop for the currently served
    // arrangement: player asks a price, customer accepts/counters/walks,
    // player can match a counter or hold firm (which risks a walkout).
    public class HaggleController : MonoBehaviour
    {
        public GameManager gameManager;
        public WorkspaceController workspace;

        public ScoreBreakdown CurrentBreakdown { get; private set; }
        public float CurrentAskedPrice { get; private set; }
        public int Round { get; private set; }
        public bool IsActive { get; private set; }

        public void BeginHaggle(ScoreBreakdown breakdown, float askedPrice)
        {
            CurrentBreakdown = breakdown;
            CurrentAskedPrice = askedPrice;
            Round = 0;
            IsActive = true;

            ResolveRound();
        }

        // Player accepts the customer's counter-offer, or matches whatever
        // was last proposed.
        public void AcceptCounter(float price)
        {
            FinishSale(price);
        }

        // Player holds firm at the current asked price and tries again.
        public void HoldFirm()
        {
            if (!IsActive) return;
            Round++;
            ResolveRound();
        }

        private void ResolveRound()
        {
            var result = gameManager.Haggle(CurrentBreakdown, CurrentAskedPrice, Round);

            switch (result.Outcome)
            {
                case HaggleOutcome.Accepted:
                    FinishSale(CurrentAskedPrice);
                    break;
                case HaggleOutcome.WalkedAway:
                    IsActive = false;
                    workspace.ResetForNextCustomer();
                    break;
                case HaggleOutcome.CounterOffered:
                    CurrentAskedPrice = result.CounterPrice;
                    break;
            }
        }

        private void FinishSale(float finalPrice)
        {
            IsActive = false;
            gameManager.CompleteSale(CurrentBreakdown, finalPrice);
            workspace.ResetForNextCustomer();
        }
    }
}
