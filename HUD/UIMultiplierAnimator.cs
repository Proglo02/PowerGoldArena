using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PowerGolfArena.Entities;
using PowerGolfArena.EventSystem;

namespace PowerGolfArena.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class UIMultiplierAnimator : EntityBase
    {
        [HideInInspector] public TextMeshProUGUI Text;
        [SerializeField] private TMP_ColorGradient gradient;
        [SerializeField] private bool dissapearInWater;

        private float _originalFontSize;
        private float _newFontSize;
        private bool landedInWater;

        private void Awake()
        {
            Text = GetComponent<TextMeshProUGUI>();
            _originalFontSize = Text.fontSize;
        }
        public void AddedPoints()
        {
            if (!landedInWater)
            {
                _newFontSize = _originalFontSize * 1.5f;
                Text.color = Vector4.one;

                Text.fontSize = _newFontSize;
            }
        }

        private void Update()
        {
            if (_newFontSize > _originalFontSize || landedInWater)
            {
                _newFontSize--;
                Text.fontSize = _newFontSize;
            }

            if (landedInWater && _newFontSize < 0 && dissapearInWater)
            {
                landedInWater = false;
                Text.color = Vector4.zero;
                Text.colorGradientPreset = null;
            }
        }

        private void OnLandedInWater(Golfball golfball)
        {
            if (dissapearInWater)
            {
                Text.fontSize = _newFontSize;
                Text.colorGradientPreset = gradient;
                landedInWater = true;
            }
        }

        private void OnEnable()
        {
            EventManager.Instance.PlayerEvents.GolfballLandedInWater += OnLandedInWater;
        }

        protected override void OnDisableOrQuit()
        {
            EventManager.Instance.PlayerEvents.GolfballLandedInWater -= OnLandedInWater;
        }
    }
}
