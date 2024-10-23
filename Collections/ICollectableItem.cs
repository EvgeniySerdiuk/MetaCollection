using UnityEngine;

namespace Alchemy.Meta.Collection
{
    public interface ICollectableItem
    {
        public string Id { get; }
        public CollectableItemType ItemType { get; }
        public string Name { get; }
        public string Description { get; }
        public string OpenCondition { get; }
        public int LevelIndex { get; }
        public Sprite Sprite { get; }
        public Sprite SpriteForHomeDeck { get; }
        public int CurrentAmountCard { get; }
        public int RequiredAmountCard { get; }
        public int UpgradePrice { get; }
        public bool IsOpen { get; }
        public bool IsSelected { get; set; }
        public bool IsNewItem { get; set; }
        public bool IsHaveMaxLevel { get; }
        public ItemInfoPanelUi[] InfoPanels { get; }

        public void UpgradeLevel();
        public bool TryChangeAmountCard(int amountCard);
        public void OpenItem();
    }
}