using UnityEngine;
using UnityEngine.UI;
using FloristGame.Core.Flowers;

namespace FloristGame.Gameplay
{
    public class DayBannerUI : MonoBehaviour
    {
        public GameManager gameManager;
        public Text dayLabel;

        private void OnEnable()
        {
            gameManager.OnDayAdvanced += HandleDayAdvanced;
            Refresh();
        }

        private void OnDisable()
        {
            gameManager.OnDayAdvanced -= HandleDayAdvanced;
        }

        private void HandleDayAdvanced(int day, Season season) => Refresh();

        private void Refresh()
        {
            if (dayLabel != null && gameManager.DayCycle != null)
            {
                dayLabel.text = $"Day {gameManager.DayCycle.CurrentDay} - {gameManager.DayCycle.CurrentSeason}";
            }
        }
    }
}
