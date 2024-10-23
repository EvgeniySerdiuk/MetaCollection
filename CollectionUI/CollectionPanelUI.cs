using System.Linq;
using Alchemy.Services;
using TMPro;
using UnityEngine;

namespace Alchemy.Meta.Collection.UI
{
    public class CollectionPanelUI : MonoBehaviour
    {
        [field: SerializeField] public CollectableItemType CollectionPanelType { get; private set; }

        [SerializeField] private TMP_Text panelText;
        [SerializeField] private TMP_Text currentAmountOpenCards;
        [SerializeField] private RectTransform slotsContainer;
        [SerializeField] private CollectionSlotUI collectionSlotPrefab;
        [SerializeField] private Canvas panelCanvas;
        [SerializeField] private string defaultText;
        [SerializeField] private string selectCardText;

        public CollectionSlotUI[] Slots { get; private set; }

        private ICollectableItem[] _items;
        private IEventBuss _eventBuss;

        public void Construct(ICollectableItem[] collectionItems, CollectionSlotUIPreview slotPreview,
            IEventBuss eventBuss, MetaCurrencyService metaCurrencyService,
            ItemsRepositoryService itemsRepositoryService)
        {
            _eventBuss = eventBuss;
            _items = collectionItems;

            Slots = new CollectionSlotUI[collectionItems.Length];

            for (var index = 0; index < collectionItems.Length; index++)
            {
                ICollectableItem item = collectionItems[index];
                var slot = Instantiate(collectionSlotPrefab, slotsContainer);
                slot.Construct(item, slotPreview, _eventBuss, metaCurrencyService, itemsRepositoryService);

                Slots[index] = slot;
            }
        }

        public void Show()
        {
            foreach (var collectionSlotUI in Slots)
            {
                collectionSlotUI.Show();
            }

            var openedSlotsAmount = _items.Count(x => x.IsOpen);
            currentAmountOpenCards.text = $"{openedSlotsAmount}/{_items.Length}";
        }

        public void Hide()
        {
            foreach (var collectionSlotUI in Slots)
            {
                collectionSlotUI.Hide();
            }

            IsShowSelectedPanel(false);
        }

        public void IsShowSelectedPanel(bool isEnable)
        {
            panelCanvas.overrideSorting = isEnable;
            panelText.text = isEnable ? selectCardText : defaultText;
        }
    }
}