namespace FloristGame.Core.Economy
{
    public sealed class EconomyLedger
    {
        public float Cash { get; private set; }

        public EconomyLedger(float startingCash)
        {
            Cash = startingCash;
        }

        public void Add(float amount) => Cash += amount;

        public bool TrySpend(float amount)
        {
            if (amount < 0f || Cash < amount) return false;
            Cash -= amount;
            return true;
        }
    }
}
