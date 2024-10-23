using System.Collections.Generic;
using UnityEngine;

namespace Alchemy.Meta.Collection.UI
{
    public class CollectionPreviewUpgradeInfoUI : MonoBehaviour
    {
        [SerializeField] private StatsUpgradeInfoUI statsUpgradeInfoUI;
        [SerializeField] private Transform root;

        private List<StatsUpgradeInfoUI> _panels = new();

        public void Construct(CollectionItemInfoPanelUIConfig[] itemInfoPanel, int levelIndex)
        {
            for (int i = 0; i < itemInfoPanel.Length; i++)
            {
                for (int j = 0; j < itemInfoPanel[i].StatsUsed.Length; j++)
                {
                    var info = Instantiate(statsUpgradeInfoUI, root);
                    var statPanel = itemInfoPanel[i].StatsUsed[j];

                    var before = CalculateValue(statPanel, levelIndex) *
                                 (statPanel.ValueMultiplier == 0 ? 1 : statPanel.ValueMultiplier);

                    var after = CalculateValue(statPanel, levelIndex + 1) *
                                (statPanel.ValueMultiplier == 0 ? 1 : statPanel.ValueMultiplier);

                    info.Construct(statPanel.StatIcon, string.Format(statPanel.ValueFormat, before),
                        string.Format(statPanel.ValueFormat, after), statPanel.StatDescription);
                    
                    _panels.Add(info);
                }
            }
        }

        public void RefreshValue(CollectionItemInfoPanelUIConfig[] itemInfoPanel, int levelIndex)
        {
            for (int i = 0; i < itemInfoPanel.Length; i++)
            {
                for (int j = 0; j < itemInfoPanel[i].StatsUsed.Length; j++)
                {
                    var statPanel = itemInfoPanel[i].StatsUsed[j];

                    var before = CalculateValue(statPanel, levelIndex) *
                                 (statPanel.ValueMultiplier == 0 ? 1 : statPanel.ValueMultiplier);

                    var after = CalculateValue(statPanel, levelIndex + 1) *
                                (statPanel.ValueMultiplier == 0 ? 1 : statPanel.ValueMultiplier);
                    
                    _panels[i].UpgradeStatsEffect(string.Format(statPanel.ValueFormat, before),string.Format(statPanel.ValueFormat, after));
                }
            }
        }
        
        private float CalculateValue(CollectionItemStatsUI collectionItemStatsUI, int levelIndex)
        {
            if (levelIndex == 0 || collectionItemStatsUI.LevelData is SummonLevelsData)
            {
                return collectionItemStatsUI.GetValue(levelIndex);
            }

            return collectionItemStatsUI.GetValue(levelIndex);
        }
    }
}