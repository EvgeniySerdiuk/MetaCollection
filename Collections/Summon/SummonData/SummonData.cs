using System;
using System.Collections.Generic;
using UnityEngine;

namespace Alchemy.Meta.Collection
{
    [CreateAssetMenu(menuName = "Alchemy/Summon/SummonLevelData")]
    public class SummonLevelsData : LevelData
    {
        [field: SerializeField] public int BaseCharacterDamage { get; private set; }
        [field: SerializeField] public int CharacterDamagePerLevel { get; private set; }

        [field: SerializeField] public int BaseCharacterHealth { get; private set; }
        [field: SerializeField] public int CharacterHealthPerLevel { get; private set; }
   
        [field: SerializeField] public SummonLevelData[] LevelData { get; private set; }

        public override IReadOnlyList<ILevelData> LevelsData => LevelData;
    }

    [Serializable]
    public class SummonLevelData : ILevelData
    {
        [field: SerializeField] public int Health { get; private set; }
    }
}
