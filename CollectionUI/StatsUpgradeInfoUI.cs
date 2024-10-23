using System;
using System.Collections;
using Alchemy.Match.SlotMachine.SpellsSlot;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Alchemy.Meta.Collection.UI
{
    public class StatsUpgradeInfoUI : MonoBehaviour
    {
        [SerializeField] private Image statIcon;

        [SerializeField] private TMP_Text beforeText;
        [SerializeField] private TMP_Text afterText;

        [SerializeField] private TMP_Text descriptionText;

        [SerializeField] private TweenSettings<float> upgradeScaleLevelStartTweenSettings;
        [SerializeField] private TweenSettings<float> upgradeScaleLevelFinishTweenSettings;

        public void Construct(Sprite icon, string before, string after, string description)
        {
            statIcon.sprite = icon;

            beforeText.text = before.Replace(',', '.');
            afterText.text = after.Replace(',', '.');

            descriptionText.text = description;
        }

        public void UpgradeStatsEffect(string before, string after)
        {
            ShowUpgradeEffect(beforeText.transform, beforeText, before);

            StartCoroutine(Delay(
                upgradeScaleLevelStartTweenSettings.Duration + upgradeScaleLevelFinishTweenSettings.Duration,
                () => { ShowUpgradeEffect(afterText.transform, afterText, after); }));
        }

        private void ShowUpgradeEffect(Transform statTransform, TMP_Text changedText, string newStat)
        {
            Sequence seq = DOTween.Sequence();

            Tween levelScaleStart = statTransform.transform
                .DOScale(upgradeScaleLevelStartTweenSettings.TargetValue, upgradeScaleLevelStartTweenSettings.Duration)
                .SetEase(upgradeScaleLevelStartTweenSettings.Curve)
                .Pause();

            Tween levelScaleFinish = statTransform.transform
                .DOScale(upgradeScaleLevelFinishTweenSettings.TargetValue,
                    upgradeScaleLevelFinishTweenSettings.Duration)
                .SetEase(upgradeScaleLevelFinishTweenSettings.Curve)
                .Pause();

            seq.Append(levelScaleStart)
                .Append(levelScaleFinish)
                .Pause();

            seq.Play();
            changedText.text = newStat.Replace(',', '.');
        }

        private IEnumerator Delay(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }
    }
}