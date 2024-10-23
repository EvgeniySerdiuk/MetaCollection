using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Alchemy.Match.SlotMachine.SpellsSlot;
using Alchemy.Services;
using DG.Tweening;
using FMODUnity;
using TheraBytes.BetterUi;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VFX;
using static Alchemy.Meta.Collection.UI.CollectionSlotBottomUI;
using static Alchemy.Meta.Collection.UI.CollectionSlotUI;

namespace Alchemy.Meta.Collection.UI
{
    [Serializable]
    public class CollectionSlotContent
    {
        [field: SerializeField] public CollectionSlotContentType ContentType { get; private set; }
        [field: SerializeField] public Sprite BorderSprite { get; private set; }
        [field: SerializeField] public Material ItemIconMaterial { get; private set; }
        [field: SerializeField] public Material TextMaterial { get; private set; }

        [field: SerializeField] public CanvasGroup[] ShowContent { get; private set; }
        [field: SerializeField] public CanvasGroup[] HideContent { get; private set; }
    }

    public class CollectionSlotUI : MonoBehaviour
    {
        public enum CollectionSlotContentType
        {
            Lock = 0,
            Unlock = 1,
            Selected = 2,
            Pressed = 3,
            ReadySelect = 4,
            ReadyDeselect = 5
        }

        public event Action<CollectionSlotUI> OnPressedSlot;
        public event Action<CollectionSlotUI> OnSetReadySelectState;
        public event Action<CollectableItemType> OnDeselectSlot;
        public event Action<CollectableItemType> OnCancelButtonClick;
        public event Action<CollectableItemType> OnSelectSlot;

        [SerializeField] private RectTransform slotRectTransform;
        [SerializeField] private BetterLayoutElement betterLayoutElement;

        [SerializeField] private CanvasGroup newItemIndicatorCanvasGroup;

        [SerializeField] private Button slotButton;
        [SerializeField] private Button selectButton;
        [SerializeField] private Button infoButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button upgradeButton;

        [SerializeField] private Image itemBorder;
        [SerializeField] private Image itemImage;

        [SerializeField] private TMP_Text itemName;
        [SerializeField] private TMP_Text itemLevel;

        [SerializeField] private string itemLevelFormat;

        [SerializeField] private CollectionSlotBottomUI[] bottomPanels;
        [SerializeField] private CollectionSlotContent[] slotContent;

        [SerializeField] private VFXSystem upgradeItemVFX;
        [SerializeField] private EventReference upgradeEvent;

        [SerializeField] private TweenSettings<float> readyDeselectTweenSettings;
        [SerializeField] private TweenSettings<float> selectTweenSettings;
        [SerializeField] private TweenSettings<float> upgradeScaleTweenSettings;
        [SerializeField] private TweenSettings<float> upgradeScaleLevelStartTweenSettings;
        [SerializeField] private TweenSettings<float> upgradeScaleLevelFinishTweenSettings;
        [SerializeField] private TweenSettings<float> upgradeMoveLevelTweenSettings;

        public RectTransform SlotRectTransform => slotRectTransform;
        public ICollectableItem CollectableItem { get; private set; }
        public CollectionSlotContentType CurrentContentType { get; private set; }
        public CollectionSlotContentType BeforePressedContentType { get; private set; }
        public CollectionSlotContentType BeforeContentType { get; private set; }

        private readonly List<CollectionSlotBottomUI> _currentBottomPanels = new();
        private CollectionSlotUIPreview _slotPreview;
        private MetaCurrencyService _metaCurrencyService;
        private ItemsRepositoryService _itemsRepositoryService;
        private Tweener _readyDeselectTween;
        private Tweener _selectTween;
        private IEventBuss _eventBuss;

        public void Construct(ICollectableItem collectionItem, CollectionSlotUIPreview slotUIPreview,
            IEventBuss eventBuss, MetaCurrencyService metaCurrencyService,
            ItemsRepositoryService itemsRepositoryService)
        {
            CollectableItem = collectionItem;
            _slotPreview = slotUIPreview;
            _eventBuss = eventBuss;
            _itemsRepositoryService = itemsRepositoryService;
            _metaCurrencyService = metaCurrencyService;

            itemImage.sprite = collectionItem.Sprite;
            itemName.text = collectionItem.Name;

            itemName.enableWordWrapping = collectionItem.Name.Contains(" ");
        }

        public void Show()
        {
            RefreshState();
            RefreshNewItemIndicator();

            infoButton.onClick.AddListener(() => { _slotPreview.ShowSlotPreview(this); });

            selectButton.onClick.AddListener(() =>
            {
                if (CollectableItem.ItemType is CollectableItemType.AttackSpell or CollectableItemType.BuffSpell)
                {
                    SetReadySelectState();
                }
                else
                {
                    SetSelectedState();
                }
            });

            cancelButton.onClick.AddListener(CancelSelection);

            CreateTweens();
        }

        private void CreateTweens()
        {
            _readyDeselectTween = betterLayoutElement.transform.DOShakePosition(readyDeselectTweenSettings.Duration,
                    readyDeselectTweenSettings.TargetValue, fadeOut: false)
                .SetEase(readyDeselectTweenSettings.Curve).Pause();

            _selectTween = betterLayoutElement.transform
                .DOScale(selectTweenSettings.TargetValue, selectTweenSettings.Duration)
                .SetEase(selectTweenSettings.Curve).Pause();
        }

        public void Hide()
        {
            betterLayoutElement.transform.SetParent(transform);

            slotButton.onClick.RemoveAllListeners();
            selectButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();
            infoButton.onClick.RemoveAllListeners();

            _selectTween.Kill();
            _readyDeselectTween.Kill();
        }

        private void RefreshState()
        {
            switch (CollectableItem)
            {
                case { IsOpen: false }:
                    SetLockState();
                    break;
                case { IsSelected: true }:
                    SetSelectedState();
                    break;
                default:
                    SetUnlockState();
                    break;
            }

            itemLevel.text = string.Format(itemLevelFormat, CollectableItem.LevelIndex + 1);
            ShowBottomPanel();
        }

        private void SetLockState()
        {
            RefreshContent(CollectionSlotContentType.Lock);
            slotButton.onClick.RemoveAllListeners();
            slotButton.onClick.AddListener(SetPressedState);
        }

        public void SetUnlockState()
        {
            RefreshContent(CollectionSlotContentType.Unlock);
            slotButton.onClick.RemoveAllListeners();
            slotButton.onClick.AddListener(SetPressedState);
        }

        public void SetSelectedState()
        {
            _readyDeselectTween.Pause();
            bool shouldScale = CurrentContentType != CollectionSlotContentType.Selected &&
                               (CurrentContentType == CollectionSlotContentType.ReadySelect ||
                                CollectableItem.ItemType is not
                                    (CollectableItemType.AttackSpell or CollectableItemType.BuffSpell));

            if (shouldScale)
            {
                _selectTween.Play();
            }

            slotButton.onClick.RemoveAllListeners();

            if (CollectableItem.ItemType is not (CollectableItemType.AttackSpell or CollectableItemType.BuffSpell))
            {
                OnSelectSlot?.Invoke(CollectableItem.ItemType);
            }

            if (CurrentContentType is not CollectionSlotContentType.ReadyDeselect)
            {
                _eventBuss.Execute(new CollectionSlotSelected() { CollectableItem = CollectableItem });
            }

            RefreshContent(CollectionSlotContentType.Selected);
            slotButton.onClick.AddListener(SetPressedState);
        }

        public void SetPressedState()
        {
            if (CollectableItem.IsNewItem)
            {
                CollectableItem.IsNewItem = false;
                _itemsRepositoryService.Save();
            }

            BeforePressedContentType = CurrentContentType;
            RefreshContent(CollectionSlotContentType.Pressed);
            slotButton.onClick.RemoveAllListeners();
            slotButton.onClick.AddListener(OffPressedState);

            OnPressedSlot?.Invoke(this);
        }

        public void OffPressedState()
        {
            OnPressedSlot?.Invoke(this);
            RefreshState();
        }

        public void SetReadySelectState()
        {
            slotButton.onClick.RemoveAllListeners();
            RefreshContent(CollectionSlotContentType.ReadySelect);
            OnSetReadySelectState?.Invoke(this);
        }

        public void SetReadyDeselectState()
        {
            _readyDeselectTween.SetLoops(-1, LoopType.Restart).Play();
            RefreshContent(CollectionSlotContentType.ReadyDeselect);
            slotButton.onClick.RemoveAllListeners();

            slotButton.onClick.AddListener(() =>
            {
                _readyDeselectTween.Pause();
                _eventBuss.Execute(new CollectionSlotDeselected() { CollectableItem = CollectableItem });
                SetUnlockState();
                OnDeselectSlot?.Invoke(CollectableItem.ItemType);
            });
        }

        private void CancelSelection()
        {
            switch (CurrentContentType)
            {
                case CollectionSlotContentType.ReadySelect:
                    SetUnlockState();
                    break;
                case CollectionSlotContentType.ReadyDeselect:
                    SetSelectedState();
                    break;
            }

            _readyDeselectTween.Pause();
            OnCancelButtonClick?.Invoke(CollectableItem.ItemType);
        }

        private void RefreshContent(CollectionSlotContentType type)
        {
            BeforeContentType = CurrentContentType;
            CurrentContentType = type;
            var slot = slotContent.First(x => x.ContentType == type);

            if (type != CollectionSlotContentType.Pressed)
            {
                itemBorder.sprite = slot.BorderSprite;
            }

            itemImage.material = slot.ItemIconMaterial;
            itemName.fontMaterial = slot.TextMaterial;

            slot.ShowContent.ForEach(content => content.Show());
            slot.HideContent.ForEach(content => content.Hide());

            ShowBottomPanel();
        }

        private void ShowBottomPanel()
        {
            betterLayoutElement.transform.SetParent(transform);
            betterLayoutElement.transform.localPosition = Vector3.zero;

            if (_currentBottomPanels.Count > 0)
            {
                _currentBottomPanels.ForEach(x => x.gameObject.SetActive(false));
                _currentBottomPanels.Clear();
            }

            switch (CurrentContentType)
            {
                case CollectionSlotContentType.Lock:
                    var lockPanel = SearchBottomPanel(CollectionSlotButtonContentType.Lock);
                    _currentBottomPanels.Add(lockPanel);
                    lockPanel.BottomText.text = CollectableItem.OpenCondition;
                    break;
                case CollectionSlotContentType.Unlock:
                case CollectionSlotContentType.Selected:
                    var cardPanel = SearchBottomPanel(CollectionSlotButtonContentType.Card);
                    _currentBottomPanels.Add(cardPanel);
                    UpdateCardPanelValues(cardPanel);
                    break;
                case CollectionSlotContentType.Pressed:
                    _currentBottomPanels.Add(SearchBottomPanel(CollectionSlotButtonContentType.Info));

                    if (!CollectableItem.IsHaveMaxLevel &&
                        CollectableItem.CurrentAmountCard >= CollectableItem.RequiredAmountCard)
                    {
                        var upgradePanel = SearchBottomPanel(CollectionSlotButtonContentType.Price);
                        _currentBottomPanels.Add(upgradePanel);
                        UpdateUpgradePanelValues(upgradePanel);
                    }

                    if (BeforePressedContentType != CollectionSlotContentType.Selected &&
                        BeforePressedContentType != CollectionSlotContentType.Lock)
                    {
                        _currentBottomPanels.Add(SearchBottomPanel(CollectionSlotButtonContentType.Select));
                    }

                    betterLayoutElement.transform.SetParent(transform.parent.parent.parent);
                    break;
                case CollectionSlotContentType.ReadySelect:
                    _currentBottomPanels.Add(SearchBottomPanel(CollectionSlotButtonContentType.Cancel));
                    betterLayoutElement.transform.SetParent(transform.parent.parent.parent);
                    break;
                case CollectionSlotContentType.ReadyDeselect:
                    cardPanel = SearchBottomPanel(CollectionSlotButtonContentType.Card);
                    betterLayoutElement.transform.SetParent(transform.parent.parent.parent);
                    _currentBottomPanels.Add(cardPanel);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (_currentBottomPanels.Count > 0)
            {
                _currentBottomPanels.ForEach(x => x.gameObject.SetActive(true));
            }
        }

        private void UpdateCardPanelValues(CollectionSlotBottomUI cardBottom)
        {
            if (CollectableItem.IsHaveMaxLevel)
            {
                SetCardPanelMaxLevel(cardBottom);
                return;
            }

            cardBottom.BottomText.text =
                $"{CollectableItem.CurrentAmountCard}/{CollectableItem.RequiredAmountCard}";

            cardBottom.ProgressBar.FillAmount =
                (float)CollectableItem.CurrentAmountCard / (float)CollectableItem.RequiredAmountCard;

            cardBottom.ProgressBar.Filler.sprite = cardBottom.ProgressBar.FillAmount >= 1
                ? cardBottom.HaveCardForUpgradeSprite
                : cardBottom.DontHaveCardForUpgradeSprite;
        }

        public void SetCardPanelMaxLevel(CollectionSlotBottomUI cardBottom)
        {
            cardBottom.BottomText.text = cardBottom.TextForMaxLevel;
            cardBottom.BottomText.fontSize = cardBottom.SizeTextForMaxLevel;
            cardBottom.CardImage.gameObject.SetActive(false);
            cardBottom.BottomText.margin = Vector4.zero;
            cardBottom.ProgressBar.Filler.sprite = cardBottom.HaveCardForUpgradeSprite;
            cardBottom.ProgressBar.FillAmount = 0;
        }

        private void UpdateUpgradePanelValues(CollectionSlotBottomUI upgradePanel)
        {
            upgradeButton.onClick.RemoveAllListeners();
            upgradePanel.BottomText.text = $"{CollectableItem.UpgradePrice}";

            if (CollectableItem.UpgradePrice > _metaCurrencyService.GetValue(Currencies.Soft))
            {
                upgradePanel.BottomText.fontMaterial = upgradePanel.HaveNotMoneyForUpgradeMaterial;
            }
            else
            {
                upgradePanel.BottomText.fontMaterial = upgradePanel.HaveMoneyForUpgradeMaterial;

                upgradeButton.onClick.AddListener(() =>
                {
                    UpgradeItem();
                    ShowUpgradeEffect();
                });
            }
        }

        private CollectionSlotBottomUI SearchBottomPanel(CollectionSlotButtonContentType type)
        {
            return bottomPanels.First(x => x.BottomType == type);
        }

        private void UpgradeItem()
        {
            if (!CollectableItem.IsHaveMaxLevel &&
                CollectableItem.UpgradePrice <= _metaCurrencyService.GetValue(Currencies.Soft))
            {
                upgradeButton.onClick.RemoveAllListeners();
                _metaCurrencyService.AddValue(Currencies.Soft, (int)-CollectableItem.UpgradePrice);
                CollectableItem.UpgradeLevel();
                _itemsRepositoryService.Save();

                ShowBottomPanel();
            }
        }

        private void ShowUpgradeEffect()
        {
            var borderPos = itemBorder.transform.localPosition;

            Tween borderScaleStart = itemBorder.transform
                .DOScale(upgradeScaleTweenSettings.TargetValue, upgradeScaleTweenSettings.Duration)
                .SetDelay(upgradeScaleTweenSettings.Delay)
                .SetEase(upgradeScaleTweenSettings.Ease)
                .Pause();

            Tween borderMoveYStart = itemBorder.transform
                .DOLocalMoveY(borderPos.y + upgradeMoveLevelTweenSettings.TargetValue,
                    upgradeMoveLevelTweenSettings.Duration)
                .SetDelay(upgradeMoveLevelTweenSettings.Delay)
                .SetEase(upgradeMoveLevelTweenSettings.Ease)
                .Pause();

            Tween borderMoveYFinish = itemBorder.transform
                .DOLocalMoveY(borderPos.y, upgradeMoveLevelTweenSettings.Duration)
                .SetDelay(upgradeMoveLevelTweenSettings.Delay)
                .SetEase(upgradeMoveLevelTweenSettings.Ease)
                .Pause();

            Tween levelScaleFinish = itemLevel.transform
                .DOScaleY(upgradeScaleLevelFinishTweenSettings.TargetValue,
                    upgradeScaleLevelFinishTweenSettings.Duration)
                .SetDelay(upgradeScaleLevelFinishTweenSettings.Delay)
                .SetEase(upgradeScaleLevelFinishTweenSettings.Curve)
                .Pause();

            Tween levelScaleStart = itemLevel.transform
                .DOScaleY(upgradeScaleLevelStartTweenSettings.TargetValue, upgradeScaleLevelStartTweenSettings.Duration)
                .SetEase(upgradeScaleLevelStartTweenSettings.Curve)
                .OnComplete(() => { itemLevel.text = string.Format(itemLevelFormat, CollectableItem.LevelIndex + 1); })
                .Pause();

            Tween borderScaleFinish = itemBorder.transform
                .DOScale(upgradeScaleTweenSettings.FromValue, upgradeScaleTweenSettings.Duration)
                .SetDelay(upgradeScaleTweenSettings.Delay)
                .SetEase(upgradeScaleTweenSettings.Ease)
                .Pause();

            Sequence seq = DOTween.Sequence();

            seq.Append(borderScaleStart)
                .Join(borderMoveYStart)
                .Join(levelScaleStart)
                .Append(levelScaleFinish)
                .Join(borderScaleFinish)
                .Join(borderMoveYFinish)
                .Pause();

            CreateAndShowUpgradeVFX(itemBorder.transform);

            seq.Play();
        }

        public void CreateAndShowUpgradeVFX(Transform parent)
        {
            FMODUnity.RuntimeManager.PlayOneShot(upgradeEvent);
            if (upgradeItemVFX)
            {
                var upgradeVFX = Instantiate(upgradeItemVFX, parent, true);
                upgradeVFX.transform.localPosition = Vector3.zero;
                upgradeVFX.Play();
            }
        }

        public void RefreshLevelAndBottom()
        {
            itemLevel.text = string.Format(itemLevelFormat, CollectableItem.LevelIndex + 1);
            ShowBottomPanel();
        }

        public void RefreshNewItemIndicator()
        {
            newItemIndicatorCanvasGroup.alpha = CollectableItem.IsNewItem ? 1 : 0;
        }
    }
}