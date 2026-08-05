using System;
using FloristGame.Core.Flowers;

namespace FloristGame.Core.GameState
{
    // Drives the in-game calendar. Seasons cycle through a fixed order and
    // gate flower availability/pricing (see FlowerSpecies.IsInSeason).
    public sealed class DayCycleController
    {
        public int CurrentDay { get; private set; }
        public Season CurrentSeason { get; private set; }

        public event Action<int, Season> OnDayAdvanced;

        private readonly Season[] _seasonOrder;
        private readonly int _daysPerSeason;

        public DayCycleController(Season[] seasonOrder, int daysPerSeason, int startDay = 1)
        {
            _seasonOrder = seasonOrder;
            _daysPerSeason = Math.Max(1, daysPerSeason);
            CurrentDay = startDay;
            CurrentSeason = ComputeSeason(startDay);
        }

        public void AdvanceDay()
        {
            CurrentDay++;
            CurrentSeason = ComputeSeason(CurrentDay);
            OnDayAdvanced?.Invoke(CurrentDay, CurrentSeason);
        }

        private Season ComputeSeason(int day)
        {
            int index = ((day - 1) / _daysPerSeason) % _seasonOrder.Length;
            return _seasonOrder[index];
        }
    }
}
