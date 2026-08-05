using System;
using UnityEngine;
using FloristGame.Core.Arrangements;
using FloristGame.Core.Scoring;

namespace FloristGame.Gameplay
{
    // Owns the in-progress ArrangementBuilder for whichever customer is
    // currently at the counter, and drives the serve -> evaluate -> haggle
    // flow through GameManager.
    public class WorkspaceController : MonoBehaviour
    {
        public GameManager gameManager;
        public HaggleController haggleController;

        public ArrangementBuilder Builder { get; private set; } = new ArrangementBuilder();

        public event Action OnArrangementChanged;

        public void AddStemToArrangement(string speciesId)
        {
            Builder.AddStem(speciesId);
            OnArrangementChanged?.Invoke();
        }

        public void RemoveStemFromArrangement(string speciesId)
        {
            Builder.RemoveStem(speciesId);
            OnArrangementChanged?.Invoke();
        }

        public void SetWrapping(WrappingType wrapping)
        {
            Builder.Wrapping = wrapping;
            OnArrangementChanged?.Invoke();
        }

        public void SetVase(VaseType vase)
        {
            Builder.Vase = vase;
            OnArrangementChanged?.Invoke();
        }

        public void SetStyle(ArrangementStyle style)
        {
            Builder.Style = style;
            OnArrangementChanged?.Invoke();
        }

        // Call when the player hits "serve" with an asking price. Finalizes
        // the arrangement against inventory, scores it, then hands off to
        // the haggle loop.
        public void ServeCustomer(float askedPrice)
        {
            if (!Builder.TryFinalize(gameManager.Inventory, gameManager.DayCycle.CurrentDay, out Arrangement arrangement))
            {
                Debug.LogWarning("Not enough fresh stock to build this arrangement.");
                return;
            }

            ScoreBreakdown breakdown = gameManager.EvaluateArrangement(arrangement, askedPrice);
            haggleController.BeginHaggle(breakdown, askedPrice);
        }

        public void ResetForNextCustomer()
        {
            Builder = new ArrangementBuilder();
            OnArrangementChanged?.Invoke();
        }
    }
}
