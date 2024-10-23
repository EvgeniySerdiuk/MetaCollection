using System.Collections.Generic;
using UnityEngine;

namespace Alchemy.Meta.Collection
{
    public abstract class LevelData : ScriptableObject
    {
        public abstract IReadOnlyList<ILevelData> LevelsData { get; }
    }
}
