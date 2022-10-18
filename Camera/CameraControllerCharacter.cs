using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using PowerGolfArena.EventSystem;
using PowerGolfArena.Input;

namespace PowerGolfArena.Entities
{
    [RequireComponent(typeof(CinemachineFreeLook))]
    public class CameraControllerCharacter : CameraControllerBase
    {
        [SerializeField] private List<float> distancesFromCharacter = new List<float>() { 3.75f };
        [SerializeField] private float panningSpeed                 = 1.5f;
        [SerializeField] private float panningEasing                = 7.5f;
        [SerializeField] private float minPanning                   = -90.0f;
        [SerializeField] private float maxPanning                   = 90.0f;

        private int _currentDistanceFromPlayer;
        private float _totalRotation;
        private float _radiusVelocity;
        private float _heightVelocity;

        public void Rotate(Vector2 rotation)
        {
            _totalRotation = Mathf.Clamp(_totalRotation + rotation.y * panningSpeed * Mathf.Deg2Rad * Time.deltaTime, minPanning * Mathf.Deg2Rad, maxPanning * Mathf.Deg2Rad);
        }

        private void OnZoomIn(Player player, InputAction.CallbackContext ctx)
        {
            if (Player != player || ctx.phase != InputActionPhase.Performed)
                return;

            if (_currentDistanceFromPlayer > 0)
                _currentDistanceFromPlayer--;
        }
        private void OnZoomOut(Player player, InputAction.CallbackContext ctx)
        {
            if (Player != player || ctx.phase != InputActionPhase.Performed)
                return;

            if (_currentDistanceFromPlayer <= distancesFromCharacter.Count)
                _currentDistanceFromPlayer++;
        }

        private void OnGolfballStoppedMoving(Golfball golfball)
        {
            if (Player != golfball.Player)
                return;

            CinemachineCamera.PreviousStateIsValid = false;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            EventManager.Instance.PlayerEvents.GolfballStoppedMoving += OnGolfballStoppedMoving;
            InputManager.Instance.GameplayInputs.ZoomIn              += OnZoomIn;
            InputManager.Instance.GameplayInputs.ZoomOut             += OnZoomOut;
        }
        protected override void OnDisableOrQuit()
        {
            base.OnDisableOrQuit();

            EventManager.Instance.PlayerEvents.GolfballStoppedMoving -= OnGolfballStoppedMoving;
            InputManager.Instance.GameplayInputs.ZoomIn              -= OnZoomIn;
            InputManager.Instance.GameplayInputs.ZoomOut             -= OnZoomOut;
        }

        private void Awake()
        {
            _currentDistanceFromPlayer = 0;
        }

        private void Update()
        {
            if (Mathf.Approximately(Time.deltaTime, 0.0f))
                return;

            for (int i = 0; i < CinemachineCamera.m_Orbits.Length; i++)
            {
                CinemachineFreeLook.Orbit orbit = CinemachineCamera.m_Orbits[i];

                float distanceFromCharacter = distancesFromCharacter[_currentDistanceFromPlayer];
                Vector2 targetOrbit         = new Vector2(Mathf.Cos(_totalRotation) * distanceFromCharacter, Mathf.Sin(_totalRotation) * distanceFromCharacter);
                orbit.m_Radius              = Mathf.SmoothDamp(orbit.m_Radius, targetOrbit.x, ref _radiusVelocity, panningEasing);
                orbit.m_Height              = Mathf.SmoothDamp(orbit.m_Height, targetOrbit.y, ref _heightVelocity, panningEasing);

                CinemachineCamera.m_Orbits[i] = orbit;
            }
        }
    }
}
