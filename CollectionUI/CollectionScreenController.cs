using System;
using System.Linq;
using Alchemy.Meta.UI;
using Extensions;
using Match.UI;
using UnityEngine;
using VContainer;
using Screen = Match.UI.Screen;

namespace Alchemy.Meta.Collection.UI
{
    public class CollectionScreenController : IScreenController
    {
        public event Action<CollectionSlotUI> OnPressedNewSlot;

        public string ScreenId => ScreensNames.Collection;

        private readonly ScreenManager _screenManager;
        private readonly IObjectResolver _resolver;

        private CollectionScreenUI _collectionScreenUI;
        private CollectionSlotUI? _currentPressedSlot;

        public CollectionScreenController(IObjectResolver resolver, ScreenManager screenManager)
        {
            _screenManager = screenManager;
            _resolver = resolver;

            _screenManager.RegisterController(this);
        }

        public void Initialize()
        {
        }

        public void Show(Screen screen, params object[] args)
        {
            if (!_collectionScreenUI)
            {
                _resolver.Inject(screen);
                _collectionScreenUI = screen as CollectionScreenUI;
            }

            screen.Show();
            SubscribesToSlotsEvent();
        }

        public void Hide()
        {
            _currentPressedSlot?.SetPressedState();
            _currentPressedSlot = null;
            UnsubscribesToSlotsEvent();
            _collectionScreenUI.Hide();
        }

        private void SubscribesToSlotsEvent()
        {
            _collectionScreenUI.OnClickCollectionScreen += DeactivatePressedSlot;

            foreach (var panel in _collectionScreenUI.CollectionPanels)
            {
                foreach (var slot in panel.Slots)
                {
                    if (slot.CurrentContentType != CollectionSlotUI.CollectionSlotContentType.Lock)
                    {
                        slot.OnSetReadySelectState += SetReadyDeselectState;
                        slot.OnDeselectSlot += ReadySelectToSelect;
                        slot.OnCancelButtonClick += CancelSelection;
                        slot.OnPressedSlot += PressedSlot;
                        slot.OnSelectSlot += DeselectCurrentSelectedSlot;
                    }
                    else
                    {
                        slot.OnPressedSlot += PressedSlot;
                    }
                }
            }
        }

        private void UnsubscribesToSlotsEvent()
        {
            _collectionScreenUI.OnClickCollectionScreen -= DeactivatePressedSlot;

            foreach (var panel in _collectionScreenUI.CollectionPanels)
            {
                foreach (var slot in panel.Slots)
                {
                    if (slot.CurrentContentType != CollectionSlotUI.CollectionSlotContentType.Lock)
                    {
                        slot.OnSetReadySelectState -= SetReadyDeselectState;
                        slot.OnDeselectSlot -= ReadySelectToSelect;
                        slot.OnCancelButtonClick -= CancelSelection;
                        slot.OnPressedSlot -= PressedSlot;
                        slot.OnSelectSlot -= DeselectCurrentSelectedSlot;
                    }
                    else
                    {
                        slot.OnPressedSlot -= PressedSlot;
                    }
                }
            }
        }

        private void SetReadyDeselectState(CollectionSlotUI collectionSlotUI)
        {
            ScrollToPanel(collectionSlotUI.CollectableItem);

            var panel = _collectionScreenUI.CollectionPanels.First(x =>
                x.CollectionPanelType == collectionSlotUI.CollectableItem.ItemType);

            var selectedSlots = panel.Slots.Where(x =>
                x.CurrentContentType == CollectionSlotUI.CollectionSlotContentType.Selected);

            selectedSlots.ForEach(x => { x.SetReadyDeselectState(); });

            CollectionOverrideFadeEnable(true, panel);
        }

        private void ReadySelectToSelect(CollectableItemType collectableItemType)
        {
            _currentPressedSlot = null;
            _collectionScreenUI.CollectionFadeCanvasGroup.Hide();
            _collectionScreenUI.ScrollRect.enabled = true;

            var panel = _collectionScreenUI.CollectionPanels.First(x => x.CollectionPanelType == collectableItemType);
            var readySelectSlot = panel.Slots.First(x =>
                x.CurrentContentType == CollectionSlotUI.CollectionSlotContentType.ReadySelect);
            readySelectSlot.SetSelectedState();

            CancelSelection(collectableItemType);
        }

        private void CancelSelection(CollectableItemType collectableItemType)
        {
            _currentPressedSlot = null;

            var panel = _collectionScreenUI.CollectionPanels.First(x => x.CollectionPanelType == collectableItemType);
            var slots = panel.Slots.Where(x =>
                x.CurrentContentType == CollectionSlotUI.CollectionSlotContentType.ReadyDeselect);

            slots.ForEach(x => { x.SetSelectedState(); });

            CollectionOverrideFadeEnable(false, panel);
        }

        private void DeactivatePressedSlot()
        {
            if (_currentPressedSlot?.CurrentContentType == CollectionSlotUI.CollectionSlotContentType.ReadySelect)
            {
                return;
            }

            PressedSlot(null);
        }

        private void PressedSlot(CollectionSlotUI? collectionSlotUI)
        {
            ScrollToSlot(collectionSlotUI);
            OnPressedNewSlot?.Invoke(collectionSlotUI);

            if (_currentPressedSlot == collectionSlotUI)
            {
                _currentPressedSlot = null;
                return;
            }

            if (_currentPressedSlot != null)
            {
                _currentPressedSlot.OffPressedState();
                _currentPressedSlot = null;
            }

            _currentPressedSlot = collectionSlotUI;
        }

        private void DeselectCurrentSelectedSlot(CollectableItemType collectableItemType)
        {
            _currentPressedSlot = null;

            var panel = _collectionScreenUI.CollectionPanels.First(x => x.CollectionPanelType == collectableItemType);
            var slot = panel.Slots.FirstOrDefault(x =>
                x.CurrentContentType == CollectionSlotUI.CollectionSlotContentType.Selected);

            slot?.SetUnlockState();
        }

        private void CollectionOverrideFadeEnable(bool value, CollectionPanelUI collectionPanelUI)
        {
            if (value)
            {
                _collectionScreenUI.CollectionFadeCanvasGroup.Show();
            }
            else
            {
                _collectionScreenUI.CollectionFadeCanvasGroup.Hide();
            }

            collectionPanelUI.IsShowSelectedPanel(value);
            _collectionScreenUI.ScrollRect.enabled = !value;
        }

        public void ScrollToPanel(ICollectableItem collectableItem)
        {
            var itemType = collectableItem.ItemType;

            foreach (var panel in _collectionScreenUI.CollectionPanels)
            {
                if (itemType == panel.CollectionPanelType)
                {
                    Vector3 selectedLocalPosition =
                        _collectionScreenUI.ScrollRect.content.InverseTransformPoint(panel.transform.position);

                    Vector2 newAnchoredPosition = _collectionScreenUI.ScrollRect.content.anchoredPosition;
                    newAnchoredPosition.y = -selectedLocalPosition.y;

                    _collectionScreenUI.ScrollRect.content.anchoredPosition = newAnchoredPosition;

                    break;
                }
            }
        }

        public void ShowPreviewSlot(ICollectableItem collectableItem)
        {
            _collectionScreenUI.ShowPreviewSlot(collectableItem);
        }

        private void ScrollToSlot(CollectionSlotUI collectionSlotUI)
        {
            if (collectionSlotUI == null)
            {
                return;
            }

            var rectSlot = collectionSlotUI.SlotRectTransform;
            var slotPosition =
                _collectionScreenUI.ScrollRect.viewport.InverseTransformPoint(rectSlot.position);

            var viewportRect = _collectionScreenUI.ScrollRect.viewport.rect;
            var maxYSlotPosition = slotPosition.y + rectSlot.rect.height / 2;
            var minYSlotPosition = slotPosition.y - rectSlot.rect.height;

            if (viewportRect.yMax < maxYSlotPosition || viewportRect.yMin > minYSlotPosition)
            {
                Vector2 newAnchoredPosition = _collectionScreenUI.ScrollRect.content.anchoredPosition;

                newAnchoredPosition.y -= viewportRect.yMax < maxYSlotPosition
                    ? (maxYSlotPosition - viewportRect.yMax) + 20
                    : (minYSlotPosition - viewportRect.yMin) - 100;

                _collectionScreenUI.ScrollRect.content.anchoredPosition = newAnchoredPosition;
            }
        }
    }
}