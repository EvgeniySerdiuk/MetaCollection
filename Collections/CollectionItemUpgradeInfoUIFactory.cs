using Alchemy.Meta.Collection.UI;
using Alchemy.Meta.GameModes.DefaultMatchMode;
using Alchemy.Meta.Summon;
using Alchemy.Meta.Weapon;
using SpellsSlotMachine;
using Alchemy.Meta.User;
using UnityEngine;

namespace Alchemy.Meta.Collection
{
    public class CollectionItemUpgradeInfoUIFactory
    {
        private readonly CollectionPreviewUpgradeInfoUI _panelInfoPrefab;
        private readonly CollectionPreviewUpgradeInfoCharacterUI _panelInfoCharacterPrefab;
        private readonly Transform _root;
        private readonly UserInfo _userInfo;

        private CollectionPreviewUpgradeInfoUI[] _activePanel;
        private CollectionPreviewUpgradeInfoCharacterUI _activeCharacterPanel;

        private ICollectableItem _currentCollectableItem;

        public CollectionItemUpgradeInfoUIFactory(CollectionPreviewUpgradeInfoUI panelInfoPrefab,
            CollectionPreviewUpgradeInfoCharacterUI panelInfoCharacterPrefab, Transform root,
            PlayerInfoProvider playerInfoProvider)
        {
            _panelInfoPrefab = panelInfoPrefab;
            _panelInfoCharacterPrefab = panelInfoCharacterPrefab;
            _root = root;
            _userInfo = playerInfoProvider.GetPlayerInfo();
        }

        public void ShowPanels(ICollectableItem collectableItem)
        {
            _currentCollectableItem = collectableItem;
            CreateCharacterInfoPanel(collectableItem);
            CreateInfoPanel(collectableItem.InfoPanels, collectableItem.LevelIndex);
        }

        public void RefreshPanelValue()
        {
            CreateCharacterInfoPanel(_currentCollectableItem);

            for (int i = 0; i < _activePanel.Length; i++)
            {
                _activePanel[i].RefreshValue(_currentCollectableItem.InfoPanels[i].Panel,
                    _currentCollectableItem.LevelIndex);
            }
        }

        public void DestroyPanels()
        {
            if (_activeCharacterPanel != null)
            {
                Object.Destroy(_activeCharacterPanel.gameObject);
            }

            if (_activePanel != null)
            {
                foreach (var panel in _activePanel)
                {
                    Object.Destroy(panel.gameObject);
                }
            }

            _currentCollectableItem = null;
            _activePanel = null;
            _activeCharacterPanel = null;
        }

        private void CreateInfoPanel(ItemInfoPanelUi[] collectionItemInfoPanels, int levelIndex)
        {
            _activePanel = new CollectionPreviewUpgradeInfoUI[collectionItemInfoPanels.Length];

            for (int i = 0; i < collectionItemInfoPanels.Length; i++)
            {
                var panel = Object.Instantiate(_panelInfoPrefab, _root);
                panel.Construct(collectionItemInfoPanels[i].Panel, levelIndex);
                _activePanel[i] = panel;
            }
        }

        private void CreateCharacterInfoPanel(ICollectableItem collectableItem)
        {
            float oldDamage = 0;
            float newDamage = 0;
            float oldHealth = 0;
            float newHealth = 0;

            int level = collectableItem.LevelIndex;

            switch (collectableItem.ItemType)
            {
                case CollectableItemType.AttackSpell:
                    var spellAttack = (collectableItem as CollectableItem<SpellConfigBase>).Config.SpellData;
                    GetValuesCharacter(spellAttack.BaseCharacterDamage, level,
                        spellAttack.CharacterDamagePerLevel, ref oldDamage, ref newDamage);
                    break;
                case CollectableItemType.BuffSpell:
                    var spellBuff = (collectableItem as CollectableItem<SpellConfigBase>).Config.SpellData;
                    GetValuesCharacter(spellBuff.BaseCharacterHealth, level, spellBuff.CharacterHealthPerLevel,
                        ref oldHealth, ref newHealth);
                    break;
                case CollectableItemType.Weapon:
                    var weapon = (collectableItem as CollectableItem<WeaponConfigBase>).Config;
                    GetValuesCharacter(weapon.BaseCharacterDamage, level,
                        weapon.CharacterDamagePerLevel, ref oldDamage, ref newDamage);
                    break;
            }

            if (oldDamage + newDamage <= 0 && oldHealth + newHealth <= 0)
            {
                return;
            }

            if (_activeCharacterPanel == null)
            {
                _activeCharacterPanel = Object.Instantiate(_panelInfoCharacterPrefab, _root);
                
                if (oldDamage + newDamage > 0)
                {
                    _activeCharacterPanel.ShowAttackInfo(oldDamage, newDamage);
                }

                if (oldHealth + newHealth > 0)
                {
                    _activeCharacterPanel.ShowHealthInfo(oldHealth, newHealth);
                }
            }
            else
            {
                if (oldDamage + newDamage > 0)
                {
                    _activeCharacterPanel.RefreshAttackInfo(oldDamage, newDamage);
                }

                if (oldHealth + newHealth > 0)
                {
                    _activeCharacterPanel.RefreshHealthInfo(oldHealth, newHealth);
                }
            }
        }

        private void GetValuesCharacter(float baseValue, float level, float valuePerLevel, ref float beforeValue,
            ref float afterValue)
        {
            beforeValue = baseValue + level * valuePerLevel;
            afterValue = baseValue + (level + 1) * valuePerLevel;
        }
    }
}