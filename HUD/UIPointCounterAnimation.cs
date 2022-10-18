using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PowerGolfArena.Entities;
using PowerGolfArena.EventSystem;
using PowerGolfArena.Core;

namespace PowerGolfArena.UI
{
    public class UIPointCounterAnimation : EntityBase
    {
        [SerializeField] private TMP_ColorGradient ValidGradient;
        [SerializeField] private TMP_ColorGradient InvalidGradient;
        [SerializeField] private TMP_ColorGradient TextGradient;

        [HideInInspector] public TextMeshProUGUI Text;

        private float _originalFontSize;
        private float _newFontSize;

        private bool _pointsValid = false;
        private bool _pointsInvalid = false;

        private void Awake()
        {
            Text = GetComponent<TextMeshProUGUI>();
            _originalFontSize = Text.fontSize;
        }

        public void PointsValid()
        {
            _pointsValid = true;
            _newFontSize = _originalFontSize * 1.5f;
            Text.fontSize = _newFontSize;
            Text.colorGradientPreset = ValidGradient;
        }

        public void PointsInvalid()
        {
            _pointsInvalid = true;
            Text.colorGradientPreset = InvalidGradient;
        }

        public void OnReset()
        {
            Text.fontSize = 144;
            Text.colorGradientPreset = TextGradient;
        }

        private void Update()
        {
            if (_pointsValid && Text.fontSize < _newFontSize)
            {
                Text.fontSize += 2f;
                if (Text.fontSize >= _newFontSize)
                {
                    _pointsValid = false;
                }
            }

            if (_pointsInvalid && Text.fontSize > 0.0f)
            {
                Text.fontSize -= 2;
                if (Text.fontSize <= 0.0f)
                {
                    _pointsInvalid = false;
                }
            }
        }
    }
}
