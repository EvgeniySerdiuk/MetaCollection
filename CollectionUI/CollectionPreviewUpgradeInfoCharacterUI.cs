using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Alchemy.Meta.Collection.UI
{
    public class CollectionPreviewUpgradeInfoCharacterUI : MonoBehaviour
    {
        [SerializeField] private StatsUpgradeInfoUI upgradeInfoUI;

        [SerializeField] private Sprite damageIcon;
        [SerializeField] private Sprite healthIcon;

        [SerializeField] private string damageDescription;
        [SerializeField] private string healthDescription;

        [SerializeField] private string damageValueFormat = "{0}";
        [SerializeField] private string healthValueFormat = "{0}";

        [MinValue(1)] [SerializeField] private int damageValueMultiplier;

        public void ShowAttackInfo(float before, float after)
        {
            upgradeInfoUI.Construct(damageIcon,
                string.Format(damageValueFormat, Mathf.CeilToInt(damageValueMultiplier * before)),
                string.Format(damageValueFormat, Mathf.CeilToInt(damageValueMultiplier * after)),
                damageDescription);
        }

        public void ShowHealthInfo(float before, float after)
        {
            upgradeInfoUI.Construct(healthIcon, string.Format(healthValueFormat, before),
                string.Format(healthValueFormat, after),
                healthDescription);
        }

        public void RefreshAttackInfo(float before, float after)
        {
            upgradeInfoUI.UpgradeStatsEffect(
                string.Format(damageValueFormat, Mathf.CeilToInt(damageValueMultiplier * before)),
                string.Format(damageValueFormat, Mathf.CeilToInt(damageValueMultiplier * after)));
        }

        public void RefreshHealthInfo(float before, float after)
        {
            upgradeInfoUI.UpgradeStatsEffect(
                string.Format(healthValueFormat, before), string.Format(healthValueFormat, after));
        }
    }
}