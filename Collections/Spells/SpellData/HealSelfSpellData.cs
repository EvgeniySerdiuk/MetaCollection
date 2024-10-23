using System;
using System.Collections.Generic;
using UnityEngine;

namespace Alchemy.Meta.Collection
{
    [CreateAssetMenu(menuName = "Alchemy/LevelData/HealSelfSpellData")]
    public class HealSelfSpellData : SpellLevelDataConfig
    {
        public override IReadOnlyList<ILevelData> LevelsData => levelsData;

        [SerializeField] private HealSelfSpellLevelData[] levelsData;
    }

    [Serializable]
    public class HealSelfSpellLevelData : SpellLevelData
    {
        [field: SerializeField] public float HealValue { get; private set; }
    }
}
