using System;
using UnityEngine;

namespace Alchemy.Meta.Collection
{
    [Serializable]
    public class ItemInfoPanelUi
    {
        [field: SerializeField] public CollectionItemInfoPanelUIConfig[] Panel { get; private set; }
    }

    [Serializable]
    public class CollectableItemConfig<T>
    {
        [field: SerializeField] public T Config { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public string OpenCondition { get; private set; }
        [field: SerializeField] public Sprite ItemSprite { get; private set; }
        [field: SerializeField] public Sprite ItemSpriteForDeck { get; private set; }
        [field: SerializeField] public bool ItemIsOpen { get; private set; }
        [field: SerializeField] public int MaxLevel { get; private set; } = 20;
        [field: SerializeField] public CollectableItemLevelData LevelData { get; private set; }
        [field: SerializeField] public ItemInfoPanelUi[] InfoPanels { get; private set; }
    }

    public class CollectableConfigsContainer<TCollectableConfig> : ScriptableObject
    {
        [field: SerializeField] public CollectableItemType CollectableItemType { get; private set; }
        [field: SerializeField] public TCollectableConfig[] Items { get; private set; }
    }
}
