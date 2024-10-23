using Alchemy.SlotMachine.Configs;
using Match;
using SpellsSlotMachine;
using SpellsSlotMachine.UI;
using System;

namespace Alchemy.Meta.Collection
{
    public class SpellModel : ISlotElement
    {
        public string Id => SpellConfig.Id;
        public ILevelData Level => SpellConfig.SpellData.LevelsData[CurrentLevelIndex];

        public float CharacterDamage => CurrentLevelIndex * SpellConfig.SpellData.CharacterDamagePerLevel;

        public int CharacterHealth => CurrentLevelIndex * SpellConfig.SpellData.CharacterHealthPerLevel;

        public readonly SpellConfigBase SpellConfig;

        public UiSlotMachineIcon.UISlotMachineData SlotUIData => SpellConfig.SlotUIData;

        public SlotWeightConfig SlotWeightConfig => SpellConfig.SlotWeightConfig;

        public int Weight => SpellConfig.Weight;

        public int CurrentLevelIndex => _collectableItem != null ? _collectableItem.LevelIndex : _currentLevelIndex;

        private readonly ICollectableItem _collectableItem;
        private readonly int _currentLevelIndex;

        public SpellModel(SpellConfigBase spellConfig, int currentLevelIndex)
        {
            SpellConfig = spellConfig;
            _currentLevelIndex = currentLevelIndex;
        }

        public SpellModel(SpellConfigBase spellConfig, ICollectableItem collectableItem)
        {
            SpellConfig = spellConfig;
            _collectableItem = collectableItem;
        }

        public SpellController CreateController()
        {
            return (SpellController)Activator.CreateInstance(SpellConfig.ControllerType);
        }
    }
}