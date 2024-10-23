using Alchemy.Meta.Summon;
using Alchemy.Meta.Weapon;
using SpellsSlotMachine;
using System;
using UnityEngine;

namespace Alchemy.Meta.Collection
{
    [Serializable]
    public class AmountSelectedItemsForType
    {
        [field: SerializeField] public CollectableItemType ItemType { get; private set; }
        [field: SerializeField] public int AmountOfSelectedItems { get; private set; }
    }

    [CreateAssetMenu(menuName = "Alchemy/CollectableItemsContainer")]
    public class CollectableItemsContainer : ScriptableObject
    {
        [field: SerializeField] public SpellsConfigsContainer AttackSpellContainer { get; private set; }
        [field: SerializeField] public SpellsConfigsContainer BuffSpellContainer { get; private set; }
        [field: SerializeField] public WeaponsConfigsContainer WeaponsContainer { get; private set; }
        [field: SerializeField] public SummonConfigsContainer SummonContainer { get; private set; }
        [field: SerializeField] public AmountSelectedItemsForType[] AmountSelectedItems { get; private set; }

        public int GetAmountItems(CollectableItemType itemType)
        {
            for (int i = 0; i < AmountSelectedItems.Length; i++)
            {
                if (AmountSelectedItems[i].ItemType == itemType)
                {
                    return AmountSelectedItems[i].AmountOfSelectedItems;
                }
            }

            return 0;
        }
    }
}