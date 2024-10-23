using Sirenix.OdinInspector;
using TMPro;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Alchemy.Meta.Collection.UI
{
    public class CollectionSlotBottomUI : MonoBehaviour
    {
        public enum CollectionSlotButtonContentType
        {
            Lock = 0,
            Card = 1,
            Price = 2,
            Select = 3,
            Info = 4,
            Cancel = 5
        }

        [field: SerializeField] public CollectionSlotButtonContentType BottomType {  get; private set; }
        [field: SerializeField] public CanvasGroup CanvasGroup { get; private set; }
        [field: SerializeField] public TMP_Text BottomText { get; private set; }
        [field: SerializeField] public ProgressBar ProgressBar { get; private set; }
        
        [field: ShowIf(nameof(BottomType), CollectionSlotButtonContentType.Card)]
        [field: SerializeField] public Sprite HaveCardForUpgradeSprite { get; private set; }
        [field: ShowIf(nameof(BottomType), CollectionSlotButtonContentType.Card)]
        [field: SerializeField] public Sprite DontHaveCardForUpgradeSprite { get; private set; }
        [field: ShowIf(nameof(BottomType), CollectionSlotButtonContentType.Card)]
        [field: SerializeField] public string TextForMaxLevel { get; private set; }
        [field: ShowIf(nameof(BottomType), CollectionSlotButtonContentType.Card)]
        [field: SerializeField] public float SizeTextForMaxLevel { get; private set; }
        [field: ShowIf(nameof(BottomType), CollectionSlotButtonContentType.Card)]
        [field: SerializeField] public Image CardImage { get; private set; }
        
        [field: ShowIf(nameof(BottomType), CollectionSlotButtonContentType.Price)]
        [field: SerializeField] public Material HaveMoneyForUpgradeMaterial { get; private set; }
        [field: ShowIf(nameof(BottomType), CollectionSlotButtonContentType.Price)]
        [field: SerializeField] public Material HaveNotMoneyForUpgradeMaterial { get; private set; }
    }
}
