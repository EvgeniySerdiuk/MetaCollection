using System;
using System.Linq;
using Alchemy.Meta.GameModes.DefaultMatchMode;
using Alchemy.Meta.User;
using Alchemy.Services;
using Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;
using Screen = Match.UI.Screen;

namespace Alchemy.Meta.Collection.UI
{
    public class CollectionScreenUI : Screen, IPointerDownHandler
    {
        public event Action OnClickCollectionScreen;
        public event Action OnShowSlotPreview;

        [SerializeField] private CollectionSlotUIPreview collectionSlotUIReview;
        [SerializeField] private CollectionPanelUI[] collectionPanelsPrefabs;

        [field: SerializeField] public CanvasGroup CollectionFadeCanvasGroup { get; private set; }
        [field: SerializeField] public CollectionScrollRect ScrollRect { get; private set; }

        public CollectionPanelUI[] CollectionPanels { get; private set; }

        private ItemsRepositoryService _itemsRepository;
        private MetaCurrencyService _metaCurrencyService;

        [Inject]
        public void Construct(ItemsRepositoryService itemsRepositoryService, IEventBuss eventBuss,
            MetaCurrencyService metaCurrencyService, PlayerInfoProvider playerInfoProvider)
        {
            _itemsRepository = itemsRepositoryService;
            _metaCurrencyService = metaCurrencyService;

            collectionSlotUIReview.Construct(metaCurrencyService, playerInfoProvider, itemsRepositoryService);
            CollectionPanels = new CollectionPanelUI[collectionPanelsPrefabs.Length];

            base.Construct(eventBuss);

            for (int i = 0; i < collectionPanelsPrefabs.Length; i++)
            {
                CollectionPanels[i] = CreateCollectionPaneL(collectionPanelsPrefabs[i]);
                CollectionPanels[i].transform.SetSiblingIndex(i);
            }

            CollectionFadeCanvasGroup.Hide();
            CollectionFadeCanvasGroup.transform.SetAsLastSibling();
        }

        public override void Show()
        {
            for (var i = CollectionPanels.Length - 1; i >= 0; i--)
            {
                CollectionPanels[i].Show();
            }

            base.Show();

            ScrollRect.OnBeginDragScroll += InvokeOnСlick;
            collectionSlotUIReview.OnShowSlotPreview += InvokeOnShowSlotPreview;
        }

        public override void Hide()
        {
            for (var i = CollectionPanels.Length - 1; i >= 0; i--)
            {
                CollectionPanels[i].Hide();
            }

            base.Hide();
            CollectionFadeCanvasGroup.Hide();
            ScrollRect.OnBeginDragScroll -= InvokeOnСlick;
            collectionSlotUIReview.OnShowSlotPreview -= InvokeOnShowSlotPreview;
        }

        private void InvokeOnShowSlotPreview()
        {
            OnShowSlotPreview?.Invoke();
        }

        private CollectionPanelUI CreateCollectionPaneL(CollectionPanelUI collectionPanelUiPrefab)
        {
            var collectableItems = _itemsRepository.GetRepositoryForType(collectionPanelUiPrefab.CollectionPanelType)
                .CollectableItems;

            var panel = Instantiate(collectionPanelUiPrefab, ScrollRect.content);
            panel.Construct(collectableItems, collectionSlotUIReview, EventBuss, _metaCurrencyService, _itemsRepository);

            return panel;
        }

        private void InvokeOnСlick()
        {
            OnClickCollectionScreen?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            InvokeOnСlick();
        }

        public void ShowPreviewSlot(ICollectableItem collectableItem)
        {
            var panel = CollectionPanels.First(x => x.CollectionPanelType == collectableItem.ItemType);
            var slot = panel.Slots.First(x => x.CollectableItem == collectableItem);
            slot.SetPressedState();
            collectionSlotUIReview.ShowSlotPreview(slot);
        }
    }
}