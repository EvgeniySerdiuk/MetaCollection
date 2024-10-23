using Sirenix.OdinInspector;
using System;
using System.Data;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Alchemy.Meta.Collection
{
    [CreateAssetMenu(menuName = "Alchemy/UI/CollectionItemInfoPanelConfig")]
    public class CollectionItemInfoPanelUIConfig : ScriptableObject
    {
        [field: SerializeField] public LevelData LevelData { get; private set; }

        [ListDrawerSettings(IsReadOnly = true)]
        [SerializeField] private CollectionItemStatsUI[] stats;

        public CollectionItemStatsUI[] StatsUsed => stats.Where(x => x.UseStat).ToArray();

        [Button]
        private void RefreshStats()
        {
            if (LevelData.LevelsData == null || LevelData.LevelsData.Count < 1)
            {
                Debug.LogError("This level data does not contain data! Insert other data");
                return;
            }

            stats = null;

            var dataType = LevelData.LevelsData[0].GetType();

            var dataFields = dataType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x => x.FieldType == typeof(int) || x.FieldType == typeof(float))
                .ToArray();

            stats = new CollectionItemStatsUI[dataFields.Length];

            for (int i = 0; i < dataFields.Length; i++)
            {
                stats[i] = new CollectionItemStatsUI(dataFields[i], LevelData);
            }
        }
    }  
}
