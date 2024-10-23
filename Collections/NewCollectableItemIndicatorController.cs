using System;
using Alchemy.Meta.Collection.UI;
using Alchemy.Meta.LootBox.LootBoxReceive;
using Alchemy.Meta.UI;
using Alchemy.Save;
using FMOD;
using Match.UI;
using VContainer.Unity;

namespace Alchemy.Meta.Collection
{
    public class NewItemIndicatorSaveModel
    {
        public string ItemId = string.Empty;
        public CollectableItemType ItemType;
    }

    public class NewCollectableItemIndicatorController : IInitializable, IListener<HasBeenReceivedNewItem>
    {
        public const string SaveKey = "NewCollectableItem";

        private readonly HudController _hudController;
        private readonly CollectionScreenController _collectionScreenController;
        private readonly ItemsRepositoryService _itemsRepositoryService;
        private readonly SaveService _saveService;
        private readonly IEventBuss _eventBuss;

        public ICollectableItem? CurrentNewItem { get; private set; }

        public NewCollectableItemIndicatorController(HudController hudController, IEventBuss eventBuss,
            CollectionScreenController collectionScreenController, ItemsRepositoryService itemsRepositoryService,
            SaveService saveService)
        {
            _hudController = hudController;
            _eventBuss = eventBuss;
            _collectionScreenController = collectionScreenController;
            _itemsRepositoryService = itemsRepositoryService;
            _saveService = saveService;
        }

        public void Initialize()
        {
            Load();
            _eventBuss.AddListener(this);
        }

        private void RefreshNewItemIndicator(CollectionSlotUI collectableSlot)
        {
            if (collectableSlot == null)
            {
                return;
            }

            if (CurrentNewItem == collectableSlot.CollectableItem)
            {
                _collectionScreenController.OnPressedNewSlot -= RefreshNewItemIndicator;
                _hudController.ToggleNewItemIndicator(false);
                CurrentNewItem.IsNewItem = false;
                CurrentNewItem = null;
                collectableSlot.RefreshNewItemIndicator();
                Save();
            }
        }

        public void Execute(ref HasBeenReceivedNewItem command)
        {
            if (CurrentNewItem != null)
            {
                CurrentNewItem.IsNewItem = false;
            }

            CurrentNewItem = command.NewItem;
            CurrentNewItem.IsNewItem = true;

            _hudController.ToggleNewItemIndicator(true);
            _collectionScreenController.OnPressedNewSlot += RefreshNewItemIndicator;
            Save();
        }

        private void Save()
        {
            var saveModel = new NewItemIndicatorSaveModel
            {
                ItemId = CurrentNewItem != null ? CurrentNewItem.Id : string.Empty,
                ItemType = CurrentNewItem != null ? CurrentNewItem.ItemType : CollectableItemType.Summon,
                
            };
            _saveService.FlashChanges(SaveKey, saveModel);
        }

        private void Load()
        {
            var loadModel = _saveService.LoadEntry(SaveKey, new NewItemIndicatorSaveModel());

            if (loadModel.ItemId != string.Empty)
            {
                _hudController.ToggleNewItemIndicator(true);
                CurrentNewItem = _itemsRepositoryService.GetCollectableItemByID(loadModel.ItemId, loadModel.ItemType);
                CurrentNewItem.IsNewItem = true;
                _collectionScreenController.OnPressedNewSlot += RefreshNewItemIndicator;
            }
        }
    }
}