using Alchemy.Meta.Collection;
using Alchemy.Meta.Summon;

namespace Alchemy
{
    public class SummonModel
    {
        public readonly SummonConfig SummonConfig;

        public SummonLevelData Level => SummonConfig.SummonLevelData.LevelData[CurrentLevelIndex];
        public int CurrentLevelIndex => _collectableItem != null ? _collectableItem.LevelIndex : _currentLevelIndex;
        public int CharacterDamage => CurrentLevelIndex * SummonConfig.SummonLevelData.CharacterDamagePerLevel;
        public int CharacterHealth => CurrentLevelIndex * SummonConfig.SummonLevelData.CharacterHealthPerLevel;

        private readonly ICollectableItem _collectableItem;
        private readonly int _currentLevelIndex;
        
        public SummonModel(SummonConfig summonConfig, int levelIndex)
        {
            _currentLevelIndex = levelIndex;
            SummonConfig = summonConfig;
        }

        public SummonModel(SummonConfig summonConfig, ICollectableItem collectableItem)
        {
            SummonConfig = summonConfig;
            _collectableItem = collectableItem;
        }

        public SummonController CreateController()
        {
            return new SummonController();
        }
    }
}