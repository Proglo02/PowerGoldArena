using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PowerGolfArena.EventSystem;
using PowerGolfArena.Entities;

namespace PowerGolfArena.UI
{
    public class UIMultipler : EntityBase
    {
        [SerializeField] private UIPointCounterAnimation pointCounterText;
        [SerializeField] private UIMultiplierAnimator addedPointsText;
        [SerializeField] private UIMultiplierAnimator multiplierText;

        private void OnPointCounterChanged(Golfball golfball)
        {
            if (golfball.PointCounter > 0f)
                pointCounterText.Text.text = golfball.PointCounter.ToString();
            else
                pointCounterText.Text.text = "";
        }

        private void OnCounterChanged(Golfball golfball)
        {
            if (golfball.AddedPoints > 0f)
            {
                addedPointsText.Text.text = "+ " + golfball.AddedPoints.ToString();
                addedPointsText.AddedPoints();
            }
            else
                addedPointsText.Text.text = "";
        }

        private void OnPointMultiplierChanged(Golfball golfball)
        {
            if (golfball.PointMultiplier > 1f)
            {
                multiplierText.Text.text = "x " + golfball.PointMultiplier.ToString();
                multiplierText.AddedPoints();
            }
            else
                multiplierText.Text.text = "";
        }

        private void OnValidatePoints(bool valid)
        {
            if (valid)
                pointCounterText.PointsValid();
            else
                pointCounterText.PointsInvalid();
        }

        private void OnShotBegan(Character character)
        {
            pointCounterText.OnReset();
        }

        private void OnEnable()
        {
            EventManager.Instance.PlayerEvents.PointCounterChanged    += OnPointCounterChanged;
            EventManager.Instance.PlayerEvents.AddedPointsChanged     += OnCounterChanged;
            EventManager.Instance.PlayerEvents.PointMultiplierChanged += OnPointMultiplierChanged;
            EventManager.Instance.PlayerEvents.ValidatePoints         += OnValidatePoints;
            EventManager.Instance.PlayerEvents.ShotBegan              += OnShotBegan;
        }

        protected override void OnDisableOrQuit()
        {
            EventManager.Instance.PlayerEvents.PointCounterChanged    -= OnPointCounterChanged;
            EventManager.Instance.PlayerEvents.AddedPointsChanged     -= OnCounterChanged;
            EventManager.Instance.PlayerEvents.PointMultiplierChanged -= OnPointMultiplierChanged;
            EventManager.Instance.PlayerEvents.ValidatePoints         -= OnValidatePoints;
            EventManager.Instance.PlayerEvents.ShotBegan              -= OnShotBegan;
        }

        private void Awake()
        {
            pointCounterText.Text.text = "";
            addedPointsText.Text.text  = "";
            multiplierText.Text.text   = "";
        }
    }
}
