using System.Collections.Generic;
using System.Linq;
using Alchemy.Meta.Summon;
using Alchemy.Meta.Weapon;
using Alchemy.Save;
using SpellsSlotMachine;
using VContainer.Unity;

namespace Alchemy.Meta.Collection
{
    public class CollectableItemSaveModel
    {
        public string Name;
        public bool IsOpen;
        public int AmountCard;
        public int LevelIndex;
        public CollectableItemType ItemType;
    }

    public class CollectableItemsSaveModelContainer
    {
        public CollectableItemSaveModel[] Models;
    }

    public class ItemsRepositoryService : IInitializable
    {
        private const string SaveKey = "ReceivedItems";
        private CollectableItemRepository[] _repositories;

        private readonly SpellsConfigsContainer _attackSpellsConfigsContainer;
        private readonly SpellsConfigsContainer _buffSpellsConfigsContainer;
        private readonly SummonConfigsContainer _summonConfigsContainer;
        private readonly WeaponsConfigsContainer _weaponsConfigsContainer;

        private readonly SaveService _saveService;

        public ItemsRepositoryService(CollectableItemsContainer collectableItemsContainer, SaveService saveService)
        {
            _weaponsConfigsContainer = collectableItemsContainer.WeaponsContainer;
            _summonConfigsContainer = collectableItemsContainer.SummonContainer;
            _attackSpellsConfigsContainer = collectableItemsContainer.AttackSpellContainer;
            _buffSpellsConfigsContainer = collectableItemsContainer.BuffSpellContainer;

            _saveService = saveService;
        }

        public void Initialize()
        {
            var openItems = Load();

            _repositories = new CollectableItemRepository[4];

            for (int i = 0; i < _repositories.Length; i++)
            {
                _repositories[i] = new CollectableItemRepository();
            }

            _repositories[0].Construct(_attackSpellsConfigsContainer, openItems);
            _repositories[1].Construct(_buffSpellsConfigsContainer, openItems);
            _repositories[2].Construct(_summonConfigsContainer, openItems);
            _repositories[3].Construct(_weaponsConfigsContainer, openItems);
        }

        public CollectableItemRepository GetRepositoryForType(CollectableItemType collectableItemType)
        {
            return _repositories.FirstOrDefault(rep => rep.RepositoryType == collectableItemType);
        }

        public ICollectableItem GetCollectableItemByID(string id, CollectableItemType itemType)
        {
            var rep = GetRepositoryForType(itemType);
            var item = rep.CollectableItems.First(x => x.Id == id);
            return item;
        }

        public HashSet<ICollectableItem> GetCollectableItemsForOpenness(bool isOpen)
        {
            HashSet<ICollectableItem> items = new();

            foreach (var rep in _repositories)
            {
                foreach (var item in rep.CollectableItems)
                {
                    if (item.IsOpen == isOpen)
                    {
                        items.Add(item);
                    }
                }
            }

            return items;
        }

        public void Save()
        {
            var openItems = GetCollectableItemsForOpenness(true);
            var saveModel = new CollectableItemsSaveModelContainer
            {
                Models = new CollectableItemSaveModel[openItems.Count]
            };

            int index = 0;

            foreach (var item in openItems)
            {
                var model = saveModel.Models[index] = new CollectableItemSaveModel();
                
                model.Name = item.Name;
                model.AmountCard = item.CurrentAmountCard;
                model.IsOpen = item.IsOpen;
                model.ItemType = item.ItemType;
                model.LevelIndex = item.LevelIndex;
                index++;
            }

            _saveService.FlashChanges(SaveKey, saveModel);
        }

        private CollectableItemsSaveModelContainer Load()
        {
            var loadModel = new CollectableItemsSaveModelContainer();
            return _saveService.LoadEntry(SaveKey, loadModel);
        }
    }
}