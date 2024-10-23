using System;
using System.Collections.Generic;
using UnityEngine;

namespace Alchemy.Meta.Collection
{
    [CreateAssetMenu(menuName = "Alchemy/LevelData/ShieldEffectData")]
    public class ShieldEffectData : LevelData
    {
        public override IReadOnlyList<ILevelData> LevelsData => levelsData;

        [SerializeField] private ShieldEffectLevelData[] levelsData;
    }

    [Serializable]
    public class ShieldEffectLevelData : ILevelData
    {
        [field: SerializeField] public float ShieldCapacity { get; private set; }
        [field: SerializeField] public float CharacterEffectScale { get; private set; }
        [field: SerializeField] public float SummonsEffectScale { get; private set; }
        [field: SerializeField] public float ShieldRadius { get; private set; }
        [field: SerializeField] public float SummonShieldRadius { get; private set; }
    }
}
