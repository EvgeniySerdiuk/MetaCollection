using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Alchemy.Meta.Collection
{
    [Serializable]
    public class CollectionItemStatsUI
    {
        [field: SerializeField] public bool UseStat { get; private set; }

        [field: ReadOnly]
        [field: SerializeField]
        public string Name { get; private set; }

        [field: ShowIf("UseStat")]
        [field: SerializeField]
        public string StatDescription { get; private set; }

        [field: ShowIf("UseStat")]
        [field: SerializeField]
        public Sprite StatIcon { get; private set; }

        [field: ShowIf("UseStat")]
        [field: SerializeField]
        public string ValueFormat { get; private set; } = "{0}";

        [field: ShowIf("UseStat")]
        [field: SerializeField]
        public int ValueMultiplier { get; private set; } = 0;

        public LevelData LevelData => _levelData;

        [HideInInspector] [SerializeField] private LevelData _levelData;

        [ValueDropdown(nameof(GetLevelFields))]
        [InfoBox("Field error", InfoMessageType.Error, VisibleIf = nameof(HasErrorInField))]
        [SerializeField]
        private string _fieldName;

        public CollectionItemStatsUI(FieldInfo fieldInfo, LevelData levelData)
        {
            _fieldName = fieldInfo.Name;
            _levelData = levelData;

            Name = FormatName(fieldInfo.Name);
        }

        private string FormatName(string name)
        {
            if (name.StartsWith("<") && name.EndsWith(">k__BackingField"))
            {
                string withoutSuffix = name[1..name.IndexOf(">")];
                name = withoutSuffix;
            }

            return name;
        }

        public float GetValue(int levelIndex)
        {
            var dataType = _levelData.LevelsData[0].GetType();

            var dataFields = dataType.GetField(_fieldName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            var value = dataFields.GetValue(_levelData.LevelsData[levelIndex]);
            return (float.Parse(value.ToString()));
        }

        private IEnumerable<string> GetLevelFields() =>
            _levelData == null || _levelData.LevelsData.Count == 0
                ? Array.Empty<string>()
                : _levelData.LevelsData[0].GetType()
                    .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Select(x => x.Name);

        private bool HasErrorInField => _levelData == null || _levelData.LevelsData.Count == 0 ||
                                        _levelData.LevelsData[0].GetType().GetField(_fieldName,
                                            BindingFlags.Public 
                                            | BindingFlags.NonPublic 
                                            | BindingFlags.Instance) == null;
    }
}