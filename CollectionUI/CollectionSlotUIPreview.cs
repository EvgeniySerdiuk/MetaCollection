using System;
using Alchemy.Match.SlotMachine.SpellsSlot;
using Extensions;
using System.Linq;
using Alchemy.Meta.GameModes.DefaultMatchMode;
using Alchemy.Services;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Alchemy.Meta.Collection.UI.CollectionSlotBottomUI;
using static Alchemy.Meta.Collection.UI.CollectionSlotUI;

namespace Alchemy.Meta.Collection.UI
{
    public struct CollectionSlotSelected
    {
        public ICollectableItem CollectableItem;
    }

    public struct CollectionSlotDeselected
    {
        public ICollectableItem CollectableItem;
    }

    public class CollectionSlotUIPreview : MonoBehaviour
    {
        public event Action OnShowSlotPreview;

        [SerializeField] private RectTransform rootTransform;
        [SerializeField] private Image itemBorder;
        [SerializeField] private Image itemImage;

        [SerializeField] private TMP_Text itemName;
        [SerializeField] private TMP_Text itemDescription;
        [SerializeField] private TMP_Text itemLevel;

        [SerializeField] private Button closeButton;
        [SerializeField] private Button selectButton;
        [SerializeField] private Button upgradeButton;

        [SerializeField] private Canvas previewCanvas;
        [SerializeField] private CanvasGroup previewUiCanvasGroup;
        [SerializeField] private CanvasGroup bgPreview;

        [SerializeField] private CollectionPreviewUpgradeInfoUI upgradeInfoUI;
        [SerializeField] private CollectionPreviewUpgradeInfoCharacterUI upgradeInfoCharacterUI;
        [SerializeField] private RectTransform infoPanelRoot;
        [SerializeField] private Transform itemLevelTransform;

        [SerializeField] private CollectionSlotContent[] slotContent;
        [SerializeField] private CollectionSlotBottomUI[] bottomPanels;

        [SerializeField] private TweenSettings<float> showSetting;
        [SerializeField] private TweenSettings<float> hideSetting;
        [SerializeField] private TweenSettings<float> fadeSetting;
        [SerializeField] private TweenSettings<float> upgradeScaleLevelStartTweenSettings;
        [SerializeField] private TweenSettings<float> upgradeScaleLevelFinishTweenSettings;

        private CollectionSlotBottomUI _currentBottomUi;
        private CollectionItemUpgradeInfoUIFactory _panelInfoGeneration;
        private CollectionSlotUI _currentCollectionSlot;
        private CollectionSlotContentType _currentContentType;
        private ICollectableItem _collectableItem;
        private MetaCurrencyService _metaCurrencyService;
        private ItemsRepositoryService _repositoryService;

        public void Construct(MetaCurrencyService metaCurrencyService, PlayerInfoProvider playerInfoProvider,
            ItemsRepositoryService repositoryService)
        {
            previewCanvas.enabled = false;
            previewUiCanvasGroup.Hide();
            rootTransform.localPosition -= Vector3.up * rootTransform.rect.height;

            _panelInfoGeneration =
                new CollectionItemUpgradeInfoUIFactory(upgradeInfoUI, upgradeInfoCharacterUI, infoPanelRoot,
                    playerInfoProvider);

            _metaCurrencyService = metaCurrencyService;
            _repositoryService = repositoryService;
        }

        public void ShowSlotPreview(CollectionSlotUI collectionSlot)
        {
            _currentCollectionSlot = collectionSlot;
            _currentContentType = collectionSlot.BeforePressedContentType;

            Show(collectionSlot.CollectableItem);
            OnShowSlotPreview?.Invoke();
        }

        private void Show(ICollectableItem collectableItem)
        {
            previewCanvas.enabled = true;
            previewUiCanvasGroup.Show();

            StartShowAnimation();

            _collectableItem = collectableItem;
            _collectableItem.IsNewItem = false;

            itemImage.sprite = collectableItem.Sprite;
            itemName.text = collectableItem.Name;
            itemDescription.text = collectableItem.Description;
            itemLevel.text = (collectableItem.LevelIndex + 1).ToString();

            var slot = slotContent.First(x => x.ContentType == _currentContentType);

            itemBorder.sprite = slot.BorderSprite;
            itemImage.material = slot.ItemIconMaterial;
            itemName.fontMaterial = slot.TextMaterial;

            _panelInfoGeneration.ShowPanels(collectableItem);

            slot.ShowContent.ForEach(content => content.Show());
            slot.HideContent.ForEach(content => content.Hide());

            closeButton.onClick.AddListener(OnCloseButtonClicked);
            ShowButtons();
            _currentCollectionSlot.OffPressedState();

            if (_currentContentType != CollectionSlotContentType.Lock)
            {
                selectButton.onClick.AddListener(SelectedSlot);
            }
        }

        private void StartShowAnimation()
        {
            rootTransform.DOAnchorPos(Vector3.zero, showSetting.Duration)
                .SetDelay(showSetting.Delay)
                .SetEase(showSetting.Curve);

            bgPreview.DOFade(fadeSetting.TargetValue, fadeSetting.Duration)
                .SetDelay(fadeSetting.Delay)
                .SetEase(fadeSetting.Ease);
        }

        private void StartHideAnimation(Action onComplete)
        {
            var finishPos = rootTransform.anchoredPosition - Vector2.up * rootTransform.rect.height;
            rootTransform.DOAnchorPos(finishPos, hideSetting.Duration)
                .SetDelay(hideSetting.Delay)
                .SetEase(hideSetting.Curve)
                .OnComplete(onComplete.Invoke);

            bgPreview.DOFade(fadeSetting.FromValue, fadeSetting.Duration)
                .SetDelay(fadeSetting.Delay)
                .SetEase(fadeSetting.Ease);
        }

        private void Hide()
        {
            StartHideAnimation(() =>
            {
                _panelInfoGeneration?.DestroyPanels();
                previewCanvas.enabled = false;
                previewUiCanvasGroup.Hide();
            });

            selectButton.onClick.RemoveAllListeners();
            closeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.RemoveAllListeners();
        }

        private void OnCloseButtonClicked()
        {
            Hide();
        }

        private void SelectedSlot()
        {
            if (_collectableItem.ItemType is CollectableItemType.AttackSpell or CollectableItemType.BuffSpell)
            {
                _currentCollectionSlot.SetReadySelectState();
            }
            else
            {
                _currentCollectionSlot.SetSelectedState();
            }

            Hide();
        }

        private void ShowButtons()
        {
            _currentBottomUi?.CanvasGroup.Hide();

            if (_currentContentType == CollectionSlotContentType.Lock)
            {
                _currentBottomUi = bottomPanels.First(x => x.BottomType == CollectionSlotButtonContentType.Lock);
                _currentBottomUi.BottomText.text = _collectableItem.OpenCondition;
                _currentBottomUi.CanvasGroup.Show();
                _currentBottomUi.CanvasGroup.DisableInteractable();
                return;
            }

            if (!_collectableItem.IsHaveMaxLevel &&
                _collectableItem.CurrentAmountCard >= _collectableItem.RequiredAmountCard)
            {
                _currentBottomUi = bottomPanels.First(x => x.BottomType == CollectionSlotButtonContentType.Price);
                _currentBottomUi.BottomText.text = $"{_collectableItem.UpgradePrice}";
                _currentBottomUi.CanvasGroup.Show();
                upgradeButton.onClick.RemoveAllListeners();

                if (_collectableItem.UpgradePrice > _metaCurrencyService.GetValue(Currencies.Soft))
                {
                    _currentBottomUi.BottomText.fontMaterial = _currentBottomUi.HaveNotMoneyForUpgradeMaterial;
                }
                else
                {
                    _currentBottomUi.BottomText.fontMaterial = _currentBottomUi.HaveMoneyForUpgradeMaterial;
                    upgradeButton.onClick.AddListener(UpgradeItem);
                }
            }
            else
            {
                _currentBottomUi = bottomPanels.First(x => x.BottomType == CollectionSlotButtonContentType.Card);

                if (_collectableItem.IsHaveMaxLevel)
                {
                    _currentCollectionSlot.SetCardPanelMaxLevel(_currentBottomUi);
                }
                else
                {
                    _currentBottomUi.BottomText.text =
                        $"{_collectableItem.CurrentAmountCard}/{_collectableItem.RequiredAmountCard}";
                    _currentBottomUi.ProgressBar.FillAmount = (float)_collectableItem.CurrentAmountCard /
                                                              (float)_collectableItem.RequiredAmountCard;
                }

                _currentBottomUi.CanvasGroup.Show();
                _currentBottomUi.CanvasGroup.DisableInteractable();
            }
        }

        private void ShowUpgradeEffect()
        {
            Sequence seq = DOTween.Sequence();

            Tween levelScaleFinish = itemLevelTransform.transform
                .DOScale(upgradeScaleLevelFinishTweenSettings.TargetValue,
                    upgradeScaleLevelFinishTweenSettings.Duration)
                .SetDelay(upgradeScaleLevelFinishTweenSettings.Delay)
                .SetEase(upgradeScaleLevelFinishTweenSettings.Curve)
                .Pause();

            Tween levelScaleStart = itemLevelTransform.transform
                .DOScale(upgradeScaleLevelStartTweenSettings.TargetValue, upgradeScaleLevelStartTweenSettings.Duration)
                .SetEase(upgradeScaleLevelStartTweenSettings.Curve)
                .Pause();

            seq.Append(levelScaleStart)
                .Append(levelScaleFinish)
                .Pause();

            _currentCollectionSlot.CreateAndShowUpgradeVFX(itemImage.transform);
            seq.Play();
        }

        private void UpgradeItem()
        {
            if (_collectableItem.UpgradePrice <= _metaCurrencyService.GetValue(Currencies.Soft))
            {
                _metaCurrencyService.AddValue(Currencies.Soft, (int)-_collectableItem.UpgradePrice);
                _collectableItem.UpgradeLevel();
                _repositoryService.Save();

                _currentBottomUi.BottomText.text = $"{_collectableItem.UpgradePrice}";
                _panelInfoGeneration.RefreshPanelValue();
                ShowButtons();
                itemLevel.text = (_collectableItem.LevelIndex + 1).ToString();
                ShowUpgradeEffect();
                _currentCollectionSlot.RefreshLevelAndBottom();
            }
        }
    }
}