using System;
using System.Collections.Generic;
using UnityEngine;

namespace Alchemy.Meta.Collection
{
    [CreateAssetMenu(menuName = "Alchemy/LevelData/ProjectileSpellData")]
    public class ProjectileSpellData : SpellLevelDataConfig
    {
        public override IReadOnlyList<ILevelData> LevelsData => levelsData;

        [SerializeField] private ProjectileSpellLevelData[] levelsData;
    }

    [Serializable]
    public class ProjectileSpellLevelData : SpellLevelData
    {
        [field: SerializeField] public int AmountProjectile { get; private set; }
        [field: SerializeField] public float Damage { get ; private set; }
    }
}
