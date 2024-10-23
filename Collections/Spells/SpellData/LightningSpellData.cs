using System;
using System.Collections.Generic;
using UnityEngine;

namespace Alchemy.Meta.Collection
{
    [CreateAssetMenu(menuName = "Alchemy/LevelData/LigtningSpellData")]
    public class LigtningSpellData : SpellLevelDataConfig
    {
        public override IReadOnlyList<ILevelData> LevelsData => levelsData;

        [SerializeField] private LigtningSpellLevelData[] levelsData;
    }

    [Serializable]
    public class LigtningSpellLevelData : SpellLevelData
    {
        [field: SerializeField] public float Damage { get; private set; }
    }
}
