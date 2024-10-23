using System;
using System.Collections.Generic;
using Alchemy.Meta.Collection.UI;
using Alchemy.Meta.Summon;
using Alchemy.Meta.User;
using Alchemy.Meta.Weapon;
using SpellsSlotMachine;
using System.Linq;
using Alchemy.Save;
using Extensions;
using VContainer.Unity;

namespace Alchemy.Meta.Collection
{
    public class SelectedItemsSaveModel
    {
        public string[] SelectedItemsName;
    }
    
    public class CollectableItemsSelectedRegistry : IInitializable, IListener<CollectionSlotSelected>,
        IListener<CollectionSlotDeselected>
    {
        public const string SaveKey = "SelectedItems";
        
        private readonly UserInfo _userInfo;
        private readonly IEventBuss _eventBuss;
        private readonly CollectableItemsContainer _itemsContainer;
        private readonly ItemsRepositoryService _itemsRepositoryService;

        private CollectableItem<SpellConfigBase>[] _attackSpells;
        private CollectableItem<SpellConfigBase>[] _buffSpells;
        private CollectableItem<WeaponConfigBase> _weaponItem;
        private CollectableItem<SummonConfig> _summonItem;

        private HashSet<ICollectableItem> _selectedItems = new();
        private readonly SaveService _saveService;

        public CollectableItemsSelectedRegistry(UserInfo userInfo, IEventBuss eventBuss,
            CollectableItemsContainer itemsContainer, ItemsRepositoryService itemsRepositoryService, SaveService saveService)
        {
            _eventBuss = eventBuss;
            _userInfo = userInfo;
            _itemsContainer = itemsContainer;
            _itemsRepositoryService = itemsRepositoryService;
            _saveService = saveService;

            _attackSpells =
                new CollectableItem<SpellConfigBase>[_itemsContainer.GetAmountItems(CollectableItemType.AttackSpell)];
            _buffSpells =
                new CollectableItem<SpellConfigBase>[_itemsContainer.GetAmountItems(CollectableItemType.BuffSpell)];
        }

        public void Initialize()
        {
            _eventBuss.AddListener<CollectionSlotDeselected>(this);
            _eventBuss.AddListener<CollectionSlotSelected>(this);
            SetSelectedItems();
        }

        public void Execute(ref CollectionSlotSelected command)
        {
            var collectableItem = command.CollectableItem;
            
            SelectItem(collectableItem);
        }

        public void Execute(ref CollectionSlotDeselected command)
        {
            var collectableItem = command.CollectableItem;

            switch (collectableItem.ItemType)
            {
                case CollectableItemType.AttackSpell:
                    RemoveSpellToArray(collectableItem, _attackSpells);
                    break;
                case CollectableItemType.BuffSpell:
                    RemoveSpellToArray(collectableItem, _buffSpells);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SelectItem(ICollectableItem collectableItem)
        {
            switch (collectableItem.ItemType)
            {
                case CollectableItemType.AttackSpell:
                    AddSpellToArray(collectableItem, _attackSpells);
                    _userInfo.Deck.SetSpellsToDeck(SpellType.Attack, GetSpellModels(_attackSpells));
                    break;
                case CollectableItemType.BuffSpell:
                    AddSpellToArray(collectableItem, _buffSpells);
                    _userInfo.Deck.SetSpellsToDeck(SpellType.Buff, GetSpellModels(_buffSpells));
                    break;
                case CollectableItemType.Summon:
                    SetCurrentItem(ref _summonItem, collectableItem);
                    _userInfo.SummonModel = new SummonModel(_summonItem.Config, collectableItem);
                    break;
                case CollectableItemType.Weapon:
                    SetCurrentItem(ref _weaponItem, collectableItem);
                    _userInfo.WeaponModel = new WeaponModel(_weaponItem.Config, collectableItem);
                    break;
            }
            
            Save();
        }

        private void SetSelectedItems()
        {
            var attackRepository = _itemsRepositoryService.GetRepositoryForType(CollectableItemType.AttackSpell);
            var buffRepository = _itemsRepositoryService.GetRepositoryForType(CollectableItemType.BuffSpell);
            var summonRepository = _itemsRepositoryService.GetRepositoryForType(CollectableItemType.Summon);
            var weaponRepository = _itemsRepositoryService.GetRepositoryForType(CollectableItemType.Weapon);

            if (TryLoad())
            {
                return;
            }
            
            for (int i = 0; i < _itemsContainer.GetAmountItems(CollectableItemType.AttackSpell); i++)
            {
                SelectItem(attackRepository.CollectableItems[i]);
                SelectItem(buffRepository.CollectableItems[i]);
            }

            SelectItem(weaponRepository.CollectableItems[0]);
            SelectItem(summonRepository.CollectableItems[0]);
        }

        private void SetCurrentItem<T>(ref CollectableItem<T> currentItem, ICollectableItem newItem)
        {
            if (currentItem != null)
            {
                currentItem.IsSelected = false;
                _selectedItems.Remove(currentItem);
            }

            newItem.IsSelected = true;
            _selectedItems.Add(newItem);

            currentItem = newItem as CollectableItem<T>;
        }

        private void AddSpellToArray<T>(ICollectableItem collectableItem, CollectableItem<T>[] collectableItemsArray)
        {
            for (int i = 0; i < collectableItemsArray.Length; i++)
            {
                if (collectableItemsArray[i] == null)
                {
                    collectableItemsArray[i] = collectableItem as CollectableItem<T>;
                    collectableItem.IsSelected = true;
                    _selectedItems.Add(collectableItem);
                    break;
                }
            }
        }

        private void RemoveSpellToArray<T>(ICollectableItem collectableItem, CollectableItem<T>[] collectableItemsArray)
        {
            for (int i = 0; i < collectableItemsArray.Length; i++)
            {
                if (collectableItemsArray[i] == collectableItem)
                {
                    _selectedItems.Remove(collectableItem);
                    collectableItem.IsSelected = false;
                    collectableItemsArray[i] = null;
                    break;
                }
            }
        }

        private SpellModel[] GetSpellModels(CollectableItem<SpellConfigBase>[] collectableItems)
        {
            var items = collectableItems.Where(x => x != null)
                .Select(x => new SpellModel(x.Config, x)).ToArray();

            return items;
        }

        public ICollectableItem[] GetSelectedItems(CollectableItemType itemType)
        {
            switch (itemType)
            {
                case CollectableItemType.AttackSpell:
                    return _attackSpells;
                case CollectableItemType.BuffSpell:
                    return _buffSpells;
                case CollectableItemType.Summon:
                    return new[] { _summonItem };
                case CollectableItemType.Weapon:
                    return new[] { _weaponItem };
            }

            return null;
        }

        private void Save()
        {
            var saveModel = new SelectedItemsSaveModel
            {
                SelectedItemsName = new string [_selectedItems.Count]
            };

            var index = 0;
            foreach (var item in _selectedItems)
            {
                saveModel.SelectedItemsName[index] = item.Name;
                index++;
            }
            
            _saveService.FlashChanges(SaveKey, saveModel);
        }

        private bool TryLoad()
        {
            var loadModel = _saveService.LoadEntry(SaveKey, new SelectedItemsSaveModel());

            if (loadModel.SelectedItemsName != null && loadModel.SelectedItemsName.Length > 0)
            {
                var openItems = _itemsRepositoryService.GetCollectableItemsForOpenness(true);
                
                loadModel.SelectedItemsName.ForEach(name =>
                {
                    SelectItem(openItems.First(x => x.Name == name));
                });
                
                return true;
            }
            return false;
        }
    }
}