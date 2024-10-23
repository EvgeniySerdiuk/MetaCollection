using System;
using UnityEngine;

namespace Alchemy.Meta.Collection
{
    [Serializable]
    public class CollectableItemLevelData
    {
        [field: SerializeField] public int CardStepForUpgrade { get; private set; } = 5;
    }
}
