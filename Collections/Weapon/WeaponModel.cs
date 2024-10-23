using Alchemy.Meta.Weapon;

namespace Alchemy.Meta.Collection
{
    public class WeaponModel
    {
        public readonly WeaponConfigBase WeaponConfig;
        public float CharacterDamage => CurrentLevelIndex  * WeaponConfig.CharacterDamagePerLevel;
        public float CharacterHealth => CurrentLevelIndex  * WeaponConfig.CharacterHealthPerLevel;
        public int CurrentLevelIndex => _collectableItem != null ? _collectableItem.LevelIndex : _currentLevelIndex;

        private readonly ICollectableItem _collectableItem;
        private readonly int _currentLevelIndex;
        
        public WeaponModel(WeaponConfigBase weaponConfig, int levelIndex)
        {
            _currentLevelIndex = levelIndex;
            WeaponConfig = weaponConfig;
        }
        
        public WeaponModel(WeaponConfigBase weaponConfig, ICollectableItem collectableItem)
        {
            _collectableItem = collectableItem;
            WeaponConfig = weaponConfig;
        }
    }
}
