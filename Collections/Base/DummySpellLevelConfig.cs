using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alchemy.Meta.Collection.Base
{
    [CreateAssetMenu(menuName = "Alchemy/LevelData/DummySpellLevelConfig")]
    public class DummySpellLevelConfig : SpellLevelDataConfig, IReadOnlyList<ILevelData>
    {
        public int Count => 0;

        public ILevelData this[int index] => null;
        
        public override IReadOnlyList<ILevelData> LevelsData => this;

        public IEnumerator<ILevelData> GetEnumerator()
        {
            return null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}