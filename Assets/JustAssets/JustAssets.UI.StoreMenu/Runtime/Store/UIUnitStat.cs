using System;
using JustAssets.Shared.Providers;
using JustAssets.UI.StoreMenu.Store.Configuration;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JustAssets.UI.StoreMenu.Store
{
    public class UIUnitStat : MonoBehaviour
    {
        [SerializeField]
        private UIColorPalette _colorPalette = null;

        [SerializeField]
        private EStyle _displayStyle = EStyle.Absolute;

        [SerializeField]
        private Image _statIcon;

        [SerializeField]
        private UIStatConfiguration _statConfiguration;

        [SerializeField]
        private TMP_Text _valueCurrent = null;

        [SerializeField]
        private TMP_Text _valueNew = null;

        private StatId _statName;

        private int _statValue;

        private int? _newStatValue;

        public void Init(StatId statName, int statValue, int? newStatValue)
        {
            _statName = statName;
            _statValue = statValue;
            _newStatValue = newStatValue;

            InitName();
            InitValue();
        }
        
        [Serializable]
        private enum EStyle
        {
            Absolute,
            Delta
        }

        private static string GetSignedDelta(int delta)
        {
            return delta > 0 ? "+" + delta : delta.ToString();
        }

        private void InitName()
        {
            _statIcon.sprite = _statConfiguration.Get(_statName);
        }

        private void InitValue()
        {
            switch (_displayStyle)
            {
                case EStyle.Absolute:
                    InitValueAbsolute();
                    break;
                case EStyle.Delta:
                    InitValueRelative();
                    break;
            }
        }

        private void InitValueAbsolute()
        {
            _valueCurrent.enabled = true;
            _valueCurrent.text = _statValue.ToString();

            var isVisible = _newStatValue.HasValue && _newStatValue.Value != _statValue;
            _valueNew.enabled = isVisible;

            if (isVisible)
            {
                var isImprovement = _newStatValue > _statValue;

                var delta = _newStatValue - _statValue;
                _valueNew.text = isImprovement ? $"+{delta}" : delta.ToString();

                if (isImprovement)
                    _valueNew.color = _colorPalette[UIColorPalette.ImprovementText];
                else if (_newStatValue < _statValue)
                    _valueNew.color = _colorPalette[UIColorPalette.DeteriorationText];
                else
                    _valueNew.color = _colorPalette[UIColorPalette.NeutralText];
            }
        }

        private void InitValueRelative()
        {
            _valueCurrent.enabled = false;
            _valueNew.enabled = true;

            var delta = _newStatValue - _statValue;
            _valueNew.text = delta != null ? GetSignedDelta(delta.Value) : _statValue.ToString();
            _valueNew.color = _newStatValue == null || delta == 0 ? _colorPalette[UIColorPalette.NeutralText] :
                delta.Value > 0 ? _colorPalette[UIColorPalette.ImprovementText] : _colorPalette[UIColorPalette.DeteriorationText];
        }
    }
}