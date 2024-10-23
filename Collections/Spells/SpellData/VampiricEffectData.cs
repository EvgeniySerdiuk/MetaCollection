using System;
using System.Collections.Generic;
using UnityEngine;

namespace Alchemy.Meta.Collection
{
    [CreateAssetMenu(menuName = "Alchemy/LevelData/VampiricEffectData")]
    public class VampiricEffectData : LevelData
    {
        public override IReadOnlyList<ILevelData> LevelsData => levelsData;

        [SerializeField] private VampiricEffectLevelData[] levelsData;
    }

    [Serializable]
    public class VampiricEffectLevelData : ILevelData
    {
        [field: SerializeField] public float VampiricPercent { get; private set; }
    }
}
