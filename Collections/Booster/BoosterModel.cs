using Alchemy.Meta.Boosters;

namespace Alchemy.Meta.Collection
{
    public class BoosterModel
    {
        public readonly BoosterConfigBase BoosterConfig;
        public readonly int CurrentLevelIndex = 0;
        public string Id => BoosterConfig.Id;
        
        public bool IsOpen { get; private set; }
        public int Amount { get; private set; }

        public BoosterModel(BoosterConfigBase config, bool isOpen, int amountBoosters)
        {
            BoosterConfig = config;
            IsOpen = isOpen;
            Amount = amountBoosters;
        }

        public void OpenBooster()
        {
            IsOpen = true;
        }

        public void AddAmountBooster(int value)
        {
            Amount = Amount + value > -1 ? Amount + value : 0;
        }
    }
}