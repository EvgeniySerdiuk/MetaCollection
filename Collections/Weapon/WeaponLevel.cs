using System;
using UnityEngine;

namespace Alchemy.Meta.Collection
{
    [Serializable]
    public class WeaponLevel
    {
        [field: SerializeField] public int CharacterHealth { get; private set; }
        [field: SerializeField] public int CharacterDamage { get; private set; }
    }
}