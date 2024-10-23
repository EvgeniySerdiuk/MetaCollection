using Alchemy.Meta.Summon;
using Alchemy.Meta.Weapon;
using SpellsSlotMachine;
using UnityEngine;

namespace Alchemy.Meta.Collection
{
    public enum CollectableItemType
    {
        AttackSpell = 0,
        BuffSpell = 1,
        Summon = 2,
        Weapon = 3
    }

    public class CollectableItem<T> : ICollectableItem
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string OpenCondition { get; private set; }
        public int LevelIndex { get; private set; }
        public int MaxLevel { get; private set; }
        public int CurrentAmountCard { get; private set; }
        public bool IsOpen { get; private set; }
        public bool IsSelected { get; set; }
        public bool IsNewItem { get; set; }
        public Sprite Sprite { get; private set; }
        public Sprite SpriteForHomeDeck { get; private set; }
        public T Config { get; private set; }
        public CollectableItemLevelData ItemsLevelData { get; private set; }
        public CollectableItemType ItemType { get; private set; }
        public ItemInfoPanelUi[] InfoPanels { get; private set; }

        public int RequiredAmountCard => ItemsLevelData.CardStepForUpgrade * (LevelIndex + 1);
        public int UpgradePrice => (int)((LevelIndex * 2 + 3) * (100 * ((float)(LevelIndex + 2) / 2)));
        public bool IsHaveMaxLevel => LevelIndex + 1 == MaxLevel; 
        
        public CollectableItem(CollectableItemConfig<T> collectableItem, CollectableItemType itemType, bool isOpen,
            int level, int amountCard)
        {
            ItemsLevelData = collectableItem.LevelData;
            Name = collectableItem.Name;
            Description = collectableItem.Description;
            Sprite = collectableItem.ItemSprite;
            SpriteForHomeDeck = collectableItem.ItemSpriteForDeck;
            Config = collectableItem.Config;
            InfoPanels = collectableItem?.InfoPanels;
            OpenCondition = collectableItem.OpenCondition;
            IsOpen = isOpen;
            ItemType = itemType;
            CurrentAmountCard = amountCard;
            LevelIndex = level - 1 <= 0 ? 0 : level - 1;
            MaxLevel = collectableItem.MaxLevel;
            IsNewItem = false;

            SetId();
        }

        private void SetId()
        {
            switch (Config)
            {
                case SpellConfigBase spell:
                    Id = spell.Id;
                    break;
                case WeaponConfigBase weapon:
                    Id = weapon.Name;
                    break;
                case SummonConfig summon:
                    Id = summon.SummonName;
                    break;
            }
        }

        public void OpenItem()
        {
            IsOpen = true;
            IsNewItem = true;
        }

        public void UpgradeLevel()
        {
            if (LevelIndex + 1 < MaxLevel)
            {
                CurrentAmountCard -= RequiredAmountCard;
                LevelIndex++;
            }
        }

        public bool TryChangeAmountCard(int amountCard)
        {
            if (CurrentAmountCard + amountCard > 0)
            {
                if (!IsOpen)
                {
                    IsOpen = true;
                    IsNewItem = true;
                }

                CurrentAmountCard += amountCard;
                return true;
            }

            return false;
        }
    }
}