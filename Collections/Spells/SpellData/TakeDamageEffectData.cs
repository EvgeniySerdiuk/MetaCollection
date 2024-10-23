using Match;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Alchemy.Meta.Collection
{
    [CreateAssetMenu(menuName = "Alchemy/LevelData/TakeDamageEffectData")]
    public class TakeDamageEffectData : LevelData
    {
        public override IReadOnlyList<ILevelData> LevelsData => levelsData;

        [SerializeField] private TakeDamageEffectLevelData[] levelsData;
    }

    [Serializable]
    public class TakeDamageEffectLevelData : ILevelData
    {
        [field: SerializeField] public bool UseDamagePercentOfCharacterDamage { get; private set; }
        [field: SerializeField] public bool UseDamagePercentOfTargetHealth { get; private set; }

        [field: SerializeField] public ModifierType StackType { get; private set; }

        [field: HideIf(nameof(UseDamagePercentOfCharacterDamage))]
        [field: SerializeField]
        public float DamagePerTick { get; private set; }

        [field: ShowIf(nameof(UseDamagePercentOfCharacterDamage))]
        [field: SerializeField]
        public float PercentOfDamage { get; private set; }
    }
}
