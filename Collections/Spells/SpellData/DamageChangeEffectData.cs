using Match;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Alchemy.Meta.Collection
{
    [CreateAssetMenu(menuName = "Alchemy/LevelData/DamageChangeEffectData")]
    public class DamageChangeEffectData : LevelData
    {
        public override IReadOnlyList<ILevelData> LevelsData => levelsData;

        [SerializeField] private DamageChangeEffectLevelData[] levelsData;
    }

    [Serializable]
    public class DamageChangeEffectLevelData : ILevelData
    {
        [field: SerializeField] public float DamageMultiplierInt { get; private set; }
        [field: SerializeField] public ModifierType StackType { get; private set; }
        [field: SerializeField] public bool Increase { get; private set; }
        [field: SerializeField] public bool AffectSlotsOnly { get; private set; }
        [field: SerializeField] public int AttacksAmount { get; private set; }
    }
}
