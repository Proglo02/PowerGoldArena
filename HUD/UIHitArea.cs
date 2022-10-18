using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using PowerGolfArena.Core;
using PowerGolfArena.Entities;
using PowerGolfArena.Input;
using PowerGolfArena.EventSystem;
using PowerGolfArena.Physics;

namespace PowerGolfArena.UI
{
    public class UIHitArea : EntityBase
    {
        [SerializeField] private RectTransform _crosshairTransform;
        [SerializeField] private float _crosshairSpeed;
        [SerializeField] private Vector2 _offset;
        [SerializeField] private float _scale;

        void UpdateAim(Player player)
        {
            _crosshairTransform.anchoredPosition = _offset + player.CharacterInstance.ShootAim * _scale * _crosshairSpeed;
        }

        private void OnEnable()
        {
            EventManager.Instance.PlayerEvents.ShootAimChanged += UpdateAim;
        }
        protected override void OnDisableOrQuit()
        {
            EventManager.Instance.PlayerEvents.ShootAimChanged -= UpdateAim;
        }
    }
}
