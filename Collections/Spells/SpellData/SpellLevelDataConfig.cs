using System;
using System.Collections.Generic;
using UnityEngine;

namespace Alchemy.Meta.Collection
{
    public class SpellLevelDataConfig : LevelData
    {
        [field: SerializeField] public float BaseCharacterDamage { get; private set; }
        [field: SerializeField] public float CharacterDamagePerLevel { get; private set; }

        [field: SerializeField] public int BaseCharacterHealth { get; private set; }
        [field: SerializeField] public int CharacterHealthPerLevel { get; private set; }

        public override IReadOnlyList<ILevelData> LevelsData => null;
    }

    [Serializable]
    public class SpellLevelData : ILevelData
    {

    }
}