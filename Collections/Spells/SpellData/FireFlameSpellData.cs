using System;
using System.Collections.Generic;
using UnityEngine;

namespace Alchemy.Meta.Collection
{
    [CreateAssetMenu(menuName = "Alchemy/LevelData/FireFlameSpellData")]
    public class FireFlameSpellData : SpellLevelDataConfig
    {
        public override IReadOnlyList<ILevelData> LevelsData => levelsData;

        [SerializeField] private FireFlameSpellLevelData[] levelsData;
    }

    [Serializable]
    public class FireFlameSpellLevelData : SpellLevelData
    {
        [field: SerializeField] public float Damage { get; private set; }
    }
}
