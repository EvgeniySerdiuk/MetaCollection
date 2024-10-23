using System.Collections.Generic;
using System.Linq;

namespace Alchemy.Meta.Collection
{
    public class CollectableItemRepository
    {
        public CollectableItemType RepositoryType { get; private set; }
        public ICollectableItem[] CollectableItems { get; private set; }

        public void Construct<T>(CollectableConfigsContainer<CollectableItemConfig<T>> itemsConfig,
            CollectableItemsSaveModelContainer saveModelContainer)
        {
            var length = itemsConfig.Items.Length;
            RepositoryType = itemsConfig.CollectableItemType;
            CollectableItems = new ICollectableItem[length];

            IEnumerable<CollectableItemSaveModel> saveModels = null;

            if (saveModelContainer.Models != null)
            {
                saveModels = saveModelContainer.Models.Where(x => x.ItemType == RepositoryType);
            }

            for (int i = 0; i < length; i++)
            {
                var level = 0;
                var amountCard = 0;
                var isOpen = itemsConfig.Items[i].ItemIsOpen;

                var saveItem = saveModels?.FirstOrDefault(x => x.Name == itemsConfig.Items[i].Name);

                if (saveItem != null)
                {
                    level = saveItem.LevelIndex + 1;
                    amountCard = saveItem.AmountCard;
                    isOpen = saveItem.IsOpen;
                }

                CollectableItems[i] = new CollectableItem<T>(itemsConfig.Items[i], itemsConfig.CollectableItemType,
                    isOpen, level, amountCard);
            }
        }
    }
}