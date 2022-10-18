using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using PowerGolfArena.Input;
using PowerGolfArena.EventSystem;

namespace PowerGolfArena.Entities.Items
{
    public class RemoteControlBallBehaviour : ItemBehaviourBase
    {
        [HideInInspector] public GameObject clubModelPrefab;
        [HideInInspector] public float airStrafeForce;

        private bool _isStrafing;
        private float _stafeInput;

        private void OnAirStrafe(Player player, InputAction.CallbackContext ctx)
        {
            if (Player != player)
                return;

            if (ctx.phase == InputActionPhase.Performed)
            {
                _isStrafing = true;
                _stafeInput = ctx.ReadValue<float>();
            }
            else if (ctx.phase == InputActionPhase.Canceled)
            {
                _isStrafing = false;
                _stafeInput = 0;
            }
        }

        private void Strafe()
        {
            if (!Player.GolfballInstance.IsGrounded)
                Player.GolfballInstance.Rigidbody.AddForce(MainCamera.Instance.CachedTransform.right * _stafeInput * airStrafeForce, ForceMode.Impulse);
        }

        private void OnShotEnded(Character character)
        {
            Destroy(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            CheckForKillzoneTrigger(other);
        }

        private void CheckForKillzoneTrigger(Collider other)
        {
            Killzone killzone = other.GetComponent<Killzone>();

            if (killzone)
                Destroy(this);
        }

        private void OnEnable()
        {
            InputManager.Instance.GameplayInputs.AirStrafe += OnAirStrafe;
            EventManager.Instance.PlayerEvents.ShotEnded   += OnShotEnded;
        }
        protected override void OnDisableOrQuit()
        {
            InputManager.Instance.GameplayInputs.AirStrafe -= OnAirStrafe;
            EventManager.Instance.PlayerEvents.ShotEnded   -= OnShotEnded;
        }

        private void Start()
        {
            Player.ModelManager.SetClubModel(clubModelPrefab);
        }
        protected override void OnDestroyOrQuit()
        {
            Player.ModelManager.ResetClubModel();
        }

        private void FixedUpdate()
        {
            if (_isStrafing && !Player.GolfballInstance.IsGrounded)
                Strafe();
        }
    }
}
